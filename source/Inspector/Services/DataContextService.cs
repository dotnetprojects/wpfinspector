using System;
using ChristianMoser.WpfInspector.Services.DataObjects;
using ChristianMoser.WpfInspector.Services.ElementTree;
using ChristianMoser.WpfInspector.Utilities;
using System.Windows;
using System.Windows.Threading;

namespace ChristianMoser.WpfInspector.Services
{
    public class DataContextService
    {
        #region Private Members

        private readonly DataContextInfo _dataContextInfo = new DataContextInfo();

        #endregion

        public DataContextInfo DataContextInfo
        {
            get { return _dataContextInfo; }
        }

        public void UpdateDataContext(TreeItem treeItem)
        {
            if (treeItem != null)
            {
                var fe = treeItem.Instance as FrameworkElement;
                if (fe != null && fe.DataContext != null)
                {
                    _dataContextInfo.DataContext = fe.DataContext;
                    return;
                }
            }
            _dataContextInfo.DataContext = null;
        }

    }
}
