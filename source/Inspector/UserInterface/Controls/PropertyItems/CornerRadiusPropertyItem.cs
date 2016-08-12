using System;
using System.ComponentModel;
using System.Windows;

namespace ChristianMoser.WpfInspector.UserInterface.Controls.PropertyItems
{
    public class CornerRadiusPropertyItem : PropertyItem
    {
        #region Private Members

        private double _topLeft, _topRight, _bottomRight, _bottomLeft;
        private bool _disableUpdate;

        #endregion

        #region Construction

        public CornerRadiusPropertyItem(PropertyDescriptor property, object instance)
            : base( property, instance)
        {
            LoadCornerRadius();
        }

        #endregion

        public double TopLeft
        {
            get { return _topLeft; }
            set
            {
                _topLeft = value;
                NotifyPropertyChanged("TopLeft");
                UpdateCornerRadius();
            }
        }

        public double TopRight
        {
            get { return _topRight; }
            set
            {
                _topRight = value;
                NotifyPropertyChanged("TopRight");
                UpdateCornerRadius();
            }
        }

        public double BottomRight
        {
            get { return _bottomRight; }
            set
            {
                _bottomRight = value;
                NotifyPropertyChanged("BottomRight");
                UpdateCornerRadius();
            }
        }

        public double BottomLeft
        {
            get { return _bottomLeft; }
            set
            {
                _bottomLeft = value;
                NotifyPropertyChanged("BottomLeft");
                UpdateCornerRadius();
            }
        }

        protected override void OnValueChanged(object sender, EventArgs e)
        {
            if (_disableUpdate)
                return;

            LoadCornerRadius();
            base.OnValueChanged(sender, e);
        }

        private void LoadCornerRadius()
        {
            if( Value == null)
            {
                TopLeft = 0;
                BottomRight = 0;
                TopRight = 0;
                BottomLeft = 0;
            }
            else
            {
                var thicknes = (CornerRadius)Value;
                TopLeft = thicknes.TopLeft;
                BottomRight = thicknes.TopRight;
                TopRight = thicknes.BottomRight;
                BottomLeft = thicknes.BottomLeft;
            }
        }

        private void UpdateCornerRadius()
        {
            _disableUpdate = true;
            Property.SetValue(Instance,new CornerRadius(TopLeft, BottomRight, TopRight, BottomLeft));
            _disableUpdate = false;
        }
    }
}
