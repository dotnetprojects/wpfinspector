using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using ChristianMoser.WpfInspector.Services;
using System.Windows.Data;
using System.ComponentModel;
using ChristianMoser.WpfInspector.Services.Analyzers;
using ChristianMoser.WpfInspector.Services.DataObjects;
using ChristianMoser.WpfInspector.Services.ElementTree;
using ChristianMoser.WpfInspector.Utilities;

namespace ChristianMoser.WpfInspector.UserInterface
{
    public class IssuesListViewModel : INotifyPropertyChanged
    {
        #region Private Members

        private readonly IssuesAnalyzerService _issuesAnalyzerService;
        private readonly SelectedTreeItemService _selectedTreeItemService;

        private bool _isShowErrors = true;
        private bool _isShowWarnings = true;
        private bool _isShowMessages;

        private int _warnings;
        private int _errors;
        private int _messages;

        #endregion

        #region Construction

        public IssuesListViewModel()
        {
            _selectedTreeItemService = ServiceLocator.Resolve<SelectedTreeItemService>();
            _issuesAnalyzerService = ServiceLocator.Resolve<IssuesAnalyzerService>();
            Issues = new ListCollectionView(_issuesAnalyzerService.AnalyzerContext.Issues);
            Issues.SortDescriptions.Add(new SortDescription("Severity", ListSortDirection.Descending));
            Issues.Filter = FilterIssues;
            _issuesAnalyzerService.AnalyzerContext.IssuesChanged += (s, e) => IssuesChanged();

            ConfigureAnalyzersCommand = new Command<object>( o => new AnalyzerConfigurationWindow().Show());
        }

        private void IssuesChanged()
        {
            int warnings = 0;
            int errors = 0;
            int messages = 0;

            foreach (Issue issue in _issuesAnalyzerService.AnalyzerContext.Issues)
            {
                if (issue.Severity == IssueSeverity.Message)
                    messages++;
                if (issue.Severity == IssueSeverity.Warning)
                    warnings++;
                if (issue.Severity == IssueSeverity.Error)
                    errors++;
            }

            Errors = errors;
            Warnings = warnings;
            Messages = messages;

            Issues.Refresh();
        }

        #endregion

        public ICommand ConfigureAnalyzersCommand { get; private set; }

        public bool IsShowErrors
        {
            get { return _isShowErrors; }
            set
            {
                PropertyChanged.ChangeAndNotify(ref _isShowErrors, value, this, "IsShowErrors");
                IssuesChanged();
            }
        }

        public bool IsShowWarnings
        {
            get { return _isShowWarnings; }
            set
            {
                PropertyChanged.ChangeAndNotify(ref _isShowWarnings, value, this, "IsShowWarnings");
                IssuesChanged();
            }
        }

        public bool IsShowMessages
        {
            get { return _isShowMessages; }
            set
            {
                PropertyChanged.ChangeAndNotify(ref _isShowMessages, value, this, "IsShowMessages");
                IssuesChanged();
            }
        }

        public int Errors
        {
            get { return _errors; }
            set
            {
                PropertyChanged.ChangeAndNotify(ref _errors, value, this, "Errors");
            }
        }
        
        public int Warnings
        {
            get { return _warnings; }
            set
            {
                PropertyChanged.ChangeAndNotify(ref _warnings, value, this, "Warnings");
            }
        }
        
        public int Messages
        {
            get { return _messages; }
            set
            {
                PropertyChanged.ChangeAndNotify(ref _messages, value, this, "Messages");
            }
        }

        public void OnIssueSelected( Issue issue)
        {
            if (issue.TreeItem != null)
            {
                var treeItem = _selectedTreeItemService.FindStrategyNearItem(issue.TreeItem);

                var searchStrategy = treeItem is LogicalTreeItem
                                         ? SearchStrategy.LogicalTree
                                         : SearchStrategy.VisualTree;
                _selectedTreeItemService.SetSearchStrategy(searchStrategy);
                if (treeItem != null)
                {
                    treeItem.BringIntoView();
                }
            }

            _issuesAnalyzerService.OnIssueSelected(issue);
        }

        public ICollectionView Issues { get; private set; }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Private Methods

        private bool FilterIssues( object item)
        {
            var issue = item as Issue;
            if( issue == null)
            {
                return false;
            }

            return issue.Severity == IssueSeverity.Error && IsShowErrors ||
                   issue.Severity == IssueSeverity.Warning && IsShowWarnings ||
                   issue.Severity == IssueSeverity.Message && IsShowMessages;
        }

        #endregion
    }
}
