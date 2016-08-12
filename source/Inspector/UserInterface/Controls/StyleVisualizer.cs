using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Imaging;

namespace ChristianMoser.WpfInspector.UserInterface.Controls
{
    public class StyleVisualizer : Control
    {

        public StyleVisualizer()
        {

        }

        /// <summary>
        /// Gets or sets the style to visualize.
        /// </summary>
        public Style StyleToVisualize
        {
            get { return (Style)GetValue(StyleToVisualizeProperty); }
            set { SetValue(StyleToVisualizeProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for StyleToVisualize.
        /// </summary>
        public static readonly DependencyProperty StyleToVisualizeProperty =
            DependencyProperty.Register("StyleToVisualize", typeof(Style), typeof(StyleVisualizer),
            new FrameworkPropertyMetadata(null, OnStyleToVisualizeChanged));

        private static void OnStyleToVisualizeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var style = e.NewValue as Style;
            var visualizer = sender as StyleVisualizer;
            if (style != null && visualizer != null)
            {
                var type = style.TargetType;
                if (type != null)
                {
                    try
                    {
                        if( type.IsAbstract || type.IsInterface || type == typeof(ContextMenu) )
                        {
                            return;
                        }

                        var element = Activator.CreateInstance(type) as Control;
                        if (element != null)
                        {
                            element.Style = style;
                            element.IsHitTestVisible = false;
                            element.VerticalAlignment = VerticalAlignment.Stretch;
                            element.HorizontalAlignment = HorizontalAlignment.Stretch;
                            element.Measure(new Size(200,100));
                            element.Arrange(new Rect(0, 0, element.DesiredSize.Width, element.DesiredSize.Height));
                            if( element.DesiredSize.Width > 0 && element.DesiredSize.Height > 0)
                            {
                                var rtBitmap = new RenderTargetBitmap((int)element.DesiredSize.Width, (int)element.DesiredSize.Height, 96, 96, PixelFormats.Default);
                                rtBitmap.Render(element);
                                rtBitmap.Freeze();
                                visualizer.StyledElement = rtBitmap;
                                visualizer.ElementHeight = element.DesiredSize.Height;
                                visualizer.ElementWidth = element.DesiredSize.Width;
                            }
                        }
                    }
                    catch
                    { }

                }
            }
        }



        public double ElementWidth
        {
            get { return (double)GetValue(ElementWidthProperty); }
            set { SetValue(ElementWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ElementWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ElementWidthProperty =
            DependencyProperty.Register("ElementWidth", typeof(double), typeof(StyleVisualizer), new FrameworkPropertyMetadata(0D));


        public double ElementHeight
        {
            get { return (double)GetValue(ElementHeightProperty); }
            set { SetValue(ElementHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ElementHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ElementHeightProperty =
            DependencyProperty.Register("ElementHeight", typeof(double), typeof(StyleVisualizer), new FrameworkPropertyMetadata(0D));



        public ImageSource StyledElement
        {
            get { return (ImageSource)GetValue(StyledElementProperty); }
            set { SetValue(StyledElementProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StyledElement.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StyledElementProperty =
            DependencyProperty.Register("StyledElement", typeof(ImageSource), typeof(StyleVisualizer),
            new FrameworkPropertyMetadata(null));



    }
}
