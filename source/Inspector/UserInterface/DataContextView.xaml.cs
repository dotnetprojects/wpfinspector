using System.Windows.Controls;

namespace ChristianMoser.WpfInspector.UserInterface
{
    /// <summary>
    /// Interaction logic for DataContextWindow.xaml
    /// </summary>
    public partial class DataContextView : UserControl
    {
        public DataContextView()
        {
            InitializeComponent();
            DataContext = new DataContextViewModel();
        }
    }
}
