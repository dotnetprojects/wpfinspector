using System;
using System.Collections;
using System.Windows;
using ChristianMoser.WpfInspector.Services.DataObjects;

namespace ChristianMoser.WpfInspector.Services.ElementTree
{
    public class PresentationSourceTreeItem : TreeItem
    {
        private readonly TreeType _treeType;
        private readonly PresentationSource _presentationSource;

        public PresentationSourceTreeItem(PresentationSource presentationSource, TreeModel model, TreeType treeType)
            : base(presentationSource, null, model, 0)
        {    
            Name = "PresentationSource";
            IsExpanded = true;
            
            _treeType = treeType;
            _presentationSource = presentationSource;
        }

        protected override IEnumerable GetChildren()
        {
            yield return _presentationSource.RootVisual;
        }

        protected override TreeItem CreateChild(object instance, TreeItem parent)
        {
            switch( _treeType  )
            {
                case TreeType.LogicalTree:
                    return new LogicalTreeItem(instance, this, Model, Level + 1);
                case TreeType.VisualTree:
                    return new VisualTreeItem(instance, this, Model, Level + 1);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
