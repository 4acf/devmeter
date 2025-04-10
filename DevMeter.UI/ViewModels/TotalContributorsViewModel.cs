using CommunityToolkit.Mvvm.ComponentModel;
using DevMeter.Core.Models;
using DevMeter.Core.Processing.Formatting;

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
