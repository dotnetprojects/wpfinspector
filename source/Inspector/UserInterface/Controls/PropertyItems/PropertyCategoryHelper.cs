using System.ComponentModel;
using System.Windows.Media;
using System.Collections.Generic;

namespace ChristianMoser.WpfInspector.UserInterface.Controls.PropertyItems
{
    public class PropertySortInfo
    {
        public string Category { get; private set; }
        public int CategoryOrder { get; private set; }
        public int PropertyOrder { get; private set; }

        public PropertySortInfo(string category, int categoryOrder, int propertyOrder)
        {
            Category = category;
            CategoryOrder = categoryOrder;
            PropertyOrder = propertyOrder;
        }
    }

    public static class PropertySorter
    {
        private static Dictionary<string, PropertySortInfo> _sortInfos = new Dictionary<string, PropertySortInfo>(100);

        private const string Appearance = "Appearance";
        private const string Layout = "Layout";
        private const string Transform = "Transform";
        private const string CommonProperties = "Common Properties";
        private const string Style = "Style";
        private const string Text = "Text";
        private const string Window = "Window";
        private const string UIAutomation = "UI Automation";

        static PropertySorter()
        {
            // Appearance
            int group = 0;
            int pos = 0;
            _sortInfos.Add("Opacity", new PropertySortInfo(Appearance, group, pos++));
            _sortInfos.Add("Visibility", new PropertySortInfo(Appearance, group, pos++));
            _sortInfos.Add("BorderThickness", new PropertySortInfo(Appearance, group, pos++));
            _sortInfos.Add("Effect", new PropertySortInfo(Appearance, group, pos++));
            _sortInfos.Add("ClipToBounds", new PropertySortInfo(Appearance, group, pos++));
            _sortInfos.Add("SnapsToDevicePixels", new PropertySortInfo(Appearance, group, pos));

            // Layout
            pos = 0;
            group = group + 1;

            _sortInfos.Add("Width", new PropertySortInfo(Layout, group, pos++));
            _sortInfos.Add("Height", new PropertySortInfo(Layout, group, pos++));
            _sortInfos.Add("ZIndex", new PropertySortInfo(Layout, group, pos++));
            _sortInfos.Add("HorizontalAlignment", new PropertySortInfo(Layout, group, pos++));
            _sortInfos.Add("VerticalAlignment", new PropertySortInfo(Layout, group, pos++));
            _sortInfos.Add("Margin", new PropertySortInfo(Layout, group, pos++));
            _sortInfos.Add("Padding", new PropertySortInfo(Layout, group, pos++));
            _sortInfos.Add("Grid.Column", new PropertySortInfo(Layout, group, pos++));
            _sortInfos.Add("Grid.Row", new PropertySortInfo(Layout, group, pos++));
            _sortInfos.Add("Grid.ColumnSpan", new PropertySortInfo(Layout, group, pos++));
            _sortInfos.Add("Grid.RowSpan", new PropertySortInfo(Layout, group, pos++));
            _sortInfos.Add("Grid.IsSharedSizeScope", new PropertySortInfo(Layout, group, pos++));
            _sortInfos.Add("DockPanel.Dock", new PropertySortInfo(Layout, group, pos++));
            _sortInfos.Add("Canvas.Left", new PropertySortInfo(Layout, group, pos++));
            _sortInfos.Add("Canvas.Top", new PropertySortInfo(Layout, group, pos++));
            _sortInfos.Add("Canvas.Right", new PropertySortInfo(Layout, group, pos++));
            _sortInfos.Add("Canvas.Bottom", new PropertySortInfo(Layout, group, pos++));
            _sortInfos.Add("HorizontalContentAlignment", new PropertySortInfo(Layout, group, pos++));
            _sortInfos.Add("VerticalContentAlignment", new PropertySortInfo(Layout, group, pos++));
            _sortInfos.Add("MaxWidth", new PropertySortInfo(Layout, group, pos++));
            _sortInfos.Add("MaxHeight", new PropertySortInfo(Layout, group, pos++));
            _sortInfos.Add("MinWidth", new PropertySortInfo(Layout, group, pos++));
            _sortInfos.Add("MinHeight", new PropertySortInfo(Layout, group, pos++));
            _sortInfos.Add("ActualWidth", new PropertySortInfo(Layout, group, pos++));
            _sortInfos.Add("ActualHeight", new PropertySortInfo(Layout, group, pos));

            // Common Properties
            pos = 0;
            group = group + 1;
            _sortInfos.Add("Content", new PropertySortInfo(CommonProperties, group, pos++));
            _sortInfos.Add("IsCancel", new PropertySortInfo(CommonProperties, group, pos++));
            _sortInfos.Add("IsDefault", new PropertySortInfo(CommonProperties, group, pos++));
            _sortInfos.Add("Cursor", new PropertySortInfo(CommonProperties, group, pos++));
            _sortInfos.Add("IsEnabled", new PropertySortInfo(CommonProperties, group, pos++));
            _sortInfos.Add("ToolTip", new PropertySortInfo(CommonProperties, group, pos));

            // Style
            pos = 0;
            group = group + 1;
            _sortInfos.Add("Style", new PropertySortInfo(Style, group, pos));

            // Style
            pos = 0;
            group = group + 1;
            _sortInfos.Add("LayoutTransform", new PropertySortInfo(Transform, group, pos++));
            _sortInfos.Add("RenderTransform", new PropertySortInfo(Transform, group, pos));

            // Text
            pos = 0;
            group = group + 1;
            _sortInfos.Add("FontFamily", new PropertySortInfo(Text, group, pos++));
            _sortInfos.Add("FontSize", new PropertySortInfo(Text, group, pos++));
            _sortInfos.Add("FontStyle", new PropertySortInfo(Text, group, pos++));
            _sortInfos.Add("FontStretch", new PropertySortInfo(Text, group, pos++));
            _sortInfos.Add("FontWeight", new PropertySortInfo(Text, group, pos));

            // Window
            pos = 0;
            group = group + 1;
            _sortInfos.Add("Title", new PropertySortInfo(Window, group, pos++));
            _sortInfos.Add("WindowState", new PropertySortInfo(Window, group, pos++));
            _sortInfos.Add("WindowStyle", new PropertySortInfo(Window, group, pos++));
            _sortInfos.Add("WindowStartupLocation", new PropertySortInfo(Window, group, pos++));
            _sortInfos.Add("Topmost", new PropertySortInfo(Window, group, pos++));
            _sortInfos.Add("Top", new PropertySortInfo(Window, group, pos++));
            _sortInfos.Add("Left", new PropertySortInfo(Window, group, pos++));
            _sortInfos.Add("ShowInTaskBar", new PropertySortInfo(Window, group, pos++));
            _sortInfos.Add("ShowActivated", new PropertySortInfo(Window, group, pos++));
            _sortInfos.Add("OwnedWindows", new PropertySortInfo(Window, group, pos));

            // UI Automation
            pos = 0;
            group = group + 1;
            _sortInfos.Add("AutomationProperties.Name", new PropertySortInfo(UIAutomation, group, pos++));
            _sortInfos.Add("AutomationProperties.HelpText", new PropertySortInfo(UIAutomation, group, pos));
        }

        
        public static PropertySortInfo GetSortInfo(PropertyDescriptor property)
        {
            var type = property.PropertyType;
            if (type == typeof(Brush))
            {
                return new PropertySortInfo("Brushes", 0, 0);
            }

            string name = property.Name;
            if( _sortInfos.ContainsKey(name))
            {
                return _sortInfos[name];
            }

            return new PropertySortInfo("Misc", int.MaxValue, 0);
        }
    }
}
