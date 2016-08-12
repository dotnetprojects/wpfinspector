using System.Windows;
using ChristianMoser.WpfInspector.UserInterface.Controls.PropertyItems;


namespace ChristianMoser.WpfInspector.UserInterface
{
    /// <summary>
    /// Interaction logic for BindingEditor.xaml
    /// </summary>
    public partial class BindingEditorWindow : Window
    {
        private readonly BindingEditorViewModel _viewModel;

        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="BindingEditorWindow"/> class.
        /// </summary>
        /// <param name="propertyItem">The property item.</param>
        public BindingEditorWindow(PropertyItem propertyItem)
        {
            InitializeComponent();
            _viewModel = new BindingEditorViewModel(propertyItem);
            DataContext = _viewModel;

            tree.SelectedItemChanged += (s, e) => _viewModel.SelectedPathItem = (PathItem)tree.SelectedValue;
        }

        #endregion

        #region Private Helpers

        private void OkClick(object sender, RoutedEventArgs e)
        {
            if (_viewModel.ActivateBinding())
            {
                DialogResult = true;
                Close();
            }
        }

        #endregion
    }
}
