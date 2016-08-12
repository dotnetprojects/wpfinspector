using System.Collections.Generic;
using System.Windows;
using ChristianMoser.WpfInspector.Services.Resources;
using ChristianMoser.WpfInspector.Services.Triggers;
using ChristianMoser.WpfInspector.Utilities;
using System;

namespace ChristianMoser.WpfInspector.Services.StyleExplorer
{
    /// <summary>
    /// Model of a style
    /// </summary>
    public class StyleItem : IDisposable
    {
        #region Private Members

        private readonly List<SetterItem> _setterItems = new List<SetterItem>();
        private readonly List<TriggerItemBase> _triggerItems = new List<TriggerItemBase>();
        private readonly List<ResourceItem> _resourceItems = new List<ResourceItem>();
        private string _fullLocation;

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="StyleItem"/> class.
        /// </summary>
        public StyleItem(Style style, FrameworkElement source, string name, string location, StyleScope scope)
        {
            Style = style;
            Name = name;
            Location = location;
            Scope = scope;

            // Setters
            foreach (Setter setter in style.Setters)
            {
                if(setter.Property.Name == "OverridesDefaultStyle" && setter.Value.ToString().ToLower() == "true")
                {
                    OverridesDefaultStyle = true;
                }
                _setterItems.Add(new SetterItem(setter, source));
            }

            // Triggers
            foreach (var trigger in style.Triggers)
            {
                _triggerItems.Add(TriggerItemFactory.GetTriggerItem(trigger, source, TriggerSource.Style ));
            }

            // Ressources
            foreach (var resource in ResourceHelper.GetResourcesRecursively<object>(style.Resources))
            {
                _resourceItems.Add(ResourceItemFactory.CreateResourceItem(resource.Key, resource.Owner, source, ResourceScope.Style));
            }

            GlobalIndex = StyleHelper.GetGlobalIndex(style);
        }

        #endregion

        /// <summary>
        /// Gets or sets the index of the global.
        /// </summary>
        public int GlobalIndex { get; private set; }

        /// <summary>
        /// Gets the scope.
        /// </summary>
        public StyleScope Scope { get; private set; }

        /// <summary>
        /// Gets the style.
        /// </summary>
        public Style Style { get; private set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the location of the style
        /// </summary>
        public string Location { get; private set; }

        /// <summary>
        /// Gets or sets the full location of the style
        /// </summary>
        public string FullLocation
        {
            get { return _fullLocation ?? Location; }
            set { _fullLocation = value; }
        }

        /// <summary>
        /// Gets or sets if this style overrides the default style.
        /// </summary>
        public bool OverridesDefaultStyle { get; private set;}

        /// <summary>
        /// Gets the setter items.
        /// </summary>
        public List<SetterItem> SetterItems
        {
            get { return _setterItems; }
        }

        /// <summary>
        /// Gets the trigger items.
        /// </summary>
        public List<TriggerItemBase> TriggerItems
        {
            get { return _triggerItems; }
        }

        /// <summary>
        /// Gets the resource items.
        /// </summary>
        public List<ResourceItem> ResourceItems
        {
            get { return _resourceItems; }
        }

        public override int GetHashCode()
        {
            return GlobalIndex;
        }

        public override bool Equals(object obj)
        {
            var styleItem = obj as StyleItem;

            if( ReferenceEquals(styleItem, this))
            {
                return true;
            }

            if (ReferenceEquals(styleItem, null))
            {
                return false;
            }

            return styleItem.GlobalIndex == GlobalIndex;
        }

        public void Dispose()
        {
            foreach (var triggerItemBase in _triggerItems)
            {
                triggerItemBase.Dispose();
            }
        }
    }
}
