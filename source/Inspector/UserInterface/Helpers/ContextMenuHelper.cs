using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace ChristianMoser.WpfInspector.UserInterface.Helpers
{
    public static class ContextMenuHelper
    {
        public static readonly DependencyProperty OpenOnClickProperty = DependencyProperty.RegisterAttached("OpenOnClick",
                        typeof(bool), typeof(ContextMenuHelper),
                        new FrameworkPropertyMetadata(false, OpenOnClickChanged));

        public static void SetOpenOnClick(UIElement element, bool value)
        {
            element.SetValue(OpenOnClickProperty, value);
        }

        public static bool GetOpenOnClick(UIElement element)
        {
            return (bool)element.GetValue(OpenOnClickProperty);
        }

        public static void OpenOnClickChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var control = sender as FrameworkElement;
            if ((bool)e.NewValue && control != null)
            {
                control.PreviewMouseLeftButtonDown += (s,args) =>
                                                   {
                                                      if( control.ContextMenu != null  )
                                                      {
                                                          control.ContextMenu.PlacementTarget = control;
                                                          control.ContextMenu.IsOpen = true;
                                                      }
                                                   };
            }
        }

    }
}
