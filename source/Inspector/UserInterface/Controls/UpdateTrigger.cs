using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace ChristianMoser.WpfInspector.UserInterface.Controls
{
    public class UpdateTrigger
    {
        private bool _isUpdateRequired;

        public event EventHandler IsUpdateRequiredChanged;

        public bool IsUpdateRequired
        {
            get { return _isUpdateRequired; }
            set
            {
                _isUpdateRequired = value;
                OnIsUpdateRequiredChanged();
            }
        }

        public Action UpdateAction { get; set; }

        public void Update()
        {
            if( UpdateAction != null)
            {
                UpdateAction();
            }
            IsUpdateRequired = false;
        }

        private void OnIsUpdateRequiredChanged()
        {
            if( IsUpdateRequiredChanged != null)
            {
                IsUpdateRequiredChanged(this, EventArgs.Empty);
            }
        }
    }
}
