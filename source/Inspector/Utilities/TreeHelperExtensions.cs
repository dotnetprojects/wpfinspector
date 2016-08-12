using System.Windows;
using System.Windows.Media;

namespace ChristianMoser.WpfInspector.Utilities
{
    public static class TreeHelperExtensions
    {
        public static T FindVisualAncestor<T>(this DependencyObject dependencyObject)
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

        public static T FindLogicalAncestor<T>(this DependencyObject dependencyObject)
            where T : class
        {
            DependencyObject target = dependencyObject;
            do
            {
                target = LogicalTreeHelper.GetParent(target);
            }
            while (target != null && !(target is T));
            return target as T;
        }
    }

}
