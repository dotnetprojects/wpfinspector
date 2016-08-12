using ChristianMoser.WpfInspector.Services;
using ChristianMoser.WpfInspector.Services.ElementTree;
using ChristianMoser.WpfInspector.Services.Xaml;
using ChristianMoser.WpfInspector.Utilities;
using System.ComponentModel;

namespace ChristianMoser.WpfInspector.UserInterface
{
    public class XamlViewModel : INotifyPropertyChanged
    {
        #region Private Members

        private string _xaml;
        private readonly XamlSourceService _xamlSourceService;

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="XamlViewModel"/> class.
        /// </summary>
        public XamlViewModel()
        {
            var selectedElementTreeService = ServiceLocator.Resolve<SelectedTreeItemService>();
            _xamlSourceService = ServiceLocator.Resolve<XamlSourceService>();
            selectedElementTreeService.SelectedTreeItemChanged += OnSelectedTreeItemChanged;
        }

        #endregion

        /// <summary>
        /// Gets the xaml source
        /// </summary>
        public string Xaml
        {
            get { return _xaml; }
            private set
            {
                PropertyChanged.ChangeAndNotify(ref _xaml, value, this, "Xaml");
            }
        }

        #region INotifyPropertyChanged Members

        /// <summary>
        /// See <see cref="INotifyPropertyChanged.PropertyChanged" />
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Private Methods

        private void OnSelectedTreeItemChanged(object sender, EventArgs<TreeItem> e)
        {
            Xaml = _xamlSourceService.GetXamlSource(e.Data);
        }

        #endregion

    }
}

