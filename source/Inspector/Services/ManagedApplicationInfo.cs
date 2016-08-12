using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Windows.Media;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;
using ChristianMoser.WpfInspector.Win32;
using System.Drawing.Imaging;

namespace ChristianMoser.WpfInspector.Services
{
    [DataContract]
    public class ManagedApplicationInfo
    {
        
        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="ManagedApplicationInfo"/> class.
        /// </summary>
        public ManagedApplicationInfo(string name, IntPtr hWnd, int processId, string runtimeVersion, int bitness)
        {
            Name = name;
            HWnd = hWnd;
            ProcessId = processId;
            RuntimeVersion = runtimeVersion;
            Bitness = bitness;
            IconData = GetApplicationIcon();
        }

        #endregion

        /// <summary>
        /// Gets the process icon.
        /// </summary>
        public ImageSource ProcessIcon
        {
            get
            {
                if( IconData != null)
                {
                    var ms = new MemoryStream(IconData);
                    var bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = ms;
                    bitmapImage.EndInit();
                    bitmapImage.Freeze();
                    return bitmapImage;
                }
                return new BitmapImage(new Uri("pack://application:,,,/Inspector;component/Images/noprocessicon.png"));
            }
        }

        /// <summary>
        /// Gets or sets the icon data.
        /// </summary>
        [DataMember]
        public byte[] IconData { get; set; }

        /// <summary>
        /// Gets or sets the H WND.
        /// </summary>
        [DataMember]
        public IntPtr HWnd { get; set; }

        /// <summary>
        /// Gets or sets the process id.
        /// </summary>
        [DataMember]
        public int ProcessId { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the runtime version.
        /// </summary>
        [DataMember]
        public string RuntimeVersion { get; set; }

        /// <summary>
        /// Gets or sets the bitness of the process (32 or 64)
        /// </summary>
        [DataMember]
        public int Bitness { get; set; }

        #region Private Helpers

        private byte[] GetApplicationIcon()
        {
            var process = Process.GetProcessById(ProcessId);
            if (process.MainModule != null)
            {
                IntPtr hIcon = NativeMethods.ExtractIcon(IntPtr.Zero, process.MainModule.FileName, 0);
                if (hIcon != IntPtr.Zero)
                {
                    var icon = Icon.FromHandle(hIcon);
                    var bitmap = icon.ToBitmap();
                    var ms = new MemoryStream();
                    bitmap.Save(ms,ImageFormat.Png);
                    icon.Save(ms);
                    return ms.ToArray();
                }
            }
            return null;
        }

        public override bool Equals(object obj)
        {
            var applicationInfo = obj as ManagedApplicationInfo;
            if( applicationInfo != null)
            {
                return ProcessId == applicationInfo.ProcessId;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return ProcessId;
        }

        #endregion
    }
}
