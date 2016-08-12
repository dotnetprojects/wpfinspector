using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using ChristianMoser.WpfInspector.Services.DataObjects;
using System.Windows.Media;
using ChristianMoser.WpfInspector.Services.ElementTree;

namespace ChristianMoser.WpfInspector.Services.Analyzers.Maintainability
{
    public class LocalBrushDefinitionsAnalyzer : AnalyzerBase
    {
        #region IAnalyzer Members

        public override string Name
        {
            get { return "Local definition of brushes"; }
        }

        public override AnalyzerCategory Category
        {
            get { return AnalyzerCategory.Maintainability; }
        }

        public override string Description
        {
            get { return "Searches for locally defined brushes."; }
        }

        protected override IList<Issue> AnalyzeInternal(TreeItem treeItem)
        {
            var issues = new List<Issue>();
            var dependencyObject = treeItem.Instance as DependencyObject;
            if (dependencyObject == null)
            {
                return issues;
            }

            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(dependencyObject.GetType()))
            {
                var dpd = DependencyPropertyDescriptor.FromProperty(property);
                if (dpd != null && !dpd.IsReadOnly && dpd.PropertyType == typeof(Brush))
                {
                    var localValue = dependencyObject.ReadLocalValue(dpd.DependencyProperty);
                    var valueSource = DependencyPropertyHelper.GetValueSource(dependencyObject, dpd.DependencyProperty);
                    if (valueSource.BaseValueSource == BaseValueSource.Local && !valueSource.IsExpression)
                    {
                        if (localValue is Brush && localValue != Brushes.Transparent)
                        {
                            issues.Add(
                                new Issue( "LocalBrush",
                                    string.Format(
                                        "Property {0} contains the local brush {1}. Prevent local brushes to keep the design maintainable.",
                                        dpd.DisplayName, localValue), IssueSeverity.Message,
                                    IssueCategory.Maintainability, treeItem, dpd));
                        }
                    }
                }
            }

            return issues;
        }

        #endregion
    }
}
