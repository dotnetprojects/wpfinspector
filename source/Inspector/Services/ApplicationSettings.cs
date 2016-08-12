using System.ComponentModel;
using System.Windows;
using ChristianMoser.WpfInspector.Utilities;

namespace ChristianMoser.WpfInspector.Services
{
    public class ApplicationSettings : INotifyPropertyChanged
    {
        #region Private Members

        private double _mainWindowLeft;
        private double _mainWindowTop;
        private double _mainWindowWidth = 800;
        private double _mainWindowHeight = 600;
        private WindowState _winddowState;

        #endregion

        /// <summary>
        /// Gets or sets the left position of the main window.
        /// </summary>
        public double MainWindowLeft
        {
            get { return _mainWindowLeft; }
            set { PropertyChanged.ChangeAndNotify(ref _mainWindowLeft, value, this, "MainWindowLeft"); }
        }

        /// <summary>
        /// Gets or sets the height of the main window.
        /// </summary>
        public double MainWindowTop
        {
            get { return _mainWindowTop; }
            set { PropertyChanged.ChangeAndNotify(ref _mainWindowTop, value, this, "MainWindowTop"); }
        }

        /// <summary>
        /// Gets or sets the width of the main window.
        /// </summary>
        public double MainWindowWidth
        {
            get { return _mainWindowWidth; }
            set { PropertyChanged.ChangeAndNotify(ref _mainWindowWidth, value, this, "MainWindowWidth"); }
        }

        /// <summary>
        /// Gets or sets the height of the main window.
        /// </summary>
        public double MainWindowHeight
        {
            get { return _mainWindowHeight; }
            set { PropertyChanged.ChangeAndNotify(ref _mainWindowHeight, value, this, "MainWindowHeight"); }
        }

        /// <summary>
        /// Gets or sets the window state
        /// </summary>
        public WindowState WindowState
        {
            get { return _winddowState; }
            set { PropertyChanged.ChangeAndNotify(ref _winddowState, value, this, "WindowState"); }
        }

        #region Implementation of INotifyPropertyChanged

        /// <summary>
        /// See <see cref="INotifyPropertyChanged.PropertyChanged" />
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
