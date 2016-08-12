using System.Collections.Generic;
using System.Windows.Media;
using System.Windows;
using System.Collections;

namespace ChristianMoser.WpfInspector.Services.ElementTree
{
    public class VisualTreeItem : TreeItem
    {
        private static readonly Dictionary<object, TreeItem> VisualTreeItemLookup = new Dictionary<object, TreeItem>();

        /// <summary>
        /// Initializes a new instance of the <see cref="VisualTreeItem"/> class.
        /// </summary>
        public VisualTreeItem(object instance, TreeItem parent, TreeModel model, int level)
            : base(instance, parent, model, level)
        {
            var frameworkElement = instance as FrameworkElement;

            if (frameworkElement != null && !string.IsNullOrEmpty(frameworkElement.Name))
            {
                Name = string.Format("{0} ({1})", frameworkElement.GetType().Name, frameworkElement.Name);
            }
            else
            {
                Name = instance.GetType().Name;
            }

            if (!VisualTreeItemLookup.ContainsKey(instance))
            {
                VisualTreeItemLookup.Add(instance, this);
            }
            else
            {
                VisualTreeItemLookup[instance] = this;
            }

        }

        public static TreeItem FindVisualTreeItem(object instance)
        {
            if (instance == null)
            {
                return null;
            }
            if (VisualTreeItemLookup.ContainsKey(instance))
            {
                return VisualTreeItemLookup[instance];
            }
            return null;
        }

        protected override IEnumerable GetChildren()
        {
            var dependencyObject = Instance as DependencyObject;
            if (dependencyObject != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(dependencyObject); i++)
                {
                    yield return VisualTreeHelper.GetChild(dependencyObject, i);
                }
            }
        }

        protected override TreeItem CreateChild(object instance, TreeItem parent)
        {
            return new VisualTreeItem(instance, parent, Model, Level+1);
        }

        protected override void DisposeInternal()
        {
            base.DisposeInternal();
            VisualTreeItemLookup.Remove(Instance);
        }
    }
}
