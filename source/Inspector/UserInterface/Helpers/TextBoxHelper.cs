using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace ChristianMoser.WpfInspector.UserInterface.Helpers
{
    public static class TextBoxHelper
    {
        #region UpdateOnEnter

        public static readonly DependencyProperty UpdateOnEnterProperty = DependencyProperty.RegisterAttached("UpdateOnEnter",
            typeof(bool), typeof(TextBoxHelper), new FrameworkPropertyMetadata(false, OnUpdateOnEnterChanged));

        public static bool GetUpdateOnEnter(DependencyObject sender)
        {
            return (bool)sender.GetValue(UpdateOnEnterProperty);
        }

        public static void SetUpdateOnEnter(DependencyObject sender, bool value)
        {
            sender.SetValue(UpdateOnEnterProperty, value);
        }

        private static void OnUpdateOnEnterChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var textBox = sender as TextBox;
            if (textBox != null)
            {
                textBox.KeyUp += (s, e) =>
                {
                    if (e.Key == Key.Enter)
                    {
                        var expression = textBox.GetBindingExpression(TextBox.TextProperty);
                        if (expression != null)
                        {
                            expression.UpdateSource();
                        }
                        textBox.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                    }
                };
            }
        }

        #endregion

        #region SelectOnFocus

        public static readonly DependencyProperty SelectOnFocusProperty = DependencyProperty.RegisterAttached("SelectOnFocus",
            typeof(bool), typeof(TextBoxHelper), new FrameworkPropertyMetadata(false, OnSelectOnFocusChanged));

        public static bool GetSelectOnFocus(DependencyObject sender)
        {
            return (bool)sender.GetValue(SelectOnFocusProperty);
        }

        public static void SetSelectOnFocus(DependencyObject sender, bool value)
        {
            sender.SetValue(SelectOnFocusProperty, value);
        }

        private static void OnSelectOnFocusChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var textBox = sender as TextBox;
            if (textBox != null)
            {
                textBox.GotFocus += (s, e) => textBox.Dispatcher.BeginInvoke((Action)(() => 
                    {
                      textBox.Focus();
                      textBox.SelectAll();
                    }),
                    DispatcherPriority.Background);
            }
        }

        #endregion
    }
}
