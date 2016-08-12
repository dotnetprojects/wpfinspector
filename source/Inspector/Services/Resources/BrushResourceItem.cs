using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.ComponentModel;
using System.Windows.Data;

namespace ChristianMoser.WpfInspector.Services.Resources
{
    public class BrushStop
    {
        public Color Color { get; private set; }
        public double Offset { get; private set; }

        public string ColorText { get; private set; }

        public BrushStop(Color color, double offset)
        {
            Color = color;
            Offset = offset;
            ColorText = Color.ToString();
        }
    }

    public class BrushResourceItem : ResourceItem
    {
        private readonly Brush _brush;
        private readonly List<BrushStop> _stops = new List<BrushStop>();

        public BrushResourceItem(object resourceKey, ResourceDictionary dictionary, FrameworkElement source, ResourceScope scope)
            :base( resourceKey, dictionary,source, scope)
        {
            _brush = dictionary[resourceKey] as Brush;
            BrushStops = new ListCollectionView(_stops);

            BuildStops();
        }

        private void BuildStops()
        {
            if( _brush == null)
            {
                return;
            }

            var solidColorBrush = _brush as SolidColorBrush;
            if (solidColorBrush != null)
            {
                _stops.Add(new BrushStop(solidColorBrush.Color,-1));
            }

            var linearBrush = _brush as LinearGradientBrush;
            if( linearBrush != null)
            {
                foreach (var gradientStop in linearBrush.GradientStops)
                {
                    _stops.Add(new BrushStop(gradientStop.Color, gradientStop.Offset));
                }
            }

            var radialBrush = _brush as RadialGradientBrush;
            if (radialBrush != null)
            {
                foreach (var gradientStop in radialBrush.GradientStops)
                {
                    _stops.Add(new BrushStop(gradientStop.Color, gradientStop.Offset));
                }
            }

            BrushStops.Refresh();
        }

        public ICollectionView BrushStops { get; private set; }

        public override string Category
        {
            get
            {
                return "Brushes";
            }
        }
    }
}
