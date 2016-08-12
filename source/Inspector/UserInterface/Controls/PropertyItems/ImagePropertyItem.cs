using System;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using ChristianMoser.WpfInspector.Utilities;
using Microsoft.Win32;

namespace ChristianMoser.WpfInspector.UserInterface.Controls.PropertyItems
{
    public class ImagePropertyItem : PropertyItem
    {
        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="ImagePropertyItem"/> class.
        /// </summary>
        public ImagePropertyItem(PropertyDescriptor property, object instance)
            : base( property, instance)
        {
            BrowseCommand = new Command<object>(Browse, o => IsEditable);
        }

        #endregion

        protected override void OnValueChanged(object sender, EventArgs e)
        {
            NotifyPropertyChanged("Source");
            base.OnValueChanged(sender, e);
        }

        public ICommand BrowseCommand { get; set; }

        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        public object Source 
        {
            get
            {
                var frame = Value as BitmapFrame;
                if( frame != null)
                {
                    return frame.BaseUri;
                }
                var imageSource = Value as BitmapImage;
                if( imageSource != null)
                {
                    return imageSource.UriSource;
                }
                return null;
            }
            set
            {
                if (value is string)
                {
                    var image = new BitmapImage(); 
                    image.BeginInit();
                    image.UriSource = new Uri(value.ToString(), UriKind.Absolute); 
                    image.EndInit();
                    Value = image;
                }
            }
        }

        private void Browse(object obj)
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files (*.bmp, *.jpg, *.png, *.gif)|*.bmp;*.jpg;*.png;*.gif|All Files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                Source = openFileDialog.FileName;
            }
        }

    }
}
