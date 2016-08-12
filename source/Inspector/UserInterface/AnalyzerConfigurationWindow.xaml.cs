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

namespace ChristianMoser.WpfInspector.UserInterface
{
    /// <summary>
    /// Interaction logic for AnalyzerConfigurationWindow.xaml
    /// </summary>
    public partial class AnalyzerConfigurationWindow : Window
    {
        private readonly AnalyzerConfigurationViewModel _viewModel;

        public AnalyzerConfigurationWindow()
        {
            InitializeComponent();
            _viewModel = new AnalyzerConfigurationViewModel();
            DataContext =_viewModel;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
            _viewModel.ReAnalyze();
        }
    }
}
