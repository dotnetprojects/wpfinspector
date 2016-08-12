using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace ChristianMoser.WpfInspector.Services.Triggers
{
    public class MultiTriggerItem : TriggerItemBase
    {
        #region Private Members

        private readonly MultiTrigger _trigger;
        private readonly FrameworkElement _source;

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiTriggerItem"/> class.
        /// </summary>
        public MultiTriggerItem(MultiTrigger trigger, FrameworkElement source, TriggerSource triggerSource)
            : base(source, TriggerType.MultiTrigger, triggerSource)
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
            foreach (var condition in _trigger.Conditions)
            {
                object instance = Instance;
                var control = Instance as Control;
                if( condition.SourceName != null && TriggerSource == TriggerSource.ControlTemplate && 
                    control != null && control.Template != null)
                {
                    var source = control.Template.FindName(condition.SourceName, control);
                    if( source != null)
                    {
                        instance = source;
                    }
                }

                yield return  new ConditionItem(condition.Property, instance, condition.Value);
            }
        }
    }
}
