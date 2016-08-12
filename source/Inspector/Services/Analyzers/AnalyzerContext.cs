using System;
using System.Collections.Generic;
using System.Linq;
using ChristianMoser.WpfInspector.Services.DataObjects;
using ChristianMoser.WpfInspector.Services.ElementTree;

namespace ChristianMoser.WpfInspector.Services.Analyzers
{
    public class AnalyzerContext
    {
        private readonly List<Issue> _issues = new List<Issue>();
        private readonly Dictionary<TreeItem, Dictionary<AnalyzerBase, List<Issue>>> _treeItemBasedIssues = new Dictionary<TreeItem, Dictionary<AnalyzerBase, List<Issue>>>();
        private readonly List<AnalyzerBase> _analyzers = new List<AnalyzerBase>();
        private bool _hasChanges;
        private bool _isSuspended;

        public event EventHandler IssuesChanged;

        public void ClearTreeItemIssues( TreeItem treeItem)
        {
            bool dataChanged = false;
            if( !_treeItemBasedIssues.ContainsKey(treeItem))
            {
                return;
            }

            foreach (var analyzerList in _treeItemBasedIssues[treeItem])
            {
                foreach (var issue in analyzerList.Value)
                {
                    _issues.Remove(issue);
                    dataChanged = true;
                }    
            }

            _treeItemBasedIssues.Remove(treeItem);

            if (dataChanged)
            {
                NotifyIssuesChanged();
            }
        }

        public void UpdateIssues(AnalyzerBase analyzer, TreeItem treeItem, IList<Issue> issues)
        {
            bool hasChanges = false;

            if( !_treeItemBasedIssues.ContainsKey(treeItem))
            {
                _treeItemBasedIssues.Add(treeItem, new Dictionary<AnalyzerBase, List<Issue>>());
            }

            if( !_treeItemBasedIssues[treeItem].ContainsKey(analyzer))
            {
                _treeItemBasedIssues[treeItem].Add(analyzer, new List<Issue>());
            }

            var add = new List<Issue>();
            var remove = new List<Issue>(_treeItemBasedIssues[treeItem][analyzer]);

            foreach (var issue in issues)
            {
                // Still there
                if( remove.Contains(issue))
                {
                    remove.Remove(issue);
                }
                else
                {
                    add.Add(issue);
                }
            }

            foreach (var issue in remove)
            {
                _treeItemBasedIssues[treeItem][analyzer].Remove(issue);
                _issues.Remove(issue);
                hasChanges = true;
            }

            foreach (var issue in add)
            {
                _treeItemBasedIssues[treeItem][analyzer].Add(issue);
                _issues.Add(issue);
                hasChanges = true;
            }
            
            if( hasChanges)
            {
                NotifyIssuesChanged();
            }
        }

        public List<Issue> Issues
        {
            get { return _issues; }
        }

        public List<AnalyzerBase> Analyzers
        {
            get { return _analyzers; }
        }

        public void SuspendEvents()
        {
            _isSuspended = true;
        }

        public void ResumeEvents()
        {
            _isSuspended = false;
            if( _hasChanges )
            {
                _hasChanges = false;
                NotifyIssuesChanged();
            }
        }

        public void Analyze(TreeItem treeItem)
        {
            foreach (var analyzer in _analyzers)
            {
                if (analyzer.IsActive)
                {
                    analyzer.Analyze(treeItem, this);
                }
                else
                {
                    // Clear the existing issues
                    UpdateIssues(analyzer, treeItem, new List<Issue>());
                }
            }
        }

        private void NotifyIssuesChanged()
        {
            if (_isSuspended)
            {
                _hasChanges = true;
                return;
            }

            if( IssuesChanged != null)
            {
                IssuesChanged(this, EventArgs.Empty);
            }
        }
       
    }
}
