using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;
using ChristianMoser.WpfInspector.Services;
using ChristianMoser.WpfInspector.Services.ElementTree;

namespace ChristianMoser.WpfInspector.UserInterface
{
    /// <summary>
    /// Interaction logic for TestWindow.xaml
    /// </summary>
    public partial class TestWindow : Window
    {
        private readonly List<string> _items = new List<string>();
        private readonly ListCollectionView _view;

        public TestWindow()
        {
            var timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(1000);
            timer.Tick += OnTick;
            //timer.Start();

            InitializeComponent();
            DataContext = ServiceLocator.Resolve<SelectedTreeItemService>();
            new InspectorWindow().Show();

            _items.Add(DateTime.Now.ToString());
            _items.Add(DateTime.Now.ToString());
            _view = new ListCollectionView(_items);
            //list.ItemsSource = _view;            
        }

        void OnTick(object sender, EventArgs e)
        {
            
            _items.Add(DateTime.Now.ToString());
            _view.Refresh();
        }

        private void Local_Click(object sender, RoutedEventArgs e)
        {
            new Window{ Width = 1024, Height = 768, Title = "KABA Paxos", Owner =  this}.Show();
        }
    }
}
