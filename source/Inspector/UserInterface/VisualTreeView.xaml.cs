using System.Windows.Controls;
using ChristianMoser.WpfInspector.Services.ElementTree;

namespace ChristianMoser.WpfInspector.UserInterface
{
    /// <summary>
    /// Interaction logic for VisualTreeView.xaml
    /// </summary>
    public partial class VisualTreeView : UserControl
    {
        public VisualTreeView()
        {
            InitializeComponent();
            DataContext = new VisualTreeViewModel();
        }
    }
}
