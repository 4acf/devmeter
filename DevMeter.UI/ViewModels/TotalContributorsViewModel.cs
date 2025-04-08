using CommunityToolkit.Mvvm.ComponentModel;
using DevMeter.Core.Processing;
using DevMeter.Core.Processing.Formatting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevMeter.UI.ViewModels
{
    internal partial class TotalContributorsViewModel : ViewModelBase
    {

        [ObservableProperty]
        private string _totalContributors;

        [ObservableProperty]
        private string _averageContributions;

        public TotalContributorsViewModel()
        {
            _totalContributors = string.Empty;
            _averageContributions = string.Empty;
        }

        public void Update(HtmlData htmlData)
        {
            TotalContributors = htmlData.Contributors;
            AverageContributions = $"Average Contributions: {StringFormatting.DivideStrings(htmlData.Commits, htmlData.Contributors)}";
        }

    }
}
