using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows.Data;
using System.Windows.Markup;
using ChristianMoser.WpfInspector.UserInterface.Controls.PropertyItems;
using ChristianMoser.WpfInspector.Services;
using ChristianMoser.WpfInspector.Services.ElementTree;
using System.Windows;
using ChristianMoser.WpfInspector.Utilities;

namespace ChristianMoser.WpfInspector.UserInterface
{
    public enum BindingSource
    {
        DataContext,
        FindAncestor,
        ElementName,
        Self,
        TemplatedParent,
        PreviousData
    }
    
    public class PathItem : INotifyPropertyChanged
    {
        private readonly bool _isReadonly;
        private List<PathItem> _children;
        private ICollectionView _childrenView;
        private readonly int _dept;
        private readonly object _instance;
        private bool _isSelected;
        private bool _isExpanded;

        public PathItem(object instance, string property, string path, int dept, bool isReadonly)
        {
            _isReadonly = isReadonly;
            Name = property;
            if (property == "")
            {
                Name = instance.GetType().Name;
            }
            
            Path = path;
            _dept = dept;
            _instance = instance;

            if( _dept < 2)
            {
                IsExpanded = true;
            }
        }

        public string Name { get; private set; }
        public string Path { get; private set; }
        public bool IsReadonly { get { return _isReadonly; } }

        public ICollectionView Children
        {
            get
            {
                if( _children == null)
                {
                    _children = new List<PathItem>();
                    try
                    {
                        foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(_instance))
                        {
                            var pathItem = new PathItem(property.GetValue(_instance), property.Name,
                                string.IsNullOrEmpty(Path) ? property.Name : Path + "." + property.Name, _dept + 1, property.IsReadOnly);
                            _children.Add(pathItem);
                        }
                    }
                    catch (Exception)
                    { }

                    _childrenView = new ListCollectionView(_children);
                    _childrenView.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
                }
                return _childrenView;
            }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set { PropertyChanged.ChangeAndNotify(ref _isSelected, value, this, "IsSelected"); }
        }

        public bool IsExpanded
        {
            get { return _isExpanded; }
            set { PropertyChanged.ChangeAndNotify(ref _isExpanded, value, this, "IsExpanded"); }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }

    public class BindingEditorViewModel : INotifyPropertyChanged
    {
        #region Private Members

        private string _expression;
        private PathItem _pathItem;
        private readonly DependencyObject _instance;
        private readonly List<PathItem> _pathItems = new List<PathItem>();
        private readonly DependencyPropertyDescriptor _dpd;
        private Binding _binding;

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="BindingEditorViewModel"/> class.
        /// </summary>
        /// <param name="propertyItem">The property item.</param>
        public BindingEditorViewModel(PropertyItem propertyItem)
        {
            _instance = (DependencyObject)propertyItem.Instance;

            Sources = new ListCollectionView(Enum.GetValues(typeof(BindingSource)));
            Sources.MoveCurrentToFirst();
            Sources.CurrentChanged += (s, e) => OnSourceChanged();

            Modes = new ListCollectionView(Enum.GetValues(typeof(BindingMode)));
            Modes.MoveCurrentToFirst();
            Modes.CurrentChanged += (s, e) => UpdateBinding();
            
            UpdateSourceTriggers = new ListCollectionView(Enum.GetValues(typeof (UpdateSourceTrigger)));
            UpdateSourceTriggers.MoveCurrentToFirst();
            UpdateSourceTriggers.CurrentChanged += (s, e) => UpdateBinding();

            SourceList = new ListCollectionView(_pathItems);
            BuildSourceTree();

            _dpd = DependencyPropertyDescriptor.FromProperty(propertyItem.Property);

            _binding  = BindingOperations.GetBinding((DependencyObject) propertyItem.Instance, _dpd.DependencyProperty);
            if (_binding == null)
            {
                UpdateBinding();    
            }
            else
            {
                if( _binding.Source == null )
                {
                    Sources.MoveCurrentTo(BindingSource.DataContext);
                }
                else if( _binding.RelativeSource != null )
                {
                    if( _binding.RelativeSource.Mode == RelativeSourceMode.PreviousData )
                    {
                        Sources.MoveCurrentTo(BindingSource.PreviousData);
                    }
                    else if( _binding.RelativeSource.Mode == RelativeSourceMode.TemplatedParent)
                    {
                        Sources.MoveCurrentTo(BindingSource.TemplatedParent);
                    }
                    else if( _binding.RelativeSource.Mode == RelativeSourceMode.FindAncestor)
                    {
                        Sources.MoveCurrentTo(BindingSource.FindAncestor);
                    }
                }
                UpdateExpression();
            }
        }

        #endregion


        /// <summary>
        /// Gets or sets the source list.
        /// </summary>
        /// <value>The source list.</value>
        public ICollectionView SourceList { get; private set; }

        /// <summary>
        /// Gets or sets the sources.
        /// </summary>
        /// <value>The sources.</value>
        public ICollectionView Sources { get; private set;}

        /// <summary>
        /// Gets or sets the sources.
        /// </summary>
        /// <value>The sources.</value>
        public ICollectionView Modes { get; private set; }

        /// <summary>
        /// Gets or sets the update source triggers.
        /// </summary>
        /// <value>The update source triggers.</value>
        public ICollectionView UpdateSourceTriggers { get; private set; }

        /// <summary>
        /// Sets the selected path item.
        /// </summary>
        /// <value>The selected path item.</value>
        public PathItem SelectedPathItem
        {
            set
            {
                _pathItem = value;

                var mode = (BindingMode) Modes.CurrentItem;

                // Coerce binding mode
                if (_pathItem.IsReadonly && (mode == BindingMode.TwoWay || mode == BindingMode.OneWayToSource) )
                {
                    Modes.MoveCurrentTo(BindingMode.OneWay);
                }

                UpdateBinding();
            }
        }

         /// <summary>
         /// Gets or sets the binding expression.
         /// </summary>
         /// <value>The expression.</value>
        public string Expression
        {
            get { return _expression; }
            set { PropertyChanged.ChangeAndNotify(ref _expression, value, this, "Expression"); }
        }

        /// <summary>
        /// Activates the binding after the dialog has been closed by clicking OK
        /// </summary>
        public bool ActivateBinding()
        {
            try
            {
                BindingOperations.SetBinding(_instance, _dpd.DependencyProperty, _binding);
            }
            catch( Exception ex)
            {
                MessageBox.Show(ex.Message, "Error activating binding", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }

        private void BuildSourceTree()
        {
            _pathItems.Clear();

            if( (BindingSource)Sources.CurrentItem == BindingSource.DataContext)
            {
                var fe = _instance as FrameworkElement;
                if( fe != null && fe.DataContext != null )
                {
                    var pathItem = new PathItem(fe.DataContext, "", "", 0, true);
                    _pathItems.Add(pathItem);
                }
            }

            SourceList.Refresh();
        }

        private void UpdateBinding()
        {
            _binding = new Binding();
            _binding.Path = _pathItem == null ? null : new PropertyPath(_pathItem.Path);
            _binding.UpdateSourceTrigger = (UpdateSourceTrigger) UpdateSourceTriggers.CurrentItem;
            _binding.Mode = (BindingMode) Modes.CurrentItem;
                
            var bindingSource = (BindingSource)Sources.CurrentItem;

            if (bindingSource == BindingSource.TemplatedParent)
                _binding.RelativeSource = new RelativeSource(RelativeSourceMode.TemplatedParent);
            else if (bindingSource == BindingSource.Self)
                _binding.RelativeSource = new RelativeSource(RelativeSourceMode.Self);
            else if (bindingSource == BindingSource.PreviousData)
                _binding.RelativeSource = new RelativeSource(RelativeSourceMode.PreviousData);
            else if (bindingSource == BindingSource.FindAncestor)
            {
                _binding.RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor);
            }

            if (bindingSource == BindingSource.ElementName)
            {
                // Element name binding
            }

            UpdateExpression();
        }

        private void UpdateExpression()
        {
            string bindingString = XamlWriter.Save(_binding);
            var result = Regex.Replace(bindingString, "xmlns=\"(.)*\"", "");
            result = result.Replace("\"", "");
            if (result.Length > 5)
            {
                result = string.Concat("{", result.Substring(1, result.Length - 4), "}");
            }
            Expression = result;
        }

        private void OnSourceChanged()
        {
            BuildSourceTree();
            UpdateBinding();
        }       

      
        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

    }
}
