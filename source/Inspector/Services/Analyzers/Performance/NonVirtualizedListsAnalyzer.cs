using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using ChristianMoser.WpfInspector.Services.DataObjects;
using System.Windows.Controls;
using ChristianMoser.WpfInspector.Services.ElementTree;

namespace ChristianMoser.WpfInspector.Services.Analyzers.Performance
{
    public class NonVirtualizedListsAnalyzer : AnalyzerBase
    {
        #region IAnalyzer Members

        /// <summary>
        /// See <see cref="AnalyzerBase.Name" />
        /// </summary>
        public override string Name
        {
            get { return "Non virtualized long lists"; }
        }

        /// <summary>
        /// See <see cref="AnalyzerBase.Category" />
        /// </summary>
        public override AnalyzerCategory Category
        {
            get { return AnalyzerCategory.Performance; }
        }

        /// <summary>
        /// See <see cref="AnalyzerBase.Description" />
        /// </summary>
        public override string Description
        {
            get { return "Warns if a collection with more than 50 entries is not virtualized."; }
        }

        /// <summary>
        /// See <see cref="AnalyzerBase.Analyze" />
        /// </summary>
        protected override IList<Issue> AnalyzeInternal(TreeItem treeItem)
        {
            var issues = new List<Issue>();
            var itemsControl = treeItem.Instance as ItemsControl;
            if (itemsControl == null)
            {
                return issues;
            }

            if( itemsControl.Items.Count > 50 )
            {
                if (itemsControl.Template.VisualTree == null)
                {
                    var panel = FindItemsPanel<Panel>(itemsControl);
                    if( !(panel is VirtualizingPanel))
                    {
                        issues.Add(
                            new Issue( "Virualize list",
                                string.Format("List with {0} entries should be virtualized", itemsControl.Items.Count),
                                IssueSeverity.Warning, IssueCategory.Performance, treeItem));
                    }
                }    

            }

            return issues;
        }

        public T FindItemsPanel<T>(ItemsControl itemsControl)
    where T : Panel
        {
            var p = new Point(itemsControl.ActualWidth / 2, itemsControl.ActualHeight / 2);
            HitTestResult result = VisualTreeHelper.HitTest(itemsControl, p);

            if (result == null)
            {
                return null;
            }

            DependencyObject visual = result.VisualHit;
            while (!(visual is T))
            {
                visual = VisualTreeHelper.GetParent(visual);
            }

            return visual as T;
        }

        #endregion
    }
}
