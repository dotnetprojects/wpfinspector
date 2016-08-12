using System.Windows;

namespace ChristianMoser.WpfInspector.UserInterface
{
    /// <summary>
    /// Interaction logic for UsageHintWindow.xaml
    /// </summary>
    public partial class UsageHintWindow : Window
    {
        public UsageHintWindow()
        {
            InitializeComponent();
            DataContext = new UsageHintViewModel();
        }
    }
}
