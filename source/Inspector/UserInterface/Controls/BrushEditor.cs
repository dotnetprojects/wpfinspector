using System;
using System.Windows;
using System.Windows.Controls;
using ChristianMoser.WpfInspector.UserInterface.Controls.PropertyItems;
using System.Windows.Media;

namespace ChristianMoser.WpfInspector.UserInterface.Controls
{
    public class BrushEditor : Control
    {
        private TabControl _tabControl;
        private ColorPicker _solidColorPicker;
        private BrushPropertyItem _brushPropertyItem;
        /// <summary>
        /// Gets or sets the brush item.
        /// </summary>
        /// <value>The brush item.</value>
        public BrushPropertyItem BrushItem
        {
            get { return (BrushPropertyItem)GetValue(BrushItemProperty); }
            set { SetValue(BrushItemProperty, value); }
        }

        /// <summary>
        /// Registers a dependency property to get or set the brush item
        /// </summary>
        public static readonly DependencyProperty BrushItemProperty =
            DependencyProperty.Register("BrushItem", typeof(BrushPropertyItem), typeof(BrushEditor),
            new FrameworkPropertyMetadata(null, OnBrushItemChanged));

        public override void OnApplyTemplate()
        {
            if (Template != null)
            {
                _solidColorPicker = Template.FindName("PART_SolidColorPicker", this) as ColorPicker;
                _solidColorPicker.ColorChanged += (s, e) => UpdateModel();
                _tabControl = Template.FindName("PART_BrushSelector", this) as TabControl;
                if (_tabControl != null)
                {
                    _tabControl.SelectionChanged += (s, e) => UpdateModel(); 
                }
            }
            UpdateView();

            base.OnApplyTemplate();
        }

        private static void OnBrushItemChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var brushEditor = sender as BrushEditor;
            if (brushEditor != null)
            {
                brushEditor._brushPropertyItem = e.NewValue as BrushPropertyItem;
                brushEditor.UpdateView();
            }
        }

        private void UpdateView()
        {
            Brush brush = null;
            if (BrushItem != null)
            {
                brush = BrushItem.Property.GetValue(BrushItem.Instance) as Brush;
            }

            if (_tabControl != null)
            {
                if (brush is SolidColorBrush)
                {
                    _tabControl.SelectedIndex = 0;
                    _solidColorPicker.Color = ((SolidColorBrush) brush).Color;
                }
                if (brush is LinearGradientBrush)
                {
                    _tabControl.SelectedIndex = 1;
                }
                if (brush is RadialGradientBrush)
                {
                    _tabControl.SelectedIndex = 2;
                }
            }
        }

        private void UpdateModel()
        {
            Brush brush = null;
            switch (_tabControl.SelectedIndex)
            {
                case 0:
                    // SolidColorBrush
                    brush = new SolidColorBrush(_solidColorPicker.Color);

                    if (_brushPropertyItem != null)
                    {
                        _brushPropertyItem.Value = brush;
                    }
                    break;
                case 1:
                    // LinearGradientBrush
                    break;
                case 2:
                    // RadialGradientBrush
                    break;
                case 3:
                    // Resource
                    break;
            }

            
        }

        
    }
}
