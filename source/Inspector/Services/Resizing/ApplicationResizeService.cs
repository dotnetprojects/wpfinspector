using System.Collections.Generic;
using System.Windows;

namespace ChristianMoser.WpfInspector.Services.Resizing
{
    /// <summary>
    /// Service to set the size of the application window
    /// </summary>
    public class ApplicationResizeService
    {
        #region Private Members

        private readonly List<SizeItem> _sizes = new List<SizeItem>();

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationResizeService"/> class.
        /// </summary>
        public ApplicationResizeService()
        {
            Sizes.Add(new SizeItem(640, 480, SetWindowSize));
            Sizes.Add(new SizeItem(800, 600, SetWindowSize));
            Sizes.Add(new SizeItem(1024, 768, SetWindowSize));
            Sizes.Add(new SizeItem(1280, 1024, SetWindowSize));
            Sizes.Add(new SizeItem(1600, 1200, SetWindowSize));
        }

        #endregion

        /// <summary>
        /// Gets the sizes.
        /// </summary>
        public List<SizeItem> Sizes
        {
            get { return _sizes; }
        }

        private static void SetWindowSize(double width, double height)
        {
            // TODO: Use better logic to find the main window
            if (Application.Current != null && Application.Current.MainWindow != null)
            {
                Application.Current.MainWindow.Width = width;
                Application.Current.MainWindow.Height = height;
            }
        }
    }
}
