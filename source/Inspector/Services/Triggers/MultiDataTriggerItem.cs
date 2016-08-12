using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace ChristianMoser.WpfInspector.Services.Triggers
{
    public class MultiDataTriggerItem : TriggerItemBase
    {
        private readonly MultiDataTrigger _trigger;
        private readonly FrameworkElement _source;

        public MultiDataTriggerItem(MultiDataTrigger trigger, FrameworkElement source, TriggerSource triggerSource)
            : base(source, TriggerType.MultiDataTrigger, triggerSource)
        {
            _trigger = trigger;
            _source = source;
        }

        protected override IEnumerable<SetterItem> GetSetters()
        {
            return _trigger.Setters.Select(s => new SetterItem(s, _source));
        }

        protected override IEnumerable<ConditionItem> GetConditions()
        {
            foreach (var condition in _trigger.Conditions)
            {
                yield return new ConditionItem(condition.Binding, Instance, condition.Value);
            }
        }
    }
}
