using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;
using ChristianMoser.WpfInspector.Utilities;

namespace ChristianMoser.WpfInspector.UserInterface.Controls
{
    public class SearchTextBox : TextBox
    {
        public Command<object> ClearTextCommand { get; private set; }

        public SearchTextBox()
        {
            ClearTextCommand = new Command<object>(o => Text = string.Empty, o => !string.IsNullOrEmpty(Text));
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            if( e.Key == Key.Escape)
            {
                MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            }
            base.OnKeyUp(e);
        }

        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            ClearTextCommand.RaiseCanExecuteChanged();
            base.OnTextChanged(e);
        }
    }
}
