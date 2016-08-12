using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup.Primitives;
using ChristianMoser.WpfInspector.Services.DataObjects;
using System.Windows.Controls;
using System.Windows.Markup;
using ChristianMoser.WpfInspector.Services.ElementTree;

namespace ChristianMoser.WpfInspector.Services.Analyzers.Performance
{
    public class UnnecessaryNestedPanelsAnalyzer : AnalyzerBase
    {
        public override string Name
        {
            get { return "Unnecessary nested panels"; }
        }

        public override AnalyzerCategory Category
        {
            get { return AnalyzerCategory.Performance; }
        }

        public override string Description
        {
            get { return "Searches for unnecessary nested elements."; }
        }

        protected override IList<Issue> AnalyzeInternal(TreeItem treeItem)
        {
            var issues = new List<Issue>();
            var markupObject = MarkupWriter.GetMarkupObjectFor(treeItem.Instance);
            if ( treeItem.Instance is Grid &&  treeItem.Children.Count <= 1 && markupObject.Properties.Count() == 0)
            {
                issues.Add(
                    new Issue( "RedundantPanel","This panel seems to be redundant and can be removed. This reduces the tree dept and improves performance",
                        IssueSeverity.Warning, IssueCategory.Performance, treeItem));
            }
            return issues;
        }
    }
}
