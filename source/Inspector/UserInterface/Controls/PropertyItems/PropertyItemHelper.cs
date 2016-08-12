using System.Windows;
using System.Windows.Controls;

namespace ChristianMoser.WpfInspector.UserInterface.Controls.PropertyItems
{
    public static class PropertyItemHelper
    {
        public static readonly DependencyProperty BringIntoViewProperty = DependencyProperty.RegisterAttached("BringIntoView",
                       typeof(bool), typeof(PropertyItemHelper),
                       new FrameworkPropertyMetadata(false, OnBringIntoViewChanged));

        public static void SetBringIntoView(UIElement element, bool value)
        {
            element.SetValue(BringIntoViewProperty, value);
        }

        public static bool GetBringIntoView(UIElement element)
        {
            return (bool)element.GetValue(BringIntoViewProperty);
        }

        public static void OnBringIntoViewChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var contentPresenter = sender as ContentPresenter;
            if ((bool)e.NewValue && contentPresenter != null)
            {
                contentPresenter.BringIntoView();
            }
        }
    }
}
