using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Threading;
using ChristianMoser.WpfInspector.Win32;

namespace ChristianMoser.WpfInspector.Services
{
    public class ManagedApplicationsService
    {
        #region Private Members

        private readonly List<ManagedApplicationInfo> _managedApplications = new List<ManagedApplicationInfo>();
        private readonly Dictionary<int, bool> _validProcessIdCache = new Dictionary<int, bool>();
        private readonly Dictionary<int, string> _fileVersionCache = new Dictionary<int, string>();
        private readonly Dictionary<int, int> _bitnessCache = new Dictionary<int, int>();
        private readonly DispatcherTimer _refreshTimer = new DispatcherTimer(DispatcherPriority.Background);

        private readonly ManagedApplicationsInfo _managedApplicationsInfo;
        private readonly Process32Service _process32Service;
        private readonly int _currentProcessId;
        private readonly bool _is64BitProcess;

        #endregion

        #region Construction

        public ManagedApplicationsService()
        {
            _managedApplicationsInfo = new ManagedApplicationsInfo(_managedApplications);
            _managedApplicationsInfo.State = ApplicationsInfoState.Loading;

            _currentProcessId = Process.GetCurrentProcess().Id;
            _is64BitProcess = PlatformHelper.Is64BitProcess;

            _refreshTimer.Interval = TimeSpan.FromSeconds(3);
            _refreshTimer.Tick += (s, e) => RefreshList();
            _refreshTimer.Start();

            if(PlatformHelper.Is64BitProcess)
            {
                _process32Service = ServiceLocator.Resolve<Process32Service>();
            }

            Dispatcher.CurrentDispatcher.BeginInvoke((Action)RefreshList, DispatcherPriority.Background);
        }

        #endregion

        #region IManagedApplicationsService Members

        public ManagedApplicationsInfo ManagerApplicationsInfo
        {
            get { return _managedApplicationsInfo; }
        }

        public void RefreshList()
        {
            var applicationInfos = new List<ManagedApplicationInfo>();
            

            try
            {
                applicationInfos.AddRange(GetManagedApplications());

                if (_is64BitProcess)
                {
                    applicationInfos.AddRange(_process32Service.GetManagedApplications());
                }

                // Add new applications
                foreach (ManagedApplicationInfo applicationInfo in applicationInfos)
                {
                    if (!_managedApplications.Contains(applicationInfo))
                    {
                        _managedApplications.Add(applicationInfo);
                    }
                }

                // Remove old applications
                var itemsToRemove = new List<ManagedApplicationInfo>();
                foreach (ManagedApplicationInfo managedApplicationInfo in _managedApplications)
                {
                    if( !applicationInfos.Contains(managedApplicationInfo))
                    {
                        itemsToRemove.Add(managedApplicationInfo);
                    }
                }
                foreach (var removeInfo in itemsToRemove)
                {
                    _managedApplications.Remove(removeInfo);
                }

                _managedApplicationsInfo.ManagedApplicationInfos.Refresh();
                _managedApplicationsInfo.State = ApplicationsInfoState.Available;
            }
            catch (Exception)
            {
                _managedApplicationsInfo.State = ApplicationsInfoState.Error;
            }

        }

        public List<ManagedApplicationInfo> GetManagedApplications()
        {
            _validProcessIdCache.Clear();
            var managedApplications = new List<ManagedApplicationInfo>();
            var checkedProcessIds = new List<int>();

            foreach (var hWnd in EnumerateWindows())
            {
                int processId;
                NativeMethods.GetWindowThreadProcessId(hWnd, out processId);

                if (checkedProcessIds.Contains(processId))
                {
                    continue;
                }

                if (processId == _currentProcessId)
                    continue;

                //Process p = GetProcess(hWnd);)
                string runtimeVersion;
                int bitness;
                if (GetIsManagedApplication(hWnd, processId, out runtimeVersion, out bitness) )
                {
                    IntPtr mainWindowHandle = new MainWindowFinder().FindMainWindow(processId);
                    string windowText = GetWindowText(mainWindowHandle);
                    if (mainWindowHandle == hWnd)
                    {
                        var process = Process.GetProcessById(processId);
                        if (!process.ProcessName.Contains("devenv") && !process.ProcessName.Contains("PresentationHost") 
                            && !process.ProcessName.ToLower().Contains("inspector"))
                        {
                            var applicationInfo = new ManagedApplicationInfo(windowText, hWnd, processId, runtimeVersion, bitness);
                            managedApplications.Add(applicationInfo);
                            checkedProcessIds.Add(processId);    
                        }
                    }
                }
            }

            return managedApplications;
        }

        #endregion

        #region Private Methods

        private static string GetWindowText(IntPtr hWnd)
        {
            // Allocate correct string length first
            int length = NativeMethods.GetWindowTextLength(hWnd);
            var sb = new StringBuilder(length + 1);
            NativeMethods.GetWindowText(hWnd, sb, sb.Capacity);
            return sb.ToString();
        }

       
        private static bool EnumWindowsCallback(IntPtr hwnd, IntPtr lParam)
        {
            ((List<IntPtr>)((GCHandle)lParam).Target).Add(hwnd);
            return true;
        }

        private IEnumerable<IntPtr> EnumerateWindows()
        {
            _managedApplications.Clear();
            var windowList = new List<IntPtr>();
            var childWindowList = new List<IntPtr>();
            GCHandle handle = GCHandle.Alloc(windowList);
            GCHandle childHandle = GCHandle.Alloc(childWindowList);
            try
            {
                NativeMethods.EnumWindows(EnumWindowsCallback, (IntPtr)handle);
                foreach (var hWnd in windowList)
                {
                    NativeMethods.EnumChildWindows(hWnd, EnumWindowsCallback, (IntPtr)childHandle);    
                }
            }
            finally
            {
                handle.Free();
                childHandle.Free();
            }

            return windowList.Union(childWindowList);

        }

        private bool GetIsManagedApplication(IntPtr windowHandle, int processId, out string versionInfo, out int bitness)
        {
            bool isValid = false;
            versionInfo = null;
            bitness = _is64BitProcess ? 64 : 32;

            try
            {
                if (windowHandle == IntPtr.Zero)
                    return false;
                if (_validProcessIdCache.TryGetValue(processId, out isValid))
                {
                    versionInfo = _fileVersionCache[processId];
                    bitness = _bitnessCache[processId];
                    return isValid;
                }
                if (processId == _currentProcessId)
                    isValid = false;
                else
                {
                    var lphModule = new IntPtr[2048];
                    uint byteCount = (uint)lphModule.Length * sizeof(uint);

                    IntPtr hProcess = NativeMethods.OpenProcess(NativeMethods.ProcessAccessFlags.All, true, processId);
                    
                    if (hProcess != IntPtr.Zero)
                    {
                        bool enumModuleSuccess;

                        uint bytesNeeded;
                        if (Environment.OSVersion.Version.Major >= 6)
                        {
                            enumModuleSuccess = NativeMethods.EnumProcessModulesEx(hProcess, lphModule,
                                                                                        byteCount, out bytesNeeded,
                                                                                        NativeMethods.ModuleFilterFlags.x64Bit);
                        }
                        else
                        {
                            enumModuleSuccess = NativeMethods.EnumProcessModules(hProcess, lphModule, byteCount, out bytesNeeded);
                        }

                        if (enumModuleSuccess)
                        {
                            uint moduleCount = (uint)(bytesNeeded / IntPtr.Size);

                            for (uint i = 0; i < moduleCount; i++)
                            {
                                var exePathBuilder = new StringBuilder(1024);
                                NativeMethods.GetModuleFileNameEx(hProcess, lphModule[i], exePathBuilder, exePathBuilder.Capacity);
                                var exePath = exePathBuilder.ToString().ToLower();
                                if (_is64BitProcess && exePath.Contains("wow64"))
                                {
                                    bitness = 32;
                                }
                                if (exePath.Contains("presentationcore") ||
                                    exePath.Contains("presentationframework") ||
                                    exePath.Contains("wpfgfx"))
                                {
                                    var fileVersionInfo = FileVersionInfo.GetVersionInfo(exePath);
                                    versionInfo = string.Format("{0}.{1}", fileVersionInfo.FileMajorPart, fileVersionInfo.FileMinorPart);
                                    isValid = true;
                                    break;
                                }
                            }
                        }

                        NativeMethods.CloseHandle(hProcess);
                    }
                }
                _fileVersionCache[processId] = versionInfo;
                _validProcessIdCache[processId] = isValid;
                _bitnessCache[processId] = bitness;
            }
            catch (Exception)
            {
                return isValid;
            }
            return isValid;

        }

        #endregion

    }
}
