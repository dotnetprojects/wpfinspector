using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.ComponentModel;
using ChristianMoser.WpfInspector.Utilities;
using System.Globalization;
using System.Windows.Controls.Primitives;

namespace ChristianMoser.WpfInspector.UserInterface.Controls
{
    
    [TemplatePart(Name = "PART_MixerArea", Type = typeof(Canvas))]
    [TemplatePart(Name = "PART_MixerHandle", Type = typeof(FrameworkElement))]
    [TemplatePart(Name = "PART_ColorBar", Type = typeof(Canvas))]
    [TemplatePart(Name = "PART_ColorSelector", Type = typeof(FrameworkElement))]    
    public class ColorPicker : Control, INotifyPropertyChanged
    {
        #region Private Members

        private int _a, _r, _g, _b;
        private string _hex;
        private bool _isUpdating;

        private FrameworkElement _mixerHandle;
        private Canvas _mixerArea;
        private FrameworkElement _colorSelector;
        private Canvas _colorBar;

        private Color _mixerColor = Colors.Green;

        #endregion

        public event EventHandler<EventArgs<Color>> ColorChanged;

        /// <summary>
        /// Gets or sets the alpha component of the color
        /// </summary>
        /// <value>The A.</value>
        public int A
        {
            get { return _a; }
            set
            {
                PropertyChanged.ChangeAndNotify(ref _a, value, this, "A");
                UpdateColor();
            }
        }

        /// <summary>
        /// Gets or sets the red component of the color
        /// </summary>
        /// <value>The R.</value>
        public int R
        {
            get { return _r; }
            set
            {
                PropertyChanged.ChangeAndNotify(ref _r, value, this, "R");
                UpdateColor();
            }
        }

        /// <summary>
        /// Gets or sets the green component of the color
        /// </summary>
        /// <value>The G.</value>
        public int G
        {
            get { return _g; }
            set
            {
                PropertyChanged.ChangeAndNotify(ref _g, value, this, "G");
                UpdateColor();
            }
        }

        /// <summary>
        /// Gets or sets the blue component of the color
        /// </summary>
        /// <value>The B.</value>
        public int B
        {
            get { return _b; }
            set
            {
                PropertyChanged.ChangeAndNotify(ref _b, value, this, "B");
                UpdateColor();
            }
        }

        /// <summary>
        /// Gets or sets the hex value of the color
        /// </summary>
        /// <value>The B.</value>
        public string Hex
        {
            get { return _hex; }
            set
            {
                PropertyChanged.ChangeAndNotify(ref _hex, value, this, "Hex");
                UpdateColorFromHex();
            }
        }

        /// <summary>
        /// Gets the brush.
        /// </summary>
        public Brush Brush
        {
            get { return new SolidColorBrush(Color); }
        }

        #region Color

        /// <summary>
        /// Gets or sets the color.
        /// </summary>
        /// <value>The color.</value>
        public Color Color
        {
            get { return (Color)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }

        public Color MixerColor
        {
            get { return _mixerColor; }
            set
            {
                PropertyChanged.ChangeAndNotify(ref _mixerColor, value, this, "MixerColor");
                UpdateColorFromMixer();
            }
        }

        /// <summary>
        /// Registers a dependency property to get or set the color
        /// </summary>
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Color), typeof(ColorPicker),
            new FrameworkPropertyMetadata(Colors.White, OnColorChanged));

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Overrides

        public override void OnApplyTemplate()
        {
            if (Template != null)
            {
                _mixerArea = Template.FindName("PART_MixerArea", this) as Canvas;
                _mixerHandle = Template.FindName("PART_MixerHandle", this) as FrameworkElement;

                if (_mixerArea != null)
                {
                    _mixerArea.MouseDown += OnMixerAreaMouseDown;
                    _mixerArea.MouseUp+= OnMixerAreaMouseUp;
                    _mixerArea.MouseMove += OnMixerAreaMouseMove;
                }

                _colorBar = Template.FindName("PART_ColorBar", this) as Canvas;
                _colorSelector = Template.FindName("PART_ColorSelector", this) as FrameworkElement;

                if( _colorBar != null)
                {
                    _colorBar.MouseDown += OnColorBarMouseDown;
                    _colorBar.MouseUp += OnColorBarMouseUp;
                    _colorBar.MouseMove += OnColorBarMouseMove;
                }
            }
            base.OnApplyTemplate();
        }

        private void OnColorBarMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                UpdateColorSelectorPosition(e.GetPosition(_colorBar));
                UpdateMixerColorFromBar();
            }
        }

        private static void OnColorBarMouseUp(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(null);
        }

        private void OnColorBarMouseDown(object sender, MouseButtonEventArgs e)
        {
            UpdateColorSelectorPosition(e.GetPosition(_colorBar));
            UpdateMixerColorFromBar();
            Mouse.Capture(_colorBar);
        }

        private static void OnMixerAreaMouseUp(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(null);
        }

        private void OnMixerAreaMouseMove(object sender, MouseEventArgs e)
        {
            if( e.LeftButton == MouseButtonState.Pressed )
            {
                UpdateHandlePosition(e.GetPosition(_mixerArea));
                UpdateColorFromMixer();   
            }
        }
       

        private void OnMixerAreaMouseDown(object sender, MouseButtonEventArgs e)
        {
            UpdateHandlePosition(e.GetPosition(_mixerArea));
            UpdateColorFromMixer();
            Mouse.Capture(_mixerArea);
        }

        #endregion

        #region Private Helpers

        private void UpdateColorSelectorPosition(Point offset)
        {
            if (offset.Y <= 0)
                offset.Y = 0;

            if (offset.Y > _colorBar.ActualHeight)
                offset.Y = _colorBar.ActualHeight;
            
            offset.Y -= _colorSelector.ActualHeight / 2;
            Canvas.SetTop(_colorSelector, offset.Y);            
        }

        private void UpdateHandlePosition(Point offset)
        {
            if (offset.X <= 0)
                offset.X = 0;

            if (offset.X > _mixerArea.ActualWidth)
                offset.X = _mixerArea.ActualWidth;

            if (offset.Y <= 0)
                offset.Y = 0;

            if (offset.Y > _mixerArea.ActualHeight)
                offset.Y = _mixerArea.ActualHeight;

            offset.X -= _mixerHandle.ActualWidth / 2;
            offset.Y -= _mixerHandle.ActualHeight / 2;

            Canvas.SetLeft(_mixerHandle, offset.X);
            Canvas.SetTop(_mixerHandle, offset.Y);            
        }

        private void UpdateMixerColorFromBar()
        {
            double position = _colorSelector.TranslatePoint(new Point(_colorSelector.ActualWidth/2, _colorSelector.ActualHeight/2),_colorBar).Y;
            int colorIndex = (int) ((1/_colorBar.ActualHeight*position)*1536);

            var r = (byte)GetValue(colorIndex);
            var g = (byte)GetValue(colorIndex + 512);
            var b = (byte)GetValue(colorIndex + 1024);

            MixerColor = Color.FromArgb(0xFF, r, g, b);
        }

        private static int GetValue(int position)
        {
            position = position%1536;

            if (position >= 0 && position < 256)
                return 255;
            if (position >= 256 && position < 512)
                return 256 - (position - 255);
            if (position >= 512 && position < 1024)
                return 0;
            if (position >= 1024 && position < 1280)
                return position - 1024;
            if (position > 1280)
                return 255;
            return 0;
        }

        private void UpdateColorFromMixer()
        {
            var offset = _mixerHandle.TranslatePoint(new Point(_mixerHandle.ActualWidth / 2, _mixerHandle.ActualHeight / 2), _mixerArea);
            double saturation = 1 - (1/_mixerArea.ActualWidth*offset.X);
            double brightness = 1 - (1/_mixerArea.ActualHeight*offset.Y);

            double r = (MixerColor.R + (255 - MixerColor.R) * saturation) * brightness;
            double g = (MixerColor.G + (255 - MixerColor.G) * saturation) * brightness;
            double b = (MixerColor.B + (255 - MixerColor.B)*saturation)*brightness;

            Color = Color.FromArgb(0xFF,(byte) r, (byte) g, (byte) b);
            NotifyColorChangedByUser();
        }

        private void UpdateColor()
        {
            _isUpdating = true;
            Color = Color.FromArgb((byte)A, (byte)R, (byte)G, (byte)B);
            NotifyColorChangedByUser();
            _isUpdating = false;
        }

        private void UpdateColorFromHex()
        {
            _isUpdating = true;
            try
            {
                Color = (Color)ColorConverter.ConvertFromString(_hex);
            }
            catch (Exception)
            {
            }
            _isUpdating = false;
        }

        private static void OnColorChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var colorPicker = sender as ColorPicker;
            if (colorPicker != null)
            {
                if (colorPicker._isUpdating)
                    return;

                if (e.NewValue == null)
                {
                    return;
                }

                var color = (Color)e.NewValue;
                colorPicker.A = color.A;
                colorPicker.R = color.R;
                colorPicker.G = color.G;
                colorPicker.B = color.B;
                colorPicker.Hex = color.ToString(CultureInfo.InvariantCulture);

                colorPicker.PropertyChanged.Notify(colorPicker, "Brush");
            }
        }

        private void NotifyColorChangedByUser()
        {
            if( ColorChanged != null)
            {
                ColorChanged(this, new EventArgs<Color> {Data = Color});
            }
        }

        #endregion
    }
}
