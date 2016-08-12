using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using ChristianMoser.WpfInspector.Services.ElementTree;

namespace ChristianMoser.WpfInspector.UserInterface
{
    public static class ListBoxItemHelper
    {
        public static readonly DependencyProperty BringIntoViewProperty = DependencyProperty.RegisterAttached("BringIntoView",
                        typeof(bool), typeof(ListBoxItemHelper),
                        new FrameworkPropertyMetadata(false,OnBringIntoViewChanged));

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
            var listBox = sender as ListBox;
            if( listBox != null)
            {
                listBox.SelectionChanged += ListBoxSelectionChanged;
            }
        }

        static void ListBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listBox = sender as ListBox;
            if( listBox != null && e.AddedItems != null && e.AddedItems.Count>0)
            {
                var item = e.AddedItems[0];
                // Scroll immediately if possible             
                if (!listBox.TryScrollToCenterOfView(item))       
                {                
                    // Otherwise wait until everything is loaded, then scroll                 
                    listBox.ScrollIntoView(item);
                    listBox.Dispatcher.BeginInvoke(new Action(() => listBox.TryScrollToCenterOfView(item)));   
                }
            }
        }

        public static readonly DependencyProperty ExpandOnClickProperty = DependencyProperty.RegisterAttached("ExpandOnClick",
                        typeof(bool), typeof(ListBoxItemHelper),
                        new FrameworkPropertyMetadata(false, OnExpandOnClickChanged));

        public static void SetExpandOnClick(UIElement element, bool value)
        {
            element.SetValue(ExpandOnClickProperty, value);
        }

        public static bool GetExpandOnClick(UIElement element)
        {
            return (bool)element.GetValue(ExpandOnClickProperty);
        }

        public static void OnExpandOnClickChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var listBoxItem = sender as ListBoxItem;
            if ((bool)e.NewValue && listBoxItem != null)
            {
                listBoxItem.PreviewMouseLeftButtonDown += (s, a) =>
                                                           {
                                                               var treeItem = listBoxItem.DataContext as TreeItem;
                                                               if (!treeItem.IsExpanded && treeItem.Children.Count > 0)
                                                               {
                                                                   treeItem.IsExpanded = true;
                                                                   listBoxItem.IsSelected = true;
                                                                   a.Handled = true;
                                                               }
                                                           };
            }
        }
 
        private static bool TryScrollToCenterOfView(this ItemsControl itemsControl, object item) 
        {            
            // Find the container             
            var container = itemsControl.ItemContainerGenerator.ContainerFromItem(item) as UIElement;  
            if (container == null) return false;            
            // Find the ScrollContentPresenter            
            ScrollContentPresenter presenter = null;        
            for (UIElement vis = container; vis != null ; vis = VisualTreeHelper.GetParent(vis) as UIElement) 
                if ((presenter = vis as ScrollContentPresenter) != null)   
                    break; 
            if (presenter == null) 
                return false;        
            // Find the IScrollInfo       
            var scrollInfo =                
                !presenter.CanVerticallyScroll ? presenter :     
                presenter.Content as IScrollInfo ??         
                FirstVisualChild(presenter.Content as ItemsPresenter) as IScrollInfo ??        
                presenter;            
            // Compute the center point of the container relative to the scrollInfo
                 
            if( VisualTreeHelper.GetChildrenCount(container) > 0 )
            {
                container = VisualTreeHelper.GetChild(container, 0) as UIElement;
            }
            if (container != null && VisualTreeHelper.GetChildrenCount(container) > 0)
            {
                container = VisualTreeHelper.GetChild(container, 0) as UIElement;
            }
            if (container != null && VisualTreeHelper.GetChildrenCount(container) > 0)
            {
                container = VisualTreeHelper.GetChild(container, 0) as UIElement;
            }
            if( container == null)
            {
                return false;
            }
            Size size = container.RenderSize;     
            Point center = container.TransformToVisual((UIElement)scrollInfo).Transform(new Point(size.Width / 2, size.Height / 2)); 
            center.Y += scrollInfo.VerticalOffset;           
            center.X += scrollInfo.HorizontalOffset;           
            // Adjust for logical scrolling            
            if (scrollInfo is StackPanel || scrollInfo is VirtualizingStackPanel)
            {
                double logicalCenter = itemsControl.ItemContainerGenerator.IndexFromContainer(container) + 0.5;   
                Orientation orientation = scrollInfo is StackPanel ? ((StackPanel)scrollInfo).Orientation : ((VirtualizingStackPanel)scrollInfo).Orientation;    
                if (orientation == Orientation.Horizontal)                 
                    center.X = logicalCenter;          
                else         
                    center.Y = logicalCenter;
            }           
            // Scroll the center of the container to the center of the viewport         
            if (scrollInfo.CanVerticallyScroll) scrollInfo.SetVerticalOffset(CenteringOffset(center.Y, scrollInfo.ViewportHeight, scrollInfo.ExtentHeight));  
            if (scrollInfo.CanHorizontallyScroll) scrollInfo.SetHorizontalOffset(CenteringOffset(center.X, scrollInfo.ViewportWidth, scrollInfo.ExtentWidth));       
            return true;       
        }      
        
        private static double CenteringOffset(double center, double viewport, double extent)
        {
            return Math.Min(extent - viewport, Math.Max(0, center - viewport / 2));
        }      
        
        private static DependencyObject FirstVisualChild(UIElement visual)
        {
            if (visual == null) return null;       
            if (VisualTreeHelper.GetChildrenCount(visual) == 0) return null;        
            return VisualTreeHelper.GetChild(visual, 0);
        }   
     
       
    }
}
