using System.Collections.Generic;
using System.Collections.ObjectModel;
using Obfucsation.Controls.Analysis;
using Obfucsation.Controls.Obfuscation;
using Obfucsation.Core;

namespace Obfucsation
{
    public class MainViewModel
    {
        public ICollection<TabViewModel> Tabs { get; }

        public MainViewModel()
        {
            Tabs = new ObservableCollection<TabViewModel>();
            Tabs.Add(new ObfuscationViewModel());
            Tabs.Add(new AnalysisViewModel());
        }
    }
}