using ChristianMoser.WpfInspector.UserInterface;
using System;
using System.Windows;
using ChristianMoser.WpfInspector.Services;
namespace ChristianMoser.WpfInspector.Hook
{
    public class Inspector
    {
        public static void Inject()
        {
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            var inspectorWinow = new InspectorWindow();
            ServiceLocator.RegisterInstance<InspectorWindow>(inspectorWinow);
            if (Application.Current != null && Application.Current.MainWindow != null)
            {
                inspectorWinow.Owner = Application.Current.MainWindow;
            }
            inspectorWinow.Show();
        }

        static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show(((Exception) e.ExceptionObject).Message);
        }
    }
}
