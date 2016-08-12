using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Threading;
using ChristianMoser.WpfInspector.Services.ElementTree;

namespace ChristianMoser.WpfInspector.Services.Analyzers.Functionality
{
    internal class DataBindingCheckCallback
    {
        public bool Done { get; set; }
        public Action<Issue> DoneAction { get; set; }
        public Issue Issue { get; set; }
        
        public void SetDone()
        {
            Done = true;
            DoneAction(Issue);
        }

    }

    internal class DataBindingBackgroundHelper
    {
        private readonly List<Issue> _issues = new List<Issue>();
        private readonly AnalyzerContext _context;
        private readonly AnalyzerBase _analyzer;
        private readonly TreeItem _treeItem;
        private readonly Action _doneAction;

        private readonly List<DataBindingCheckCallback> _pendingChecks = new List<DataBindingCheckCallback>();

        public DataBindingCheckCallback CreateCallback()
        {
            var callback = new DataBindingCheckCallback {DoneAction = AddIssueAndCheck};
            _pendingChecks.Add(callback);
            return callback;
        }

        public DataBindingBackgroundHelper(AnalyzerBase analyzer, TreeItem treeItem, AnalyzerContext context, Action doneAction)
        {
            _context = context;
            _analyzer = analyzer;
            _treeItem = treeItem;
            _doneAction = doneAction;
        }

        private void AddIssueAndCheck(Issue issue)
        {
            if (issue != null)
            {
                _issues.Add(issue);
            }
            ReportIssues();
        }

        public void ReportIssues()
        {
            if (!_pendingChecks.Any(c => c.Done == false))
            {
                _context.UpdateIssues(_analyzer, _treeItem, _issues);
                _doneAction();
            }
        }
    }

    public class DataBindingErrorAnalyzer : AnalyzerBase
    {
        private readonly Dictionary<TreeItem, DataBindingBackgroundHelper> _pendingTreeItems = new Dictionary<TreeItem, DataBindingBackgroundHelper>();

        #region IAnalyzer Members

        /// <summary>
        /// See <see cref="AnalyzerBase.Name" />
        /// </summary>
        public override string Name
        {
            get { return "DataBinding Errors"; }
        }

        /// <summary>
        /// See <see cref="AnalyzerBase.Category" />
        /// </summary>
        public override AnalyzerCategory Category
        {
            get { return AnalyzerCategory.Functionality; }
        }

        /// <summary>
        /// See <see cref="AnalyzerBase.Description" />
        /// </summary>
        public override string Description
        {
            get { return "Checks all data bindings for errors."; }
        }

        /// <summary>
        /// See <see cref="AnalyzerBase.Analyze" />
        /// </summary>
        public override void Analyze(TreeItem treeItem, AnalyzerContext analyzerContext)
        {
            PresentationTraceSources.SetTraceLevel(treeItem.Instance, PresentationTraceLevel.High);
            var dependencyObject = treeItem.Instance as DependencyObject;
            if (dependencyObject == null)
            {
                return;
            }

            if( _pendingTreeItems.ContainsKey(treeItem))
                return;
            
            var backgroundHelper = new DataBindingBackgroundHelper(this, treeItem, analyzerContext, () => _pendingTreeItems.Remove(treeItem));
            _pendingTreeItems.Add(treeItem, backgroundHelper);

            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(dependencyObject.GetType()))
            {
                var dpd = DependencyPropertyDescriptor.FromProperty(property);
                if (dpd != null)
                {
                    BindingExpressionBase binding = BindingOperations.GetBindingExpressionBase(dependencyObject, dpd.DependencyProperty);
                    if (binding != null)
                    {
                        if (binding.HasError || binding.Status != BindingStatus.Active)
                        {
                            var callback = backgroundHelper.CreateCallback();

                            // Ensure that no pending calls are in the dispatcher queue
                            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.SystemIdle, (Action) delegate
                             {
                                 var stringBuilder = new StringBuilder();
                                 var stringWriter = new StringWriter(stringBuilder);
                                 var listener = new TextWriterTraceListener(stringWriter);
                                 PresentationTraceSources.DataBindingSource.Listeners.Add(listener);
                                 PresentationTraceSources.SetTraceLevel(treeItem.Instance, PresentationTraceLevel.High);

                                 // Remove and add the binding to re-trigger the binding error
                                 dependencyObject.ClearValue(dpd.DependencyProperty);
                                 BindingOperations.SetBinding(dependencyObject, dpd.DependencyProperty, binding.ParentBindingBase);

                                 listener.Flush();
                                 stringWriter.Flush();

                                 Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.SystemIdle, (Action)delegate
                                 {
                                     string bindingError = stringBuilder.ToString();
                                     if (bindingError.Length > 0)
                                     {
                                         int prefix = bindingError.IndexOf(':');
                                         bindingError = bindingError.Substring(prefix + 6).Replace("\r", "").Replace("\n", "");

                                         callback.Issue = new Issue("BindingError", string.Format("{0}: {1}", dpd.DisplayName, bindingError),
                                                                    IssueSeverity.Error, IssueCategory.Functionality,
                                                                    treeItem, dpd);
                                     }
                                     PresentationTraceSources.DataBindingSource.Listeners.Remove(listener);
                                     listener.Close();

                                     callback.SetDone();
                                 });
                             });
                        }
                    }
                }
            }

            backgroundHelper.ReportIssues();
        }

        #endregion

       
    }


}
