using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevMeter.UI.ViewModels
{
    public partial class TotalCommitsViewModel : ViewModelBase
    {

        [ObservableProperty]
        private string _totalCommits;

        [ObservableProperty]
        private string _commitsInLast30Days;

        public TotalCommitsViewModel()
        {
            TotalCommits = "-";
            CommitsInLast30Days = "-";
        }

    }
}
