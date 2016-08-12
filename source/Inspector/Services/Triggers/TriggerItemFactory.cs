using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace ChristianMoser.WpfInspector.Services.Triggers
{
    public static class TriggerItemFactory
    {
        public static TriggerItemBase GetTriggerItem(TriggerBase trigger, FrameworkElement source, TriggerSource triggerSource)
        {
            TriggerItemBase triggerItem;
            if (trigger is Trigger)
            {
                triggerItem = new TriggerItem((Trigger)trigger, source, triggerSource);
            }
            else if (trigger is DataTrigger)
            {
                triggerItem = new DataTriggerItem((DataTrigger)trigger, source, triggerSource);
            }
            else if (trigger is MultiTrigger)
            {
                triggerItem = new MultiTriggerItem((MultiTrigger)trigger, source, triggerSource);
            }
            else if (trigger is MultiDataTrigger)
            {
                triggerItem = new MultiDataTriggerItem((MultiDataTrigger)trigger, source, triggerSource);
            }
            else
            {
                return null;
            }
            triggerItem.Initialize();
            return triggerItem;
        }
    }
}
