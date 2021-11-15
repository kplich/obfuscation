using System.Collections.Generic;
using System.Collections.ObjectModel;
using Obfuscation.Controls.Analysis;
using Obfuscation.Controls.Obfuscation;
using Obfuscation.Core;

namespace Obfuscation
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