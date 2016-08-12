using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ChristianMoser.WpfInspector.Services.DataObjects;
using ChristianMoser.WpfInspector.Services.ElementTree;

namespace ChristianMoser.WpfInspector.Services.Analyzers
{
    public enum AnalyzerCategory
    {
        Functionality,
        Performance,
        Maintainability
    }

    public abstract class AnalyzerBase
    {
        public bool IsActive { get; set; }

        protected AnalyzerBase()
        {
            IsActive = true;
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Gets the category.
        /// </summary>
        public abstract AnalyzerCategory Category { get; }

        /// <summary>
        /// Gets the description.
        /// </summary>
        public abstract string Description { get; }

        /// <summary>
        /// Analyzes the specified element for issues.
        /// </summary>
        public virtual void Analyze(TreeItem treeItem, AnalyzerContext analyzerContext)
        {
            analyzerContext.UpdateIssues(this,treeItem, AnalyzeInternal(treeItem));
        }

        protected virtual IList<Issue> AnalyzeInternal(TreeItem treeItem)
        {
            return new List<Issue>();
        }
    }
}
