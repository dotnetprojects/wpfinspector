using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace ChristianMoser.WpfInspector.Services.Triggers
{

    public class DataTriggerItem : TriggerItemBase
    {
        #region Private Members

        private readonly DataTrigger _dataTrigger;
        private readonly FrameworkElement _source;

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="DataTriggerItem"/> class.
        /// </summary>
        public DataTriggerItem(DataTrigger trigger, FrameworkElement source, TriggerSource triggerSource)
            : base(source, TriggerType.DataTrigger, triggerSource)
        {
            _dataTrigger = trigger;
            _source = source;
        }

        #endregion

        protected override IEnumerable<SetterItem> GetSetters()
        {
            return _dataTrigger.Setters.Select(s => new SetterItem(s, _source));
        }

        protected override IEnumerable<ConditionItem> GetConditions()
        {
            yield return new ConditionItem(_dataTrigger.Binding, Instance, _dataTrigger.Value);
        }
        
    }
}
