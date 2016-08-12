using System.ComponentModel;
using System.Linq;
using System.Collections;

namespace ChristianMoser.WpfInspector.UserInterface.Controls.PropertyItems
{
    public class ListPropertyItem : CompositePropertyItem
    {
        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="ListPropertyItem"/> class.
        /// </summary>
        public ListPropertyItem(PropertyDescriptor property, object instance)
            : base(property, instance)
        {
            var enumerable = Property.GetValue(Instance) as IEnumerable;

            if (enumerable != null)
            {
                HasChildren = enumerable.Cast<object>().Count() > 0;
            }
        }


        #endregion

        #region Overrides

        protected override void BuildPropertyItemList()
        {
            var enumerable = Property.GetValue(Instance) as IEnumerable;

            if (enumerable != null)
            {
                int index = 0;
                foreach (object instance in enumerable)
                {
                    PropertyItems.Add(new ListItemPropertyItem(instance, index++));
                }    
            }
            Properties.Refresh();
        }

        #endregion

    }
}
