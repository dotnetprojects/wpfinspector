using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace ChristianMoser.WpfInspector.Services.Triggers
{
    public class TriggerItem : TriggerItemBase
    {
        #region Private Members

        private readonly Trigger _trigger;
        private readonly FrameworkElement _source;

        #endregion

        #region Construction

        public TriggerItem(Trigger trigger, FrameworkElement source, TriggerSource triggerSource)
            : base(source, TriggerType.Trigger, triggerSource)
        {
            _trigger = trigger;
            _source = source;
        }

        #endregion

        protected override IEnumerable<SetterItem> GetSetters()
        {
            return _trigger.Setters.Select(s => new SetterItem(s, _source));
        }

        protected override IEnumerable<ConditionItem> GetConditions()
        {
            yield return new ConditionItem(_trigger.Property, Instance, _trigger.Value);
        }

    }
}
