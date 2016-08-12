using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows.Threading;
using System.Windows;

namespace ChristianMoser.WpfInspector.Services
{
    public class FocusedElementService
    {
        private readonly DispatcherTimer _timer;
        private FocusInfo _info = new FocusInfo();

        public FocusedElementService()
        {
            _timer = new DispatcherTimer {Interval = TimeSpan.FromMilliseconds(300)};
            _timer.Tick += OnTimerTick;
            _timer.Start();
        }

        public FocusInfo Info
        {
            get { return _info; }
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            var keyboardFocusElement = Keyboard.FocusedElement;
            if( !ReferenceEquals(keyboardFocusElement, Info.KeyboardFocusElement))
            {
                Info.KeyboardFocusElement = keyboardFocusElement as FrameworkElement;
            }
        }

        
    }
}
