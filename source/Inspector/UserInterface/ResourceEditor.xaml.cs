using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ChristianMoser.WpfInspector.UserInterface.Controls.PropertyItems;

namespace ChristianMoser.WpfInspector.UserInterface
{
    /// <summary>
    /// Interaction logic for ResourceEditor.xaml
    /// </summary>
    public partial class ResourceEditor : Window
    {
        private readonly ResourcesEditorViewModel _viewModel;

        public ResourceEditor(PropertyItem propertyItem)
        {
            InitializeComponent();
            _viewModel = new ResourcesEditorViewModel(propertyItem);
            DataContext = _viewModel;
        }

        private void OkClicked(object sender, RoutedEventArgs e)
        {
            _viewModel.ApplySelectedResource();
            Close();
        }

        private void ListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var listBox = sender as ListBox;
            if (listBox != null && listBox.SelectedItem != null)
            {
                _viewModel.ApplySelectedResource();
                Close();
            }
        }
    }
}
