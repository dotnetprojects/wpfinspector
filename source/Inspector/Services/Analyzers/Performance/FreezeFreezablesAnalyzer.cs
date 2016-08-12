using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ChristianMoser.WpfInspector.Services.DataObjects;
using System.Windows;
using ChristianMoser.WpfInspector.Services.ElementTree;

namespace ChristianMoser.WpfInspector.Services.Analyzers.Performance
{
    public class FreezeFreezablesAnalyzer : AnalyzerBase
    {
        private readonly List<object> _reportedIssues = new List<object>();

        public override string Name
        {
            get { return "Freeze freezables"; }
        }

        public override AnalyzerCategory Category
        {
            get { return AnalyzerCategory.Performance; }
        }

        public override string Description
        {
            get { return "You should freeze Freezabled to save memory and increase performance."; }
        }

        protected override IList<Issue> AnalyzeInternal(TreeItem treeItem)
        {
            var issues = new List<Issue>();
            var fe = treeItem.Instance as FrameworkElement;
            if( fe != null)
            {
                AnalyzeResorucesRecursive(issues, fe.Resources, treeItem);
            }
            return issues;
        }

        private void AnalyzeResorucesRecursive(List<Issue> issues, ResourceDictionary dictionary, TreeItem treeItem)
        {
            foreach (var resourceDictionary in dictionary.MergedDictionaries)
            {
                AnalyzeResorucesRecursive(issues, resourceDictionary, treeItem);
            }
            CheckResources(issues,dictionary, treeItem);
        }

        private void CheckResources(List<Issue> issues, ResourceDictionary dictionary, TreeItem treeItem)
        {
            foreach (object resourceKey in dictionary.Keys)
            {
                var resource = dictionary[resourceKey];
                if (resource is Freezable && !((Freezable)resource).IsFrozen && !_reportedIssues.Contains(resourceKey))
                {
                    _reportedIssues.Add(resourceKey);
                    issues.Add(
                        new Issue( "Freeze Freezables",
                            string.Format("Freezing the resource {0} can save memory and increase performance.",
                                          resourceKey), IssueSeverity.Message, IssueCategory.Performance, treeItem));
                }
            }
        }
    }
}
