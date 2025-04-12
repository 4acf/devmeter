using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DevMeter.Core.Github;
using DevMeter.Core.Github.Models.Json;
using DevMeter.Core.Models;
using DevMeter.Core.Processing;
using DevMeter.Core.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
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

        private CancellationTokenSource _cancellationTokenSource;

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
            _cancellationTokenSource = new CancellationTokenSource();
            TotalLinesViewModel = new TotalLinesViewModel();
            TotalCommitsViewModel = new TotalCommitsViewModel();
            TotalContributorsViewModel = new TotalContributorsViewModel();
            LanguageBreakdownViewModel = new LanguageBreakdownViewModel();
            RecentActivityViewModel = new RecentActivityViewModel();
            LargestFilesViewModel = new LargestFilesViewModel();
            TopContributorsViewModel = new TopContributorsViewModel();
        }

        private async Task<T?> CallDataCollector<T>(Func<Task<Result<T>>> func, string statusMessage)
        {
            try
            {
                StatusMessage = statusMessage;
                var result = await func();
                if(DataCollectionFailed<T>(result))
                {
                    return default;
                }
                if (IsResultValueNull<T>(result.Value))
                {
                    return default;
                }
                return result.Value;
            }
            catch(Exception ex)
            {
                UpdateDisplayToFailState(ex.Message);
                return default;
            }
        }

        private void UpdateDisplayToFailState(string? errorMessage)
        {
            IsLoading = false;
            StatusMessage = errorMessage ?? Errors.Unexpected;
            StatusColor = Colors.Error;

            if(_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Dispose();
                _cancellationTokenSource = new CancellationTokenSource();
            }
        }

        private bool IsResultValueNull<T>(T? obj)
        {
            if (obj == null)
            {
                UpdateDisplayToFailState(null);
                return true;
            }
            return false;
        }

        private bool DataCollectionFailed<T>(Result<T>? obj)
        {
            if(obj == null)
            {
                UpdateDisplayToFailState(null);
                return true;
            }
            if (!obj.Succeeded)
            {
                UpdateDisplayToFailState(obj.ErrorMessage);
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
                IsLoading = false;
                StatusMessage = result;
                StatusColor = Colors.Error;
                return;
            }
            var repoHandle = result;

            //each request is done sequentially to handle errors and rate limits 
            //https://docs.github.com/en/rest/using-the-rest-api/best-practices-for-using-the-rest-api?apiVersion=2022-11-28#avoid-concurrent-requests

            var dataCollector = new DataCollector(_gitHubClient, repoHandle);

            var htmlData = await CallDataCollector(() => dataCollector.GetHtmlData(), "Fetching HTML data...");
            if (htmlData == null) return;

            var topContributors = await CallDataCollector(() => dataCollector.GetTopContributorData(), "Fetching top contributors...");
            if (topContributors == null) return;

            var recentCommits = await CallDataCollector(() => dataCollector.GetRecentCommitsData(), "Fetching commits in last 30 days...");
            if (recentCommits == null) return;

            var languages = await CallDataCollector(() => dataCollector.GetLinguistData(), "Fetching language data...");
            if (languages == null) return;

            var fileTree = await CallDataCollector(() => dataCollector.GetFolderContents(null, _cancellationTokenSource.Token), "Reading file tree... (this may take a while)");
            if (fileTree == null) return;

            //traverse tree to get largest files
            StatusMessage = "Analyzing file sizes";
            var largestFilesByLinesHeap = new PriorityQueue<File, int>();
            try
            {
                dataCollector.GetLargestFiles(fileTree, largestFilesByLinesHeap);
            }
            catch (Exception ex)
            {
                UpdateDisplayToFailState(ex.Message);
                return;
            }

            IsLoading = false;
            StatusMessage = string.Empty;

            //batch update ui
            RepoName = repoHandle.Substring(1);
            TotalLinesViewModel.Update(fileTree.LinesOfCode, fileTree.LinesOfWhitespace);
            TotalCommitsViewModel.Update(htmlData.Commits, recentCommits.Count);
            TotalContributorsViewModel.Update(htmlData);
            LanguageBreakdownViewModel.Update(languages);
            RecentActivityViewModel.Update(recentCommits);
            LargestFilesViewModel.Update(largestFilesByLinesHeap);
            TopContributorsViewModel.Update(topContributors);

        }

        [RelayCommand]
        private void Cancel()
        {
            _cancellationTokenSource.Cancel();
        }

    }
}
