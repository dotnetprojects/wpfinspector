using System.ComponentModel;
using System.Windows;
using ChristianMoser.WpfInspector.Utilities;

namespace ChristianMoser.WpfInspector.Services
{
    public class FocusInfo : INotifyPropertyChanged
    {
        private FrameworkElement _keyboardFocusElement;
        
        public FrameworkElement KeyboardFocusElement
        {
            get { return _keyboardFocusElement; }
            set { PropertyChanged.ChangeAndNotify(ref _keyboardFocusElement, value, this, "KeyboardFocusElement"); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
