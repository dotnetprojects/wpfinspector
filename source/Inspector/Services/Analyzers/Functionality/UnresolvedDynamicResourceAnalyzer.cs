using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using ChristianMoser.WpfInspector.Services.DataObjects;
using ChristianMoser.WpfInspector.Services.ElementTree;

namespace ChristianMoser.WpfInspector.Services.Analyzers.Functionality
{
    public class UnresolvedDynamicResourceAnalyzer : AnalyzerBase
    {
        #region IAnalyzer Members

        /// <summary>
        /// See <see cref="IAnalyzer.Name" />
        /// </summary>
        public override string Name
        {
            get { return "Unresolved Dynamic Resources"; }
        }

        public override AnalyzerCategory Category
        {
            get { return AnalyzerCategory.Functionality; }
        }

        public override string Description
        {
            get { return "Searches for {DynamicResources} that could not be resolved."; }
        }

        protected override IList<Issue> AnalyzeInternal(TreeItem treeItem)
        {
            var issues = new List<Issue>();
            var frameworkElement = treeItem.Instance as FrameworkElement;
            if (frameworkElement == null)
            {
                return issues;
            }

            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(frameworkElement.GetType()))
            {
                var dpd = DependencyPropertyDescriptor.FromProperty(property);
                if (dpd != null)
                {
                    var localValue = frameworkElement.ReadLocalValue(dpd.DependencyProperty);
                    if (localValue != null)
                    {
                        var localValueType = localValue.GetType();
                        if (localValueType.Name == "ResourceReferenceExpression")
                        {
                            var fieldInfo = localValueType.GetField("_resourceKey", BindingFlags.NonPublic | BindingFlags.Instance);
                            var resourceKey = fieldInfo.GetValue(localValue);

                            if (resourceKey != null)
                            {
                                var resource = frameworkElement.TryFindResource(resourceKey);
                                if( resource == null)
                                {
                                    issues.Add(
                                        new Issue("Resource not resolved",
                                            string.Format("The resource '{0}' on property '{1}' could not be resolved.",
                                                          resourceKey, dpd.DisplayName), IssueSeverity.Error,
                                            IssueCategory.Functionality, treeItem, dpd));
                                }
                            }
                        }
                    }
                   
                }
            }

            return issues;
        }

        #endregion
    }
}

