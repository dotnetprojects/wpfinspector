using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using ChristianMoser.WpfInspector.Services.StyleExplorer;
using ChristianMoser.WpfInspector.Utilities;
using System.Windows.Threading;

namespace ChristianMoser.WpfInspector.UserInterface.Controls.PropertyItems
{
    public class StylePropertyItem : PropertyItem
    {
        #region Private Members

        private readonly List<StyleItem> _styleItems = new List<StyleItem>();

        #endregion
        #region Construction

        public StylePropertyItem(PropertyDescriptor property, object instance)
            : base(property, instance)
        {
            Styles = new ListCollectionView(_styleItems);
            Dispatcher.CurrentDispatcher.BeginInvoke((Action) LoadStyles, DispatcherPriority.Background);
        }

        private void LoadStyles()
        {
            var style = Property.GetValue(Instance) as Style;
            var fe = Instance as FrameworkElement;
            if (style != null && fe != null)
            {
                // Add other applicable styles
                var targetType = fe.GetType();
                try
                {
                    foreach (var resource in ResourceHelper.GetResourcesRecursivelyIncludingApp<Style>(fe))
                    {
                        try
                        {
                            var styleResource = resource.Value as Style;
                            if (styleResource != null && styleResource.TargetType != null)
                            {
                                if ((targetType == styleResource.TargetType || targetType.IsSubclassOf(styleResource.TargetType)))
                                {
                                    var resourceStyleItem = new StyleItem(styleResource, fe, StyleHelper.GetKeyString(resource.Key),
                                                                          StyleHelper.GetSource(resource.Owner), StyleScope.Local);

                                    _styleItems.Add(resourceStyleItem);
                                }
                            }
                        }
                        catch (Exception)
                        { }
                    }
                }
                catch (Exception) 
                { }

                // Add the current style
                StyleItem styleItem = null;
                try
                {
                    if (StyleHelper.TryGetStyleItem(fe, style, out styleItem))
                    {
                        if (!_styleItems.Contains(styleItem))
                        {
                            _styleItems.Add(styleItem);
                        }
                    }
                }
                catch (Exception)
                { }

                Styles.Refresh();
                if (styleItem != null)
                    Styles.MoveCurrentTo(styleItem);
                Styles.CurrentChanged += OnStyleSelectionChanged;
            }
        }

        private void OnStyleSelectionChanged(object sender, System.EventArgs e)
        {
            var fe = Instance as FrameworkElement;
            var styleItem = Styles.CurrentItem as StyleItem;
            if (fe != null && styleItem != null)
            {
                fe.Style = styleItem.Style;
            }
        }

        public override void Dispose()
        {
            base.Dispose();

            foreach (var styleItem in _styleItems)
            {
                styleItem.Dispose();
            }
        }

        public ICollectionView Styles { get; private set; }

        #endregion
    }
}
