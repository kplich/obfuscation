using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Obfucsation.MVVM.ViewModel
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