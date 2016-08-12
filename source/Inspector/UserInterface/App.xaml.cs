using System.Windows;
using ChristianMoser.WpfInspector.Services;

namespace ChristianMoser.WpfInspector.UserInterface
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        
        protected override void OnExit(ExitEventArgs e)
        {
            ServiceLocator.Resolve<Process32Service>().Dispose();
            base.OnExit(e);
        }
    }
}
