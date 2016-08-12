using System.Windows.Input;
using System;
using ChristianMoser.WpfInspector.Utilities;

namespace ChristianMoser.WpfInspector.Services.Resizing
{
    public class SizeItem
    {
        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="SizeItem"/> class.
        /// </summary>
        public SizeItem(double width, double height, Action<double,double> setSizeFunction)
        {
            Width = width;
            Height = height;
            SizeCommand = new Command<object>(o => setSizeFunction(Width, Height));
        }

        #endregion

        /// <summary>
        /// Gets the width.
        /// </summary>
        public double Width { get; private set; }

        /// <summary>
        /// Gets the height.
        /// </summary>
        public double Height { get; private set; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name
        {
            get
            {
                return string.Format("{0}x{1}", Width, Height);
            }
        }

        /// <summary>
        /// Gets or sets the command to set the size
        /// </summary>
        public ICommand SizeCommand { get; private set; }
    }
}
