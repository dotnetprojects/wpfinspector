using System;
using ChristianMoser.WpfInspector.Services.Analyzers.Functionality;
using ChristianMoser.WpfInspector.Services.Analyzers.Maintainability;
using ChristianMoser.WpfInspector.Services.Analyzers.Performance;
using ChristianMoser.WpfInspector.Services.ElementTree;
using ChristianMoser.WpfInspector.Utilities;
using System.Windows.Threading;

namespace ChristianMoser.WpfInspector.Services.Analyzers
{
    public class IssuesAnalyzerService
    {
        #region Private Members

        private readonly AnalyzerContext _analyzerContext = new AnalyzerContext();
        private readonly TreeElementService _rootElementService;
        private readonly SelectedTreeItemService _selectedTreeItemService;
        private readonly DispatcherTimer _analyzerTimer;

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="IssuesAnalyzerService"/> class.
        /// </summary>
        public IssuesAnalyzerService()
        {
            LoadAnalyzers();

            _rootElementService = ServiceLocator.Resolve<VisualTreeService>();
            _selectedTreeItemService = ServiceLocator.Resolve<SelectedTreeItemService>();

            _analyzerTimer = new DispatcherTimer(DispatcherPriority.ContextIdle);
            _analyzerTimer.Tick += (s, e) => Analyze();
            _analyzerTimer.Interval = TimeSpan.FromSeconds(2);
            _analyzerTimer.Start();
        }

        #endregion

        public event EventHandler<EventArgs<Issue>> IssueSelected;

        public AnalyzerContext AnalyzerContext
        {
            get { return _analyzerContext; }
        }

        public void Analyze()
        {
            foreach (var visualRoot in _rootElementService.Elements)
            {
                visualRoot.Analyze(AnalyzerContext);
            }
        }

        public void OnIssueSelected( Issue issue)
        {
            if( IssueSelected != null)
            {
                IssueSelected(this, new EventArgs<Issue>{ Data = issue});
            }
            
            if( issue.TreeItem is VisualTreeItem)
            {
                _selectedTreeItemService.SelectedVisualTreeItem = issue.TreeItem;
            }
            else
            {
                _selectedTreeItemService.SelectedLogicalTreeItem = issue.TreeItem;
            }
            
        }

        #region Private Methods

        private void LoadAnalyzers()
        {
            AnalyzerContext.Analyzers.Add(new DataBindingErrorAnalyzer());
            AnalyzerContext.Analyzers.Add(new NonVirtualizedListsAnalyzer());
            AnalyzerContext.Analyzers.Add(new UnresolvedDynamicResourceAnalyzer());
            AnalyzerContext.Analyzers.Add(new LocalBrushDefinitionsAnalyzer());
            AnalyzerContext.Analyzers.Add(new FreezeFreezablesAnalyzer());
            //AnalyzerContext.Analyzers.Add(new UnnecessaryNestedPanelsAnalyzer());
        }

        #endregion

    }
}
