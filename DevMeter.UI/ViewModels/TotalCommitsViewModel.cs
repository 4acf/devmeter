using CommunityToolkit.Mvvm.ComponentModel;
using DevMeter.Core.Github.Models;
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

        public void Update(int recentCommits)
        {
            CommitsInLast30Days = $"+{String.Format($"{recentCommits:n0}")} in the last 30 days";
        }

    }
}
