using CommunityToolkit.Mvvm.ComponentModel;
using DevMeter.Core.Github.Models;
using DevMeter.Core.Processing.Formatting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevMeter.UI.ViewModels
{
    internal partial class TotalCommitsViewModel : ViewModelBase
    {

        [ObservableProperty]
        private string _totalCommits;

        [ObservableProperty]
        private string _commitsInLast30Days;

        public TotalCommitsViewModel()
        {
            TotalCommits = string.Empty;
            CommitsInLast30Days = string.Empty;
        }

        public void Update(string totalCommits, int recentCommits)
        {
            TotalCommits = totalCommits;
            CommitsInLast30Days = $"+{StringFormatting.CommaString(recentCommits)} in the last 30 days";
        }

    }
}
