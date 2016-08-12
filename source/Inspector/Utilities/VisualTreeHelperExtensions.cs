using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace ChristianMoser.WpfInspector.Utilities
{
    public static class VisualTreeHelperExtensions
    {
        public static T FindAncestor<T>(this DependencyObject dependencyObject)
            where T : class
        {
            DependencyObject target = dependencyObject;
            do
            {
                target = VisualTreeHelper.GetParent(target);
            }
            while (target != null && !(target is T));
            return target as T;
        }
    }

}
