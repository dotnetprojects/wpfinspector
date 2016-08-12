using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using ChristianMoser.WpfInspector.Services;
using System.ComponentModel;
using ChristianMoser.WpfInspector.Services.Analyzers;

namespace ChristianMoser.WpfInspector.UserInterface
{
    public class AnalyzerConfigurationViewModel
    {
        private readonly IssuesAnalyzerService _issuesAnalyzer;

        public AnalyzerConfigurationViewModel()
        {
            _issuesAnalyzer = ServiceLocator.Resolve<IssuesAnalyzerService>();
            Analyzers = new ListCollectionView(_issuesAnalyzer.AnalyzerContext.Analyzers);
            Analyzers.GroupDescriptions.Add(new PropertyGroupDescription("Category"));
            Analyzers.MoveCurrentToFirst();
        }

        public ICollectionView Analyzers { get; private set; }

        public void ReAnalyze()
        {
            _issuesAnalyzer.Analyze();
        }
    }
}
