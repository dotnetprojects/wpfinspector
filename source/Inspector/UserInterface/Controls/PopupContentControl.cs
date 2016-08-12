using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Threading;

namespace ChristianMoser.WpfInspector.UserInterface.Controls
{
    /// <summary>
    /// The popup is a control that displays a header.
    /// If the header is clicked, the content is shows within a popup
    /// </summary>
    [TemplatePart(Name = "PART_Header", Type = typeof(Border))]
    [TemplatePart(Name = "PART_Popup", Type = typeof(Popup))]
    [TemplatePart(Name = "PART_PopupBorder", Type = typeof(Control))]
    [ContentProperty("Content")]
    public class PopupContentControl : ComboBox, INotifyPropertyChanged
    {
        #region INotifyPropertyChanged Members

        /// <summary>
        /// See <see cref="INotifyPropertyChanged.PropertyChanged"/>.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Private Members

        private FrameworkElement _header;
        private Popup _popup;
        private Control _popupBorder;

        private readonly DispatcherTimer _delayedOpenTimer, _delayedCloseTimer, _stayOpenTimer;

        private bool _isDelayedMouseOver;
        private bool _isClosedByClick;

        private bool _isForcedToOpen, _isForcedToClose, _isForcedToStayOpen;

        #endregion

        #region Construction

        /// <summary>
        /// Initializes the <see cref="PopupContentControl"/> class.
        /// </summary>
        static PopupContentControl()
        {
            // Override the default value of the style property (that was typeof(combobox))  
            // to typeof(PopupControl).
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PopupContentControl),
                new FrameworkPropertyMetadata(typeof(PopupContentControl)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PopupContentControl"/> class.
        /// </summary>
        public PopupContentControl()
        {
            // Hook up a notification when the IsDropDownOpen dependency property changes
            DependencyPropertyDescriptor dpd = DependencyPropertyDescriptor.FromProperty(IsDropDownOpenProperty, typeof(ComboBox));
            dpd.AddValueChanged(this, OnIsDropDownOpenChanged);

            dpd = DependencyPropertyDescriptor.FromProperty(IsVisibleProperty, typeof(UIElement));
            dpd.AddValueChanged(this, OnIsVisibleChanged);

            // Initialize delayed open/close timers
            _delayedOpenTimer = new DispatcherTimer { Interval = OpenDelay };
            _delayedCloseTimer = new DispatcherTimer { Interval = CloseDelay };
            _stayOpenTimer = new DispatcherTimer();
            _delayedOpenTimer.Tick += (s, e) => { _delayedOpenTimer.Stop(); IsDelayedMouseOver = true; };
            _delayedCloseTimer.Tick += (s, e) => { _delayedCloseTimer.Stop(); IsDelayedMouseOver = false; };
            _stayOpenTimer.Tick += (s, e) => { _stayOpenTimer.Stop(); _isForcedToOpen = true; IsForcedToStayOpen = false; };
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Opens the popup.
        /// </summary>
        public void OpenPopup()
        {
            _isForcedToOpen = true;
            NotifyPropertyChanged("IsDropDownOpenResolved");
        }

        /// <summary>
        /// Open the popup and avoid closing during the specified time.
        /// After the duration the popup can be closed by any click.
        /// </summary>
        public void OpenPopup(double minOpenDurationMillis)
        {
            _stayOpenTimer.Interval = TimeSpan.FromMilliseconds(minOpenDurationMillis);
            IsForcedToStayOpen = true;
        }

        /// <summary>
        /// Close the popup.
        /// </summary>
        public void ClosePopup()
        {
            _isForcedToClose = true;
            // Close popup also during the forced open time
            IsForcedToStayOpen = false;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets if the final result, if the drop down is open
        /// </summary>
        public bool IsDropDownOpenResolved
        {
            get
            {
                if (IsForcedToStayOpen)
                {
                    return true;
                }
                if (_isForcedToClose)
                {
                    _isForcedToOpen = false;
                    return IsDropDownOpen = false;
                }
                if (_isForcedToOpen)
                {
                    _isForcedToClose = false;
                    return IsDropDownOpen = true;
                }
                if (IsOpeningOnMouseOver)
                {
                    return IsDropDownOpen = !IsClosedByClick && IsDelayedMouseOver;
                }
                return IsDropDownOpen;
            }
        }

        #endregion

        #region DependencyProperties

        #region DependencyProperty Header

        /// <summary>
        /// Registers a dependency property as backing store for the header property
        /// </summary>
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(object), typeof(PopupContentControl),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender |
                                                 FrameworkPropertyMetadataOptions.AffectsParentMeasure));

        /// <summary>
        /// Gets or sets the header.
        /// </summary>
        /// <value>The header.</value>
        [Description("Gets or sets the header")]
        public object Header
        {
            get { return GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        #endregion

        #region DependencyProperty Content

        /// <summary>
        /// Registers a dependency property as backing store for the Content property
        /// </summary>
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(object), typeof(PopupContentControl),
            new FrameworkPropertyMetadata(null,
                  FrameworkPropertyMetadataOptions.AffectsRender |
                 FrameworkPropertyMetadataOptions.AffectsParentMeasure));

        /// <summary>
        /// Gets or sets the Content.
        /// </summary>
        /// <value>The Content.</value>
        [Description("Gets or sets the Content"), Category("Common")]
        public object Content
        {
            get { return GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        #endregion

        #region DependencyProperty IsOpeningOnMouseOver

        /// <summary>
        /// Registers a dependency property that gets or sets if the popup opens
        /// when the mouse hovers over the control
        /// </summary>
        public static readonly DependencyProperty IsOpeningOnMouseOverProperty =
            DependencyProperty.Register("IsOpeningOnMouseOver", typeof(bool), typeof(PopupContentControl),
            new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Gets or sets if the popup opens when the mouse hovers the control
        /// </summary>
        public bool IsOpeningOnMouseOver
        {
            get { return (bool)GetValue(IsOpeningOnMouseOverProperty); }
            set { SetValue(IsOpeningOnMouseOverProperty, value); }
        }

        #endregion

        #region DependencyProperty OpenDelay

        /// <summary>
        /// Registers a dependency property to get or set the open delay
        /// </summary>
        public static readonly DependencyProperty OpenDelayProperty =
            DependencyProperty.Register("OpenDelay", typeof(TimeSpan), typeof(PopupContentControl),
            new FrameworkPropertyMetadata(TimeSpan.FromMilliseconds(400)));

        /// <summary>
        /// Gets or sets the open delay.
        /// </summary>
        /// <value>The open delay.</value>
        public TimeSpan OpenDelay
        {
            get { return (TimeSpan)GetValue(OpenDelayProperty); }
            set { SetValue(OpenDelayProperty, value); }
        }

        #endregion

        #region DependencyProperty CloseDelay

        /// <summary>
        /// Registers a dependency property to get or set the Close delay
        /// </summary>
        public static readonly DependencyProperty CloseDelayProperty =
            DependencyProperty.Register("CloseDelay", typeof(TimeSpan), typeof(PopupContentControl),
            new FrameworkPropertyMetadata(TimeSpan.FromMilliseconds(400)));

        /// <summary>
        /// Gets or sets the Close delay.
        /// </summary>
        /// <value>The Close delay.</value>
        public TimeSpan CloseDelay
        {
            get { return (TimeSpan)GetValue(CloseDelayProperty); }
            set { SetValue(CloseDelayProperty, value); }
        }

        #endregion


        #region DependencyProperty PopupWidth

        /// <summary>
        /// Gets or sets the popup width
        /// </summary>
        public double PopupWidth
        {
            get { return (double)GetValue(PopupWidthProperty); }
            set { SetValue(PopupWidthProperty, value); }
        }

        /// <summary>
        /// Registers a dependency property to get or set the width of the popup
        /// </summary>
        public static readonly DependencyProperty PopupWidthProperty =
            DependencyProperty.Register("PopupWidth", typeof(double), typeof(PopupContentControl),
            new FrameworkPropertyMetadata(268d));

        #endregion

        #region DependencyProperty CornerRadius

        /// <summary>
        /// Gets or sets the corner radius.
        /// </summary>
        /// <value>The corner radius.</value>
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        /// <summary>
        /// Registers a dependency property to get or set the corner radius
        /// </summary>
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(PopupContentControl),
            new FrameworkPropertyMetadata(new CornerRadius()));

        #endregion

        #endregion

        #region Overrides

        /// <summary>
        /// See <see cref="UIElement.OnMouseEnter"/>.
        /// </summary>
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);

            if (IsOpeningOnMouseOver)
            {
                _delayedOpenTimer.Start();
                _delayedCloseTimer.Stop();
            }
        }

        /// <summary>
        /// See <see cref="UIElement.OnMouseLeave"/>.
        /// </summary>
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);

            _delayedOpenTimer.Stop();
        }

        /// <summary>
        /// See <see cref="UIElement.OnMouseMove"/>.
        /// </summary>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (IsDropDownOpen)
            {
                if (_header != null && _popup != null
                    && !_header.IsMouseOver && !_popup.IsMouseOver)
                {
                    _delayedOpenTimer.Stop();
                    _delayedCloseTimer.Start();
                }
                else
                {
                    _delayedCloseTimer.Stop();
                }
            }
        }

        /// <summary>
        /// See <see cref="ComboBox.OnApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            // Find template parts
            _header = Template.FindName("PART_Header", this) as FrameworkElement;
            _popupBorder = Template.FindName("PART_PopupBorder", this) as Control;
            _popup = Template.FindName("PART_Popup", this) as Popup;

            _popup.Opened += PopupOpened;

            // Wire-up the header part
            if (_header != null)
            {
                _header.MouseDown += OnHeaderClicked;
            }

            base.OnApplyTemplate();
        }

        void PopupOpened(object sender, EventArgs e)
        {
            Dispatcher.CurrentDispatcher.BeginInvoke((Action)(() =>
                                                                  {
                                                                      var content = Content as FrameworkElement;
                                                                      if (content != null)
                                                                      {
                                                                          content.Focus();
                                                                          Keyboard.Focus(content);
                                                                      }
                                                                  }));
        }

        #endregion

        #region Private Helpers


        /// <summary>
        /// Callback when the IsDropDownOpen dependency property changes
        /// 
        /// Because we hijacked the <see cref="ComboBox"/> to steal its drop down open/close functionality we have to synchronize our
        /// intern states if the change events is fired by the <see cref="ComboBox"/>. This is much more complex than it seems to be:
        /// E.g. must the application focus change also be handled!
        /// </summary>
        private static void OnIsDropDownOpenChanged(object sender, EventArgs e)
        {
            var popupContentControl = sender as PopupContentControl;
            if (popupContentControl == null)
            {
                return;
            }

            if (!popupContentControl.IsDropDownOpen && popupContentControl._isForcedToClose)
            {
                popupContentControl._isForcedToClose = false;
            }

            if (!popupContentControl.IsDropDownOpen && popupContentControl._isForcedToOpen)
            {
                popupContentControl._isForcedToOpen = false;
            }

            if (!popupContentControl.IsDropDownOpen && popupContentControl.IsDelayedMouseOver)
            {
                popupContentControl.IsDelayedMouseOver = false;
            }

            if (!popupContentControl.IsDropDownOpen && popupContentControl.IsOpeningOnMouseOver)
            {
                popupContentControl.IsClosedByClick = false;
            }

            popupContentControl.NotifyPropertyChanged("IsDropDownOpenResolved");
        }

        /// <summary>
        /// Called when the header has been clicked.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void OnHeaderClicked(object sender, MouseButtonEventArgs e)
        {
            if (IsOpeningOnMouseOver)
            {
                if (IsDropDownOpen)
                {
                    IsClosedByClick = true;
                }
                else
                {
                    IsDelayedMouseOver = true;
                }
            }
            else
            {
                IsDropDownOpen = !IsDropDownOpen;
                NotifyPropertyChanged("IsDropDownOpenResolved");
            }
        }

        /// <summary>
        /// Notifies, that a property changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private static void OnIsVisibleChanged(object sender, EventArgs e)
        {
            var popupContentControl = sender as PopupContentControl;
            if (popupContentControl == null)
            {
                return;
            }

            // Force close popup if PopupContentControl gets invisible
            if (!popupContentControl.IsVisible && popupContentControl.IsForcedToStayOpen)
            {
                popupContentControl.ClosePopup();
            }
        }

        #endregion

        #region Private Properties

        /// <summary>
        /// Gets or sets if the popup is closed by click.
        /// This is only used in the auto popup mode
        /// </summary>
        private bool IsClosedByClick
        {
            get { return _isClosedByClick; }
            set
            {
                _isClosedByClick = value;
                NotifyPropertyChanged("IsDropDownOpenResolved");
            }
        }

        /// <summary>
        /// Gets or sets delayed, if the mouse is over the control
        /// </summary>
        private bool IsDelayedMouseOver
        {
            get { return _isDelayedMouseOver; }
            set
            {
                _isDelayedMouseOver = value;
                NotifyPropertyChanged("IsDropDownOpenResolved");
            }
        }

        private bool IsForcedToStayOpen
        {
            get { return _isForcedToStayOpen; }
            set
            {
                if (value)
                {
                    _stayOpenTimer.Start();
                }
                else
                {
                    _stayOpenTimer.Stop();
                }

                _isForcedToStayOpen = value;
                NotifyPropertyChanged("IsDropDownOpenResolved");
            }
        }

        #endregion
    }
}
