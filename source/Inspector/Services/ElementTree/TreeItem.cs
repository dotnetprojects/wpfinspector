using System;
using System.Collections.ObjectModel;
using ChristianMoser.WpfInspector.Services.Analyzers;
using ChristianMoser.WpfInspector.Utilities;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Windows.Threading;
using System.Linq;
using System.Collections;
using System.Collections.Specialized;

namespace ChristianMoser.WpfInspector.Services.ElementTree
{
    public enum TreeItemIcon
    {
        Default,
        Window
    }

    public abstract class TreeItem : IDisposable, INotifyPropertyChanged
    {
        private readonly object _instance;
        private string _name;

        private bool _isExpanded;
        private readonly TreeItem _parent;

        private readonly ObservableCollection<TreeItem> _children = new ObservableCollection<TreeItem>();
        private AnalyzerContext _analyzercontext;

        protected TreeItem(object instance, TreeItem parent, TreeModel model, int level)
        {
            _instance = instance;
            _parent = parent;
            Model = model;
            Level = level;

            var frameworkElement = instance as FrameworkElement;
            if (frameworkElement != null)
            {
                if (frameworkElement.IsLoaded)
                {
                    frameworkElement.Unloaded += OnUnloaded;
                }
                else
                {
                    frameworkElement.Loaded += OnLoaded;
                }
            }

            var itemsControl = instance as ItemsControl;
            if (itemsControl != null)
            {
                itemsControl.ItemContainerGenerator.ItemsChanged += OnListChanged;
            }

            _children.CollectionChanged += OnChildenCountChanged;
        }

        private void OnChildenCountChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            PropertyChanged.Notify("ElementCount");
            PropertyChanged.Notify("MaxNestingLevel");
        }

        public TreeItem Parent
        {
            get { return _parent; }
        }

        /// <summary>
        /// Gets or sets the indention level
        /// </summary>
        public int Level { get; private set; }

        /// <summary>
        /// Gets the number of child elements (including this).
        /// </summary>
        public int ElementCount
        {
            get { return _children == null ? 1 : _children.Sum(c => c.ElementCount) + 1; }
        }

        /// <summary>
        /// Gets the max nesting level.
        /// </summary>
        public int MaxNestingLevel
        {
            get
            {
                if( _children == null )
                {
                    return 1;
                }
                else
                {
                    int maxLevel = 0;
                    foreach (var treeItem in _children)
                    {
                        maxLevel = Math.Max(maxLevel, treeItem.MaxNestingLevel);
                    }
                    return maxLevel+1;
                }
                //return _children == null ? 1 : _children.Max(c => c.MaxNestingLevel) + 1;
            }
        }

        /// <summary>
        /// Gets the type.
        /// </summary>
        public string Type
        {
            get
            {
                if (_instance is Button)
                    return "Button";
                if (_instance is ComboBox)
                    return "ComboBox";
                if (_instance is TabControl)
                    return "TabControl";
                if (_instance is Window)
                    return "Window";
                if (_instance is UserControl)
                    return "UserControl";
                return _instance.GetType().Name;
            }
        }

        public object Instance
        {
            get { return _instance; }
        }

        public TreeItemIcon TreeItemIcon
        {
            get
            {
                if (_instance is Window)
                {
                    return TreeItemIcon.Window;
                }
                return TreeItemIcon.Default;
            }
        }

        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                PropertyChanged.ChangeAndNotify(ref _isExpanded, value, this, "IsExpanded");

                if (value)
                {
                    Model.ExpandItem(this);
                }
                else
                {
                    Model.CollapseItem(this);
                }

                
            }
        }

        public void BringIntoView()
        {
            TreeItem current = this;
            while (current != null)
            {
                current.IsExpanded = true;
                current = current.Parent;
            }
        }

        public string Name
        {
            get { return _name; }
            set { PropertyChanged.ChangeAndNotify(ref _name, value, this, "Name"); }
        }

        public ObservableCollection<TreeItem> Children
        {
            get
            {
                return _children;
            }
        }

        public void Analyze(AnalyzerContext context)
        {
            _analyzercontext = context;
            if (_analyzercontext != null)
            {
                _analyzercontext.Analyze(this);
                List<TreeItem> cloneList;
                lock (this)
                {
                    cloneList = new List<TreeItem>(Children);
                }

                foreach (var child in cloneList)
                {
                    //child.Analyze(context);
                    TreeItem item = child;
                    Dispatcher.CurrentDispatcher.BeginInvoke((Action)(() => item.Analyze(context)),
                                                             DispatcherPriority.Background);
                }
            }
        }

        public void Refresh()
        {
            var add = new List<TreeItem>();
            var remove = Children.ToDictionary(treeItem => treeItem.Instance);

            foreach (var visual in GetChildren())
            {
                if (visual == null)
                    continue;

                if (remove.ContainsKey(visual))
                {
                    remove.Remove(visual);
                }
                else
                {
                    var treeItem = CreateChild(visual, this);
                    treeItem._analyzercontext = _analyzercontext;
                    add.Add(treeItem);
                }
            }

            lock (this)
            {
                int childenCountBefore = Children.Count;
                foreach (var treeItem in add)
                {
                    Children.Add(treeItem);
                }

                foreach (var treeItem in remove.Values)
                {
                    Children.Remove(treeItem);
                    treeItem.Dispose();
                }

                Model.UpdateItem(this, add, remove.Values, childenCountBefore);
            }

            foreach (var treeItem in Children)
            {
                TreeItem item = treeItem;
                item.Refresh();
                //Dispatcher.CurrentDispatcher.BeginInvoke((Action)(item.Refresh),
                //                                       DispatcherPriority.Background);
            }

        }

        protected TreeModel Model { get; set; }

        protected abstract IEnumerable GetChildren();
        protected abstract TreeItem CreateChild(object instance, TreeItem parent);

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            lock (this)
            {
                DisposeInternal();

                Model.RemoveItem(this);

                if (_analyzercontext != null)
                {
                    _analyzercontext.ClearTreeItemIssues(this);
                }

                foreach (var treeItem in Children)
                {
                    treeItem.Dispose();
                }
                Children.Clear();
            }
        }

        protected virtual void DisposeInternal()
        {
            var frameworkElement = Instance as FrameworkElement;
            if (frameworkElement != null)
            {
                frameworkElement.Unloaded -= OnUnloaded;
            }

            var itemsControl = Instance as ItemsControl;
            if (itemsControl != null)
            {
                itemsControl.ItemContainerGenerator.ItemsChanged -= OnListChanged;
            }
        }

        private void OnListChanged(object sender, EventArgs e)
        {
            Refresh();
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            Parent.Children.Remove(this);
            Dispose();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var fe = (FrameworkElement)sender;
            fe.Unloaded += OnUnloaded;
            fe.Loaded -= OnLoaded;
        }


        #endregion

    }
}
