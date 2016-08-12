using System.ComponentModel;
using ChristianMoser.WpfInspector.Services.DataObjects;
using ChristianMoser.WpfInspector.Services.ElementTree;
using System;

namespace ChristianMoser.WpfInspector.Services.Analyzers
{
    public enum IssueSeverity
    {
        Message,
        Warning,
        Error
    }

    public enum IssueCategory
    {
        Functionality,
        Performance,
        Maintainability
    }

    public class Issue : IEquatable<Issue>
    {
        public IssueSeverity Severity { get; private set; }
        public IssueCategory Category { get; private set; }
        public string Description { get; private set; }
        public string Name { get; private set; }
        public TreeItem TreeItem { get; private set; }
        public PropertyDescriptor Property { get; private set; }

        public Issue(string name, string description, IssueSeverity severity, IssueCategory category, TreeItem treeItem)
        {
            Severity = severity;
            Description = description;
            TreeItem = treeItem;
            Category = category;
            Name = name;
        }

        public Issue(string name, string description, IssueSeverity severity, IssueCategory category, TreeItem treeItem, PropertyDescriptor property)
        {
            Severity = severity;
            Description = description;
            TreeItem = treeItem;
            Category = category;
            Property = property;
            Name = name;
        }

        #region IEquatable<Issue> Members

        public bool Equals(Issue other)
        {
            return other.Name == Name &&
                (other.Property != null ? other.Property.Name : string.Empty) == (Property != null ? Property.Name : string.Empty) &&
                other.TreeItem == TreeItem;
        }

        #endregion
    }
}
