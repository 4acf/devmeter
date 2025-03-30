using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace devmeter.ui.ViewModels
{
    public partial class TotalCommitsViewModel : ViewModelBase
    {

        [ObservableProperty]
        private string _totalCommits;

        public TotalCommitsViewModel()
        {
            TotalCommits = "-";
        }

    }
}
