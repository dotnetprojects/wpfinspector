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
using System.Windows.Navigation;
using System.Windows.Shapes;
using ChristianMoser.WpfInspector.Services.Analyzers;
using ChristianMoser.WpfInspector.Services.DataObjects;

namespace ChristianMoser.WpfInspector.UserInterface
{
    /// <summary>
    /// Interaction logic for IssueListView.xaml
    /// </summary>
    public partial class IssueListView : UserControl
    {
        private readonly IssuesListViewModel _viewModel;

        public IssueListView()
        {
            InitializeComponent();
            _viewModel = new IssuesListViewModel();
            DataContext = _viewModel;
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if( e.AddedItems != null && e.AddedItems.Count > 0 )
            {
                _viewModel.OnIssueSelected(e.AddedItems[0] as Issue);
            }
        }
    }
}
