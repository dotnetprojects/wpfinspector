using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using ChristianMoser.WpfInspector.Utilities;

namespace ChristianMoser.WpfInspector.Services.Triggers
{
    public enum TriggerType
    {
        Trigger,
        DataTrigger,
        EventTrigger,
        MultiTrigger,
        MultiDataTrigger
    }

    public enum TriggerSource
    {
        Style,
        ControlTemplate,
        Element,
        DataTemplate
    }

    public abstract class TriggerItemBase : INotifyPropertyChanged, IDisposable
    {
        #region Private Members

        private bool _isActive;
        private readonly List<SetterItem> _setters = new List<SetterItem>();
        private readonly List<ConditionItem> _conditions = new List<ConditionItem>();

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="TriggerItemBase"/> class.
        /// </summary>
        protected TriggerItemBase(object instance, TriggerType triggerType, TriggerSource triggerSource)
        {
            Instance = instance;
            TriggerType = triggerType;
            TriggerSource = triggerSource;
        }

        #endregion

        /// <summary>
        /// Initializes this trigger.
        /// </summary>
        public void Initialize()
        {
            _setters.AddRange(GetSetters());
            Setters = new ListCollectionView(_setters);

            _conditions.AddRange(GetConditions());
            foreach (var condition in _conditions)
            {
                condition.StateChanged += OnConditionStateChanged;
            }
            Conditions = new ListCollectionView(_conditions);
            
            OnConditionStateChanged(this, EventArgs.Empty);
        }

        /// <summary>
        /// Gets or the setters.
        /// </summary>
        public ICollectionView Setters { get; protected set; }

        /// <summary>
        /// Gets the conditions.
        /// </summary>
        public ICollectionView Conditions { get; protected set; }

        /// <summary>
        /// Gets the source of the trigger.
        /// </summary>
        public TriggerSource TriggerSource { get; private set; }

        /// <summary>
        /// Gets the type of the trigger.
        /// </summary>
        public TriggerType TriggerType { get; private set; }

        /// <summary>
        /// Gets if the trigger is currently active
        /// </summary>
        public bool IsActive
        {
            get { return _isActive; }
            set { PropertyChanged.ChangeAndNotify(ref _isActive, value, this, "IsActive"); }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region IDisposable Members

        public virtual void Dispose()
        {
            foreach (var condition in _conditions)
            {
                condition.StateChanged -= OnConditionStateChanged;
                condition.Dispose();
            }
        }

        #endregion

        protected object Instance { get; private set; }

        protected virtual IEnumerable<SetterItem> GetSetters()
        {
            yield break;
        }

        protected  virtual IEnumerable<ConditionItem> GetConditions()
        {
            yield break;
        }

        #region Private Members

        private void OnConditionStateChanged(object sender, EventArgs e)
        {
            if (_conditions.Any(condition => !condition.IsActive))
            {
                IsActive = false;
                return;
            }
            IsActive = true;
        }

        #endregion
    }
}
