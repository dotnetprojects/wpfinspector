using System.ComponentModel;
using ChristianMoser.WpfInspector.Services;
using ChristianMoser.WpfInspector.Services.StyleExplorer;
using System.Windows.Data;

namespace ChristianMoser.WpfInspector.UserInterface
{
    public class StyleExplorerViewModel : UpdateTriggeredTreeItemViewModel
    {
        #region Private Members

        private readonly StyleExplorerService _styleExplorerService;

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="StyleExplorerViewModel"/> class.
        /// </summary>
        public StyleExplorerViewModel()
        {
            _styleExplorerService = ServiceLocator.Resolve<StyleExplorerService>();

            StyleItems = new ListCollectionView(_styleExplorerService.StyleItems);
            //StyleItems.GroupDescriptions.Add(new PropertyGroupDescription("Location"));
        }

        #endregion

        /// <summary>
        /// Gets the style items.
        /// </summary>
        public ListCollectionView StyleItems { get; private set; }

        protected override void OnTriggerUpdate()
        {
            _styleExplorerService.UpdateStyle(SelectedTreeItem);
            StyleItems.Refresh();
        }
    }
}
