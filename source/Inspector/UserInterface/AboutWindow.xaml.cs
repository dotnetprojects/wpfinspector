using System.Windows;

namespace ChristianMoser.WpfInspector.UserInterface
{
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : Window
    {
        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="AboutWindow"/> class.
        /// </summary>
        public AboutWindow()
        {
            InitializeComponent();
            closeButton.Click += (s, e) => Close();
        }

        #endregion
    }
}
