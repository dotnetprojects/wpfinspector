using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace ChristianMoser.WpfInspector.UserInterface.Controls
{
    public enum ItemType
    {
        Resource,
        Trigger,
        Setter,
        Style
    }

    [ContentProperty("Content")]
    public class ItemControl : HeaderedContentControl
    {

        #region Dependency Property ItemType

        /// <summary>
        /// Gets or sets the type of the item.
        /// </summary>
        public ItemType ItemType
        {
            get { return (ItemType)GetValue(ItemTypeProperty); }
            set { SetValue(ItemTypeProperty, value); }
        }

        /// <summary>
        /// Registers a dependency property to get or set the item type
        /// </summary>
        public static readonly DependencyProperty ItemTypeProperty =
            DependencyProperty.Register("ItemType", typeof(ItemType), typeof(ItemControl), 
            new FrameworkPropertyMetadata(Controls.ItemType.Resource));

        #endregion

    }
}
