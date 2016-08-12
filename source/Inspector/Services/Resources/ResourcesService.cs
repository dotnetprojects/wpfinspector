using System;
using System.Collections.Generic;
using ChristianMoser.WpfInspector.Services.ElementTree;
using ChristianMoser.WpfInspector.Utilities;
using System.Windows;
using System.Windows.Threading;

namespace ChristianMoser.WpfInspector.Services.Resources
{
    /// <summary>
    /// 
    /// </summary>
    public class ResourcesService
    {
        #region Private Members

        private readonly List<ResourceItem> _resourceItems = new List<ResourceItem>();

        #endregion

        public event EventHandler ResourcesListChanged;

        public List<ResourceItem> ResourceItems
        {
            get { return _resourceItems; }
        }

        public void UpdateResourceList(TreeItem treeItem)
        {
            ResourceItems.Clear();

            if (treeItem != null)
            {
                var fe = treeItem.Instance as FrameworkElement;
                if (fe != null)
                {
                    ResourceItems.AddRange(ResourceItemFactory.GetResourcesForElement(fe));
                }
            }

            NotifyResourceListChanged();
        }

        private void NotifyResourceListChanged()
        {
            if (ResourcesListChanged != null)
            {
                ResourcesListChanged(this, EventArgs.Empty);
            }
        }
    }
}
