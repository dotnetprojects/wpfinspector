using System.Windows;
using System.Windows.Input;

namespace ChristianMoser.WpfInspector.UserInterface
{
    /// <summary>
    /// Interaction logic for SelectorWindow.xaml
    /// </summary>
    public partial class SelectorWindow : Window
    {
        private readonly SelectorWindowViewModel _viewModel  = new SelectorWindowViewModel();

        public SelectorWindow()
        {
            InitializeComponent();
            DataContext = _viewModel;
        }

        private void ListBoxMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            _viewModel.InspectCommand.Execute(null);
        }
       
    }
}
