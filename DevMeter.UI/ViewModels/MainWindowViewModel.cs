using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DevMeter.Core.Github;
using DevMeter.Core.Github.Models.Json;
using DevMeter.Core.Models;
using DevMeter.Core.Processing;
using DevMeter.Core.Utils;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevMeter.UI.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {

        [ObservableProperty]
        private string? _searchString;

        [ObservableProperty]
        private string? _statusMessage;

        [ObservableProperty]
        private string _statusColor;

        [ObservableProperty]
        private string _repoName;

        [ObservableProperty]
        private bool _isLoading;

        private readonly GitHubClient _gitHubClient;

        internal TotalLinesViewModel TotalLinesViewModel { get; }
        internal TotalCommitsViewModel TotalCommitsViewModel { get; }
        internal TotalContributorsViewModel TotalContributorsViewModel { get; }
        internal LanguageBreakdownViewModel LanguageBreakdownViewModel { get; }
        internal RecentActivityViewModel RecentActivityViewModel { get; }
        internal LargestFilesViewModel LargestFilesViewModel { get; }
        internal TopContributorsViewModel TopContributorsViewModel { get; }

        public MainWindowViewModel()
        {
            StatusColor = Colors.Status;
            RepoName = "-";
            IsLoading = false;
            _gitHubClient = new GitHubClient(App.Configuration?["PAT"]);
            TotalLinesViewModel = new TotalLinesViewModel();
            TotalCommitsViewModel = new TotalCommitsViewModel();
            TotalContributorsViewModel = new TotalContributorsViewModel();
            LanguageBreakdownViewModel = new LanguageBreakdownViewModel();
            RecentActivityViewModel = new RecentActivityViewModel();
            LargestFilesViewModel = new LargestFilesViewModel();
            TopContributorsViewModel = new TopContributorsViewModel();
        }

        private bool DataCollectionFailed<T>(Result<T>? obj)
        {
            if(obj == null)
            {
                IsLoading = false;
                StatusMessage = Errors.Unexpected;
                StatusColor = Colors.Error;
                return true;
            }
            if (!obj.Succeeded)
            {
                IsLoading = false;
                StatusMessage = obj.ErrorMessage;
                StatusColor = Colors.Error;
                return true;
            }
            return false;
        }

        private bool IsResultValueNull<T>(T? obj) where T : class
        {
            if(obj == null)
            {
                IsLoading = false;
                StatusMessage = Errors.Unexpected;
                StatusColor = Colors.Error;
                return true;
            }
            return false;
        }

        [RelayCommand]
        private async Task Search()
        {

            IsLoading = true;
            StatusColor = Colors.Status;

            if (!InputParser.TryParse(SearchString, out var result))
            {
                StatusMessage = result;
                StatusColor = Colors.Error;
                return;
            }
            var repoHandle = result;

            //each request is done sequentially to handle errors and rate limits 
            //https://docs.github.com/en/rest/using-the-rest-api/best-practices-for-using-the-rest-api?apiVersion=2022-11-28#avoid-concurrent-requests

            var dataCollector = new DataCollector(_gitHubClient, repoHandle);

            //get number of commits + number of contributors
            StatusMessage = "Fetching HTML data...";
            var htmlDataResult = await dataCollector.GetHtmlData();
            if (DataCollectionFailed<HtmlData>(htmlDataResult))
            {
                return;
            }
            var htmlData = htmlDataResult.Value;
            if (IsResultValueNull<HtmlData>(htmlData))
            {
                return;
            }

            //get handles of the top contributors
            StatusMessage = "Fetching top contributors...";
            var topContributorsResult = await dataCollector.GetTopContributorData();
            if (DataCollectionFailed<List<Contributor>>(topContributorsResult))
            {
                return;
            }
            var topContributors = topContributorsResult.Value;
            if (IsResultValueNull<List<Contributor>>(topContributors))
            {
                return;
            }

            //commits in last 30 days
            StatusMessage = "Fetching commits in last 30 days...";
            var recentCommitsResult = await dataCollector.GetRecentCommitsData();
            if (DataCollectionFailed<List<GitHubCommit>>(recentCommitsResult))
            {
                return;
            }
            var recentCommits = recentCommitsResult.Value;
            if (IsResultValueNull<List<GitHubCommit>>(recentCommits))
            {
                return;
            }

            //get data from linguist for language breakdown
            StatusMessage = "Fetching language data...";
            var linguistResult = await dataCollector.GetLinguistData();
            if (DataCollectionFailed<Dictionary<string, long>>(linguistResult))
            {
                return;
            }
            var languages = linguistResult.Value;
            if (IsResultValueNull<Dictionary<string, long>>(languages))
            {
                return;
            }

            //reading file tree (slow!)
            StatusMessage = "Reading file tree... (this may take a while)";
            var fileTreeResult = await dataCollector.GetRootFolderContents();
            if (DataCollectionFailed<Folder>(fileTreeResult))
            {
                return;
            }
            var fileTree = fileTreeResult.Value;
            if (IsResultValueNull<Folder>(fileTree))
            {
                return;
            }

            //traverse tree to get largest files
            StatusMessage = "Analyzing file sizes";
            var largestFilesByLinesHeap = new PriorityQueue<File, int>();
            dataCollector.GetLargestFiles(fileTree!, largestFilesByLinesHeap);

            IsLoading = false;
            StatusMessage = string.Empty;

            //batch update ui
            RepoName = repoHandle.Substring(1);
            TotalLinesViewModel.Update(fileTree!.LinesOfCode, fileTree!.LinesOfWhitespace);
            TotalCommitsViewModel.Update(htmlData!.Commits, recentCommits!.Count);
            TotalContributorsViewModel.Update(htmlData!);
            LanguageBreakdownViewModel.Update(languages!);
            RecentActivityViewModel.Update(recentCommits!);
            LargestFilesViewModel.Update(largestFilesByLinesHeap!);
            TopContributorsViewModel.Update(topContributors!);

        }

    }
}
