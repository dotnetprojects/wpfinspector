using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using ChristianMoser.WpfInspector.Utilities;

namespace ChristianMoser.WpfInspector.Services.DataObjects
{
    public class DataContextInfo : INotifyPropertyChanged
    {
        private object _dataContext;

        public bool IsNull
        {
            get { return DataContext == null; }
        }

        public object DataContext
        {
            get { return _dataContext; }
            set 
            { 
                PropertyChanged.ChangeAndNotify(ref _dataContext, value, this, "DataContext");
                PropertyChanged.Notify(this, "IsNull");
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
