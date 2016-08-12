using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using ChristianMoser.WpfInspector.Services.ElementTree;
using ChristianMoser.WpfInspector.Utilities;

namespace ChristianMoser.WpfInspector.Services.Triggers
{
    public class TriggerService
    {
        #region Private Members

        private readonly List<TriggerItemBase> _triggers = new List<TriggerItemBase>();

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="TriggerService"/> class.
        /// </summary>
        public TriggerService()
        {
            Triggers = new ListCollectionView(_triggers);
            Triggers.GroupDescriptions.Add(new PropertyGroupDescription("TriggerSource"));
        }

        #endregion

        /// <summary>
        /// Gets or sets the triggers.
        /// </summary>
        /// <value>The triggers.</value>
        public ICollectionView Triggers { get; private set; }

        public void UpdateTriggerList(TreeItem treeItem)
        {
            foreach (var triggerItem in _triggers)
            {
                triggerItem.Dispose();
            }

            _triggers.Clear();

            if (treeItem != null)
            {
                var fe = treeItem.Instance as FrameworkElement;
                if (fe != null)
                {
                    if (fe.Style != null)
                    {
                        AddTriggers(fe, fe.Style.Triggers, TriggerSource.Style);
                    }

                    AddTriggers(fe, fe.Triggers, TriggerSource.Element);
                }

                var control = treeItem.Instance as Control;
                if (control != null && control.Template != null)
                {
                    AddTriggers(control, control.Template.Triggers, TriggerSource.ControlTemplate);
                }

                var listBoxItem = treeItem.Instance as ListBoxItem;
                if (listBoxItem != null && listBoxItem.ContentTemplate != null)
                {
                    AddTriggers(listBoxItem, listBoxItem.ContentTemplate.Triggers, TriggerSource.DataTemplate);
                }

                var listViewItem = treeItem.Instance as ListViewItem;
                if (listViewItem != null && listViewItem.ContentTemplate != null)
                {
                    AddTriggers(listViewItem, listViewItem.ContentTemplate.Triggers, TriggerSource.DataTemplate);
                }

                var contentPresenter = treeItem.Instance as ContentPresenter;
                if (contentPresenter != null && contentPresenter.ContentTemplate != null)
                {
                    AddTriggers(contentPresenter, contentPresenter.ContentTemplate.Triggers, TriggerSource.DataTemplate);
                }
            }

            Triggers.Refresh();
        }

        private void AddTriggers(FrameworkElement instance, IEnumerable<TriggerBase> triggers, TriggerSource source)
        {
            foreach (TriggerBase trigger in triggers)
            {
                var triggerItem = TriggerItemFactory.GetTriggerItem(trigger, instance, source);
                if (triggerItem != null)
                {
                    _triggers.Add(triggerItem);
                }
            }
        }

       
    }
}
