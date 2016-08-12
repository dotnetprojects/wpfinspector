using System;
using System.Windows;
using System.Windows.Threading;
using ChristianMoser.WpfInspector.Services;
using ChristianMoser.WpfInspector.Services.ElementTree;
using ChristianMoser.WpfInspector.Utilities;
using ChristianMoser.WpfInspector.Win32;
using System.Windows.Interop;
using System.Diagnostics;

namespace ChristianMoser.WpfInspector.UserInterface
{
    /// <summary>
    /// Interaction logic for InspectorWindow.xaml
    /// </summary>
    public partial class InspectorWindow : ResizeableWindow
    {
        #region Private Members

        private readonly InspectorViewModel _viewModel;
        private IntPtr _handle;
        private readonly DispatcherTimer _ensureWindowEnabledTimer;

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="InspectorWindow"/> class.
        /// </summary>
        public InspectorWindow()
        {
            InitializeComponent();
            _viewModel = new InspectorViewModel();
            DataContext = _viewModel;

            _ensureWindowEnabledTimer = new DispatcherTimer();
            _ensureWindowEnabledTimer.Interval = TimeSpan.FromMilliseconds(500);
            _ensureWindowEnabledTimer.Tick += OnEnsureWindowEnabled;
            _ensureWindowEnabledTimer.IsEnabled = true;

            Loaded += OnLoaded;
        }

        #endregion

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke((Action) PostInitializeWindow, DispatcherPriority.Background);
        }

        private void PostInitializeWindow()
        {
            Activate();
            Focus();

            // Set the name of the inspected app to the title
            var process = Process.GetCurrentProcess();
            string processName = process.MainWindowTitle ?? process.ProcessName;
            Title += " -  " + processName;

            try
            {
                var interopHelper = new WindowInteropHelper(this);
                _handle = interopHelper.Handle;
                NativeMethods.SetForegroundWindow(_handle);
            }
            catch (Exception)
            {
            }

            if( _viewModel.ShowUsageHint)
            {
                var usageHintWindow = new UsageHintWindow();
                usageHintWindow.Owner = this;
                usageHintWindow.ShowDialog();
            }

            // Initialize this service
            ServiceLocator.Resolve<MouseElementService>();
        }

        private void OnEnsureWindowEnabled(object sender, EventArgs e)
        {
            if (_handle != IntPtr.Zero)
            {
                if (!NativeMethods.IsWindowEnabled(_handle))
                {
                    NativeMethods.EnableWindow(_handle, true);
                }
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            ServiceLocator.ShutDown();
        }
    }
}
