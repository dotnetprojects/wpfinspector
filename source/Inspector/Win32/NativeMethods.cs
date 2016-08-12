using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Drawing;

namespace ChristianMoser.WpfInspector.Win32
{


    public static class NativeMethods
    {
        [Flags]
        public enum ProcessAccessFlags : uint
        {
            All = 0x001F0FFF,
            Terminate = 0x00000001,
            CreateThread = 0x00000002,
            VmOperation = 0x00000008,
            VmRead = 0x00000010,
            VmWrite = 0x00000020,
            DupHandle = 0x00000040,
            SetInformation = 0x00000200,
            QueryInformation = 0x00000400,
            Synchronize = 0x00100000
        }

        public enum TernaryRasterOperations : uint
        {
            Srccopy = 0x00CC0020,
            Srcpaint = 0x00EE0086,
            Srcand = 0x008800C6,
            Srcinvert = 0x00660046,
            SRCERASE = 0x00440328,
            NOTSRCCOPY = 0x00330008,
            NOTSRCERASE = 0x001100A6,
            MERGECOPY = 0x00C000CA,
            MERGEPAINT = 0x00BB0226,
            PATCOPY = 0x00F00021,
            PATPAINT = 0x00FB0A09,
            PATINVERT = 0x005A0049,
            DSTINVERT = 0x00550009,
            BLACKNESS = 0x00000042,
            WHITENESS = 0x00FF0062
        }


        public enum ModuleFilterFlags : int
        {
            Default = 0x0,
            x32Bit = 0x01,
            x64Bit = 0x02,
            All = 0x03
        }

        [Flags]
        public enum AllocationType
        {
            Commit = 0x1000,
            Reserve = 0x2000,
            Decommit = 0x4000,
            Release = 0x8000,
            Reset = 0x80000,
            Physical = 0x400000,
            TopDown = 0x100000,
            WriteWatch = 0x200000,
            LargePages = 0x20000000
        }

        [Flags]
        public enum MemoryProtection
        {
            Execute = 0x10,
            ExecuteRead = 0x20,
            ExecuteReadWrite = 0x40,
            ExecuteWriteCopy = 0x80,
            NoAccess = 0x01,
            ReadOnly = 0x02,
            ReadWrite = 0x04,
            WriteCopy = 0x08,
            GuardModifierflag = 0x100,
            NoCacheModifierflag = 0x200,
            WriteCombineModifierflag = 0x400
        }

        public enum HookType : int
        {
            WH_JOURNALRECORD = 0,
            WH_JOURNALPLAYBACK = 1,
            WH_KEYBOARD = 2,
            WH_GETMESSAGE = 3,
            WH_CALLWNDPROC = 4,
            WH_CBT = 5,
            WH_SYSMSGFILTER = 6,
            WH_MOUSE = 7,
            WH_HARDWARE = 8,
            WH_DEBUG = 9,
            WH_SHELL = 10,
            WH_FOREGROUNDIDLE = 11,
            WH_CALLWNDPROCRET = 12,
            WH_KEYBOARD_LL = 13,
            WH_MOUSE_LL = 14
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct WINDOWINFO
        {
            public uint cbSize;
            public RECT rcWindow;
            public RECT rcClient;
            public uint dwStyle;
            public uint dwExStyle;
            public uint dwWindowStatus;
            public uint cxWindowBorders;
            public uint cyWindowBorders;
            public ushort atomWindowType;
            public ushort wCreatorVersion;

            public WINDOWINFO(Boolean? filler)
                : this()   // Allows automatic initialization of "cbSize" with "new WINDOWINFO(null/true/false)".
            {
                cbSize = (UInt32)(Marshal.SizeOf(typeof(WINDOWINFO)));
            }

        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT         
        {            
            public Int32 Left;    
            public Int32 Top;   
            public Int32 Right;
            public Int32 Bottom;
        }

        public delegate bool EnumWindowsCallBackDelegate(IntPtr hwnd, IntPtr lParam);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWindowEnabled(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool EnableWindow(IntPtr hWnd, bool bEnable);

        [DllImport("user32.Dll")]
        public static extern int EnumWindows(EnumWindowsCallBackDelegate callback, IntPtr lParam);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumChildWindows(IntPtr hwndParent, EnumWindowsCallBackDelegate lpEnumFunc, IntPtr lParam);

        [DllImport("user32.Dll")]
        public static extern int GetWindowThreadProcessId(IntPtr hwnd, out int processId);

        [DllImport("psapi.dll", CharSet = CharSet.Unicode)]
        public static extern int GetModuleFileNameEx(
            [In] IntPtr ProcessHandle,
            [In] [Optional] IntPtr ModuleHandle,
            [Out] StringBuilder FileName,
            [In] int Size
            );

        [DllImport("psapi.dll", SetLastError = true), SuppressUnmanagedCodeSecurityAttribute]
        public static extern bool EnumProcessModules(IntPtr hProcess,
                                                    [In][Out] IntPtr[] lphModule,
                                                    uint cb,
                                                    [MarshalAs(UnmanagedType.U4)] out uint lpcbNeeded);

        [DllImport("psapi.dll", SetLastError = true)]
        public static extern bool EnumProcessModules(IntPtr hProcess,
        [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.U4)] [In][Out] uint[] lphModule, uint cb, [MarshalAs(UnmanagedType.U4)] out uint lpcbNeeded);

        [DllImport("psapi.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumProcessModulesEx(
            [In] IntPtr ProcessHandle,
            [Out] IntPtr[] ModuleHandles,
            [In] uint Size,
            [Out] out uint RequiredSize,
            [In] ModuleFilterFlags dwFilterFlag
            );

        [DllImport("kernel32.dll"), SuppressUnmanagedCodeSecurityAttribute]
        public static extern IntPtr OpenProcess(ProcessAccessFlags dwDesiredAccess,
                                         [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle,
                                         int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true), SuppressUnmanagedCodeSecurityAttribute]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        public static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress,
           uint dwSize, AllocationType flAllocationType, MemoryProtection flProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, IntPtr lpBuffer, uint nSize, out int lpNumberOfBytesWritten);

        [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        public static extern UIntPtr GetProcAddress(IntPtr hModule, string procName);

        public delegate int HookProc(int code, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetWindowsHookEx(HookType hookType, UIntPtr lpfn,
        IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        public static extern bool VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress,
           int dwSize, AllocationType dwFreeType);

        [DllImport("user32.dll")]
        public static extern int CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern uint RegisterWindowMessage(string lpString);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("kernel32", SetLastError = true)]
        public static extern IntPtr LoadLibrary(string lpFileName);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool FreeLibrary(IntPtr hModule);

        [DllImport("user32.dll")]
        internal static extern bool GetWindowInfo(IntPtr hwnd, ref WINDOWINFO pwi);

        [DllImport("gdi32")]
        public static extern IntPtr CreateCompatibleBitmap(IntPtr hDC, int nWidth, int nHeight);

        [DllImport("gdi32.dll")]
        internal static extern bool BitBlt(IntPtr hObject, int nXDest, int nYDest, int nWidth,
        int nHeight, IntPtr hObjSource, int nXSrc, int nYSrc, TernaryRasterOperations dwRop);

        [DllImport("gdi32")]
        public static extern IntPtr CreateCompatibleDC(IntPtr hDC);

        [DllImport("gdi32")]
        public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);

        [DllImport("gdi32")]
        public static extern IntPtr DeleteDC(IntPtr hDC);

        [DllImport("user32", ExactSpelling = true, SetLastError = true)]
        internal static extern IntPtr GetWindowDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern IntPtr GetDC(IntPtr ptr);

        [DllImport("user32", EntryPoint = "ReleaseDC", SetLastError = true)]
        public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("gdi32")]
        public static extern bool DeleteObject(IntPtr handle);

        [DllImport("user32.dll")]
        public static extern IntPtr WindowFromPoint(Point point);

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out Point lpPoint);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWow64Process([In] IntPtr processHandle,
             [Out, MarshalAs(UnmanagedType.Bool)] out bool wow64Process);

        [DllImport("shell32.dll")]
        public static extern IntPtr ExtractIcon(IntPtr hInst, string lpszExeFileName, int nIconIndex);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetWindow(IntPtr hWnd, int uCmd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("uxtheme.dll", CharSet = CharSet.Auto)]
        public static extern int GetCurrentThemeName(StringBuilder pszThemeFileName, int dwMaxNameChars, StringBuilder pszColorBuff, int dwMaxColorChars, StringBuilder pszSizeBuff, int cchMaxSizeChars);

    }
}
