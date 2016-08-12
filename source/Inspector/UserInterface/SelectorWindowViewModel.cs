using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using ChristianMoser.WpfInspector.Services;
using ChristianMoser.WpfInspector.Utilities;

namespace ChristianMoser.WpfInspector.UserInterface
{
    public class SelectorWindowViewModel
    {
        #region Private Members

        private readonly ManagedApplicationsService _managedApplicationsService;
        private readonly InspectionService _inspectionService;
        private readonly UpdateCheckService _updateCheckService;

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectorWindowViewModel"/> class.
        /// </summary>
        public SelectorWindowViewModel()
        {
            _inspectionService = ServiceLocator.Resolve<InspectionService>();
            _updateCheckService = ServiceLocator.Resolve<UpdateCheckService>();

            UpdateInformation = _updateCheckService.UpdateInformation;

            _managedApplicationsService = new ManagedApplicationsService();
            ManagedApplicationsInfo = _managedApplicationsService.ManagerApplicationsInfo;
            ManagedApplicationsInfo.ManagedApplicationInfos.CurrentChanged += (s, e) => InspectCommand.RaiseCanExecuteChanged();
            ManagedApplicationsInfo.ManagedApplicationInfos.CollectionChanged += (s, e) =>
                                                      {
                                                          if (ManagedApplicationsInfo.ManagedApplicationInfos.CurrentItem == null)
                                                              ManagedApplicationsInfo.ManagedApplicationInfos.MoveCurrentToFirst();
                                                      };
            RefreshCommand = new Command<object>( RefreshApplicationList);
            InspectCommand = new Command<object>( _ => Inspect(),_ => ManagedApplicationsInfo.ManagedApplicationInfos.CurrentItem != null);
            ExitCommand  = new Command<object>(o => Application.Current.Shutdown());
            AboutCommand = new Command<object>(o => new AboutWindow().ShowDialog());
            VisitWebpageCommand = new Command<object>(VisitWebPage);
        }

        #endregion

        /// <summary>
        /// Gets or sets the update information.
        /// </summary>
        /// <value>The update information.</value>
        public UpdateInformation UpdateInformation { get; private set; }

        /// <summary>
        /// Gets the refresh command.
        /// </summary>
        public Command<object> RefreshCommand { get; private set; }

        /// <summary>
        /// Gets the inspect command.
        /// </summary>
        public Command<object> InspectCommand { get; private set; }

        /// <summary>
        /// Gets the exit command.
        /// </summary>
        public Command<object> ExitCommand { get; private set; }

        /// <summary>
        /// Gets the exit command.
        /// </summary>
        public Command<object> AboutCommand { get; private set; }

        /// <summary>
        /// Gets the visit webpage command.
        /// </summary>
        public Command<object> VisitWebpageCommand { get; private set; }

        /// <summary>
        /// Gets the managed applications info.
        /// </summary>
        public ManagedApplicationsInfo ManagedApplicationsInfo { get; private set; }


        #region Private Helpers

        private static void VisitWebPage(object obj)
        {
            try
            {
                Process.Start("http://www.wpftutorial.net/Inspector.html");
            }
            catch (Exception)
            {
                MessageBox.Show("Navigation to webpage failed.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Inspect()
        {
            if (ManagedApplicationsInfo.ManagedApplicationInfos.CurrentItem == null)
            {
                return;
            }

            try
            {
                string error = _inspectionService.Inspect((ManagedApplicationInfo) ManagedApplicationsInfo.ManagedApplicationInfos.CurrentItem);
                if (string.IsNullOrEmpty(error))
                {
                    Application.Current.Shutdown();
                }
                else
                {
                    MessageBox.Show(error, "Application inspection failed.", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Application inspection failed.", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RefreshApplicationList(object obj)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            _managedApplicationsService.RefreshList();
            Mouse.OverrideCursor = null;
        }

        #endregion
    }
}
