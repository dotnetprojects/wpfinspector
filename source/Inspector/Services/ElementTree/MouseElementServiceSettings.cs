using System.ComponentModel;
using ChristianMoser.WpfInspector.Utilities;
using System;

namespace ChristianMoser.WpfInspector.Services.ElementTree
{
    /// <summary>
    /// Settings for the mouse element service
    /// </summary>
    public class MouseElementServiceSettings : INotifyPropertyChanged
    {
        #region Private Members

        private bool _isShowAdorner = true;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets if the adorner is shown
        /// </summary>
        public bool IsShowAdorner
        {
            get { return _isShowAdorner; }
            set
            {
                PropertyChanged.ChangeAndNotify(ref _isShowAdorner, value, this, "IsShowAdorner");
                ShowAdornerChanged.Notify(this, EventArgs.Empty);
            }
        }

        #endregion

        /// <summary>
        /// Occurs when the show adorner property changed.
        /// </summary>
        public event EventHandler ShowAdornerChanged;

        #region Implementation of INotifyPropertyChanged

        /// <summary>
        /// See <see cref="INotifyPropertyChanged.PropertyChanged" />
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
