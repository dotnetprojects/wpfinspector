using System;
using System.Collections.Generic;
using System.Windows;
using ChristianMoser.WpfInspector.Services.DataObjects;
using ChristianMoser.WpfInspector.UserInterface;
using System.Collections;

namespace ChristianMoser.WpfInspector.Services.ElementTree
{
    public enum TreeType
    {
        LogicalTree,
        VisualTree
    }

    public class ApplicationTreeItem : TreeItem
    {
        private readonly TreeType _treeType;

        public ApplicationTreeItem(Application app, TreeModel model, TreeType treeType)
            : base(app, null, model, 0)
        {
            Name = "Application";
            IsExpanded = true;
            _treeType = treeType;

        }

        protected override IEnumerable GetChildren()
        {
            var windows = new List<DependencyObject>();
            var app = (Application)Instance;

            foreach (Window window in app.Windows)
            {
                // Don't inspect our own window
                if (window is InspectorWindow)
                    continue;

                windows.Add(window);
            }

            return windows;
        }

        protected override TreeItem CreateChild(object instance, TreeItem parent)
        {
            switch (_treeType)
            {
                case TreeType.LogicalTree:
                    return new LogicalTreeItem(instance, parent, Model, 1);
                case TreeType.VisualTree:
                    return new VisualTreeItem(instance, parent, Model, 1);
                default:
                    throw new ArgumentException();
            }
        }
    }
}
