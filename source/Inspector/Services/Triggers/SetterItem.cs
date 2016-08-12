using System.Windows;
using ChristianMoser.WpfInspector.Services.Resources;

namespace ChristianMoser.WpfInspector.Services.Triggers
{
    /// <summary>
    /// Abstraction model of a <see cref="Setter"/>
    /// </summary>
    public class SetterItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SetterItem"/> class.
        /// </summary>
        public SetterItem(SetterBase setterBase, FrameworkElement source)
        {
            var setter = setterBase as Setter;
            if (setter != null)
            {
                Property = setter.Property.Name;
                if (setter.Value is DynamicResourceExtension)
                {
                    var resourceKey = ((DynamicResourceExtension) setter.Value).ResourceKey;
                    Value = ResourceItemFactory.CreateResourceItem(resourceKey, source.Resources, source, ResourceScope.Style);
                }
                else if (setter.Value != null)
                {
                    Value = setter.Value;
                }
                else
                {
                    Value = "null";
                }
            }
        }

        /// <summary>
        /// Gets if the value is overridden by a descending style or template
        /// </summary>
        public bool IsOverridden { get; set; }

        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        public string Property { get; private set; }

        /// <summary>
        /// Gets the value.
        /// </summary>
        public object Value { get; private set; }

    }
}
