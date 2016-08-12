using System;
using System.Collections.Generic;
using System.Windows.Automation;
using System.Collections;

namespace ChristianMoser.WpfInspector.Services.ElementTree
{
    public class AutomationItem : TreeItem
    {
        private readonly AutomationElement _element;
        private readonly static TreeWalker TreeWalker = TreeWalker.ControlViewWalker;

        public AutomationItem(object item, TreeItem parent, TreeModel model, int level)
            : base(item, parent, model,level)
        {
            _element = (AutomationElement)item;
            Name = _element.Current.ControlType.LocalizedControlType + " " + _element.Current.Name;
        }

        protected override IEnumerable GetChildren()
        {
            var children = new List<AutomationElement>();
            try
            {
                var child = TreeWalker.GetFirstChild(_element);
                while (child != null)
                {
                    children.Add(child);
                    child = TreeWalker.GetNextSibling(child);
                }
            }
            catch (Exception)
            {
                Dispose();
                if (Parent != null)
                {
                    Parent.Children.Remove(this);
                }
            }
            return children;

        }

        protected override TreeItem CreateChild(object instance, TreeItem parent)
        {
            return new AutomationItem(instance, parent, Model, Level+1);
        }
    }
}
