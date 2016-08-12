using System;
using System.Runtime.CompilerServices;

namespace ChristianMoser.WpfInspector.Win32
{
    public class PlatformHelper
    {
        public static readonly bool Is64BitProcess = (IntPtr.Size == 8);

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static bool IsWow64Process(IntPtr processHandle)
        {
            if ((Environment.OSVersion.Version.Major == 5 && Environment.OSVersion.Version.Minor >= 1) ||
                Environment.OSVersion.Version.Major >= 6)
            {
                bool retVal;
                if (!NativeMethods.IsWow64Process(processHandle, out retVal))
                {
                    return false;
                }
                return retVal;
            }
            return false;
        }
    }
}
