using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.ComponentModel;
using ChristianMoser.WpfInspector.UserInterface.Controls.PropertyItems;
using ChristianMoser.WpfInspector.Utilities;
using ChristianMoser.WpfInspector.Services;
using System.Windows.Threading;

namespace ChristianMoser.WpfInspector.UserInterface.Controls
{
    
    public class PropertyGrid : Control, INotifyPropertyChanged
    {
        #region Private Members

        private readonly List<IPropertyGridItem> _properties = new List<IPropertyGridItem>();
        private readonly LoggerService _logger;
        private readonly DispatcherTimer _delayedUpdateTimer = new DispatcherTimer(DispatcherPriority.Background);
        private readonly DispatcherTimer _modificationUpdateTimer = new DispatcherTimer(DispatcherPriority.Background);
        private bool _isLoading;
        private string _filter;
        private readonly List<string> _filterChunks = new List<string>();
        private string _hashCode;
        private bool _isGrouping = true;
        private bool _isShowModifiedOnly;
        private bool _isShowMethods = true;
        private string _type;
        private string _fullType;

        #endregion

        #region Construction

        /// <summary>
        /// Initializes the <see cref="PropertyGrid"/> class.
        /// </summary>
        static PropertyGrid()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyGrid), 
                new FrameworkPropertyMetadata(typeof(PropertyGrid)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyGrid"/> class.
        /// </summary>
        public PropertyGrid()
        {
            _logger = ServiceLocator.Resolve<LoggerService>();

            var propertiesView = new ListCollectionView(_properties);
            propertiesView.Filter = FilterPredicate;
            propertiesView.CustomSort = new PropertyComparer();
            Properties = propertiesView;
            UpdateGrouping();

            _modificationUpdateTimer.Interval = TimeSpan.FromMilliseconds(200);
            _modificationUpdateTimer.Tick += (s, e) => Properties.Refresh();

            _delayedUpdateTimer.Interval = TimeSpan.FromMilliseconds(200);
            _delayedUpdateTimer.Tick += (s,e) => UpdateSelectedObject();
        }

        private void UpdateGrouping()
        {
            Properties.SortDescriptions.Clear();
            Properties.GroupDescriptions.Clear();
            Properties.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
            if (_isGrouping)
            {
                Properties.GroupDescriptions.Add(new PropertyGroupDescription("Category"));
                Properties.SortDescriptions.Add(new SortDescription("CategoryOrder", ListSortDirection.Ascending));
                Properties.SortDescriptions.Add(new SortDescription("PropertyOrder", ListSortDirection.Ascending));
            }
        }

        #endregion

        #region DependencyProperty SelectedElement

        /// <summary>
        /// Gets or sets the selected object.
        /// </summary>
        /// <value>The selected object.</value>
        public object SelectedObject
        {
            get { return GetValue(SelectedObjectProperty); }
            set { SetValue(SelectedObjectProperty, value); }
        }

        /// <summary>
        /// Registers a dependency property to get or set the selected object
        /// </summary>
        public static readonly DependencyProperty SelectedObjectProperty =
            DependencyProperty.Register("SelectedObject", typeof(object), typeof(PropertyGrid), 
            new FrameworkPropertyMetadata(null, OnSelectedObjectChanged));

        #endregion

        #region DependencyProperty SelectedProperty

        /// <summary>
        /// Gets or sets the filter string
        /// </summary>
        public string Filter
        {
            get { return _filter; }
            set
            {
                PropertyChanged.ChangeAndNotify(ref _filter, value, this, "Filter");
                if (!string.IsNullOrEmpty(_filter))
                {
                    _filterChunks.Clear();
                    _filterChunks.AddRange(_filter.ToLower().Split(new [] {',', ';', ' ', '+'}));
                }
                Properties.Refresh();
            }
        }

        /// <summary>
        /// Gets or sets if grouping is active
        /// </summary>
        public bool IsGrouping
        {
            get { return _isGrouping; }
            set
            {
                PropertyChanged.ChangeAndNotify(ref _isGrouping, value, this, "IsGrouping");
                UpdateGrouping();
            }
        }

        /// <summary>
        /// Gets or sets if only modified proeprties are shown
        /// </summary>
        public bool IsShowModifiedOnly
        {
            get { return _isShowModifiedOnly; }
            set
            {
                PropertyChanged.ChangeAndNotify(ref _isShowModifiedOnly, value, this, "IsShowModifiedOnly");
                if( value )
                {
                    _modificationUpdateTimer.Start();
                }
                else
                {
                    _modificationUpdateTimer.Stop();
                    Properties.Refresh();
                }
            }
        }

        public bool IsShowMethods
        {
            get { return _isShowMethods; }
            set
            {
                PropertyChanged.ChangeAndNotify(ref _isShowMethods, value, this, "IsShowMethods");
                Properties.Refresh();
            }
        }

        public bool IsLoading
        {
            get { return _isLoading; }
            set { PropertyChanged.ChangeAndNotify(ref _isLoading, value, this, "IsLoading"); }
        }

        public string Type
        {
            get { return _type; }
            set
            {
                PropertyChanged.ChangeAndNotify(ref _type, value, this, "Type");
            }
        }

        public string HashCode
        {
            get { return _hashCode; }
            set
            {
                PropertyChanged.ChangeAndNotify(ref _hashCode, value, this, "HashCode");
            }
        }

        public string FullType
        {
            get { return _fullType; }
            set
            {
                PropertyChanged.ChangeAndNotify(ref _fullType, value, this, "FullType");
            }
        }


        /// <summary>
        /// Gets or sets the selected Property.
        /// </summary>
        /// <value>The selected Property.</value>
        public string SelectedProperty
        {
            get { return (string)GetValue(SelectedPropertyProperty); }
            set { SetValue(SelectedPropertyProperty, value); }
        }

        /// <summary>
        /// Registers a dependency property to get or set the selected Property
        /// </summary>
        public static readonly DependencyProperty SelectedPropertyProperty =
            DependencyProperty.Register("SelectedProperty", typeof(string), typeof(PropertyGrid),
            new FrameworkPropertyMetadata(null, OnSelectedPropertyChanged));

        #endregion

        #region DependencyProperty IsCategorized

        /// <summary>
        /// Gets or sets if the property grid is categorized
        /// </summary>
        public bool IsCategorized
        {
            get { return (bool)GetValue(IsCategorizedProperty); }
            set { SetValue(IsCategorizedProperty, value); }
        }

        /// <summary>
        /// Registers a dependency property to get or set if the property grid is categorized
        /// </summary>
        public static readonly DependencyProperty IsCategorizedProperty =
            DependencyProperty.Register("IsCategorized", typeof(bool), typeof(PropertyGrid), 
            new FrameworkPropertyMetadata(true));



        #endregion

        #region Public Functionality

        public ICollectionView Properties { get; private set; }

        #endregion

        #region Private Helpers

        private static void OnSelectedObjectChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var propertyGrid = sender as PropertyGrid;
            if (propertyGrid != null)
            {
                propertyGrid._delayedUpdateTimer.Stop();
                propertyGrid._delayedUpdateTimer.Start();
            }
        }

        private void UpdateSelectedObject()
        {
            IsLoading = true;
            _delayedUpdateTimer.Stop();
            _properties.Clear();
            Dispatcher.BeginInvoke((Action) (() => BuildPropertyList(SelectedObject)), DispatcherPriority.Background);

            if (SelectedObject != null)
            {
                Type = SelectedObject.GetType().Name;
                HashCode = SelectedObject.GetHashCode().ToString();
                FullType = SelectedObject.GetType().FullName;
            }
            else
            {
                Type = "null";
                HashCode = string.Empty;
            }
        }

        private static void OnSelectedPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var propGrid = (PropertyGrid)sender;
            propGrid.UpdateSelectedProperty();
        }

        private void UpdateSelectedProperty()
        {
            foreach (var propertyItem in _properties)
            {
                propertyItem.IsSelected = propertyItem.Name == SelectedProperty;
                Properties.MoveCurrentTo(propertyItem);
            }
        }

        private bool FilterPredicate(object obj)
        {
            var gridItem = obj as IPropertyGridItem;
            if (gridItem != null)
            {
                if (!string.IsNullOrEmpty(_filter))
                {
                    var lowerProp = gridItem.Name.ToLower();
                    return _filterChunks.Any(lowerProp.Contains);
                }

                if (IsShowModifiedOnly)
                {
                    var propertyItem = obj as PropertyItem;
                    return propertyItem != null && propertyItem.IsChanged;
                }
                
                return true;
            }
            
            return false;
        }

        private void BuildPropertyList(object instance)
        {
            try
            {
                foreach (var propertyItem in _properties)
                {
                    propertyItem.Dispose();
                }

                _properties.AddRange(PropertyGridItemFactory.GetPropertyItems(instance));
                _properties.AddRange(PropertyGridItemFactory.GetFieldItems(instance));
                _properties.AddRange(PropertyGridItemFactory.GetMethodItems(instance));
                Properties.Refresh();
                UpdateSelectedProperty();
            }
            catch (Exception ex)
            {
                _logger.Log(LogSeverity.Error, ex);
            }
            finally
            {
                IsLoading = false;
            }
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        private class PropertyComparer : IComparer
        {
            #region Implementation of IComparer

            public int Compare(object x, object y)
            {
                var left = x as PropertyItem;
                var right = y as PropertyItem;

                if( left == null || right == null )
                {
                    return 0;
                }

                return left.Name.CompareTo(right.Name);
            }

            #endregion
        }
    }
}
