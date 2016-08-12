using System;
using System.ComponentModel;
using System.Windows;

namespace ChristianMoser.WpfInspector.UserInterface.Controls.PropertyItems
{
    public class ThicknessPropertyItem : PropertyItem
    {
        #region Private Members

        private double _left, _right, _top, _bottom;
        private bool _disableUpdate;

        #endregion

        #region Construction

        public ThicknessPropertyItem(PropertyDescriptor property, object instance)
            : base( property, instance)
        {
            LoadThickness();
        }

        #endregion

        public double Left
        {
            get { return _left; }
            set
            {
                _left = value;
                NotifyPropertyChanged("Left");
                UpdateThickness();
            }
        }

        public double Right
        {
            get { return _right; }
            set
            {
                _right = value;
                NotifyPropertyChanged("Right");
                UpdateThickness();
            }
        }

        public double Top
        {
            get { return _top; }
            set
            {
                _top = value;
                NotifyPropertyChanged("Top");
                UpdateThickness();
            }
        }

        public double Bottom
        {
            get { return _bottom; }
            set
            {
                _bottom = value;
                NotifyPropertyChanged("Bottom");
                UpdateThickness();
            }
        }

        protected override void OnValueChanged(object sender, EventArgs e)
        {
            if (_disableUpdate)
                return;

            LoadThickness();
            base.OnValueChanged(sender, e);
        }

        private void LoadThickness()
        {
            if( Value == null)
            {
                Left = 0;
                Top = 0;
                Right = 0;
                Bottom = 0;
            }
            else
            {
                var thicknes = (Thickness)Value;
                Left = thicknes.Left;
                Top = thicknes.Top;
                Right = thicknes.Right;
                Bottom = thicknes.Bottom;
            }
        }

        private void UpdateThickness()
        {
            _disableUpdate = true;
            Property.SetValue(Instance,new Thickness(Left, Top, Right, Bottom));
            _disableUpdate = false;
        }
    }
}
