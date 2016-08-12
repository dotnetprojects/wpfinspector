using System.Windows.Controls;

namespace ChristianMoser.WpfInspector.UserInterface
{
    /// <summary>
    /// Interaction logic for UiAutomationTreeView.xaml
    /// </summary>
    public partial class AutomationTreeView : UserControl
    {
        public AutomationTreeView()
        {
            InitializeComponent();
            DataContext = new AutomationViewModel();
        }
      
    }
}
