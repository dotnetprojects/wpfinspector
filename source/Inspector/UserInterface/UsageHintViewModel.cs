using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;

namespace ChristianMoser.WpfInspector.UserInterface
{
    public class UsageHintViewModel
    {
        #region Private Members

        private bool _dontShowHintAgaing;

        #endregion

        /// <summary>
        /// Gets or sets a value indicating whether [dont show hint againg].
        /// </summary>
        /// <value><c>true</c> if [dont show hint againg]; otherwise, <c>false</c>.</value>
        public bool DontShowHintAgaing
        {
            get { return _dontShowHintAgaing; }
            set
            {
                _dontShowHintAgaing = value;
                UpdateRegistry();
            }
        }

        private void UpdateRegistry()
        {
            try
            {
                Registry.SetValue(@"HKEY_CURRENT_USER\Software\WPFInspector","DontShowUsageHint", "true");
            }
            catch (Exception)
            {
            }
        }
    }
}
