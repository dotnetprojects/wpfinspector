using System.Collections;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Controls;

namespace ChristianMoser.WpfInspector.Services.ElementTree
{
    public class LogicalTreeItem : TreeItem
    {
        private static readonly Dictionary<object, TreeItem> LogicalTreeItemLookup = new Dictionary<object, TreeItem>();

        public LogicalTreeItem(object instance, TreeItem parent, TreeModel model, int level)
            : base(instance, parent, model, level)
        {
            var frameworkElement = instance as FrameworkElement;
            if (frameworkElement != null && !string.IsNullOrEmpty(frameworkElement.Name))
            {
                var name = frameworkElement.GetType().Name;
                Name = string.Format("{0} ({1})", name, frameworkElement.Name);
            }
            else
            {
                Name = instance.GetType().Name;
            }

            if (!LogicalTreeItemLookup.ContainsKey(instance))
            {
                LogicalTreeItemLookup.Add(instance, this);
            }
            else
            {
                LogicalTreeItemLookup[instance] = this;
            }
        }

        protected override IEnumerable GetChildren()
        {
            var dependencyObject = Instance as DependencyObject;
            if (dependencyObject != null)
            {
                foreach (var child in LogicalTreeHelper.GetChildren(dependencyObject))
                {
                    if (child is DependencyObject)
                    {
                        yield return child;
                    }
                }
            }
        }

        public static TreeItem FindLogicalTreeItem(object instance)
        {
            if (instance == null)
            {
                return null;
            }
            if (LogicalTreeItemLookup.ContainsKey(instance))
            {
                return LogicalTreeItemLookup[instance];
            }
            return null;
        }

        protected override TreeItem CreateChild(object instance, TreeItem parent)
        {
            return new LogicalTreeItem(instance, parent, Model, Level+1);
        }

        protected override void DisposeInternal()
        {
            base.DisposeInternal();
            LogicalTreeItemLookup.Remove(Instance);            
        }
    }
}
