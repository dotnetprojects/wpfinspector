using System.Windows.Controls;

namespace ChristianMoser.WpfInspector.UserInterface
{
    /// <summary>
    /// Interaction logic for EventsView.xaml
    /// </summary>
    public partial class EventsView : UserControl
    {
        public EventsView()
        {
            InitializeComponent();
            DataContext = new EventsViewModel();
        }
    }
}
