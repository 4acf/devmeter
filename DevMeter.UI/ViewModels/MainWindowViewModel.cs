using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DevMeter.Core.Github;
using DevMeter.Core.Github.Models;
using DevMeter.Core.Processing;
using DevMeter.Core.Processing.Formatting;
using DevMeter.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml;
using DevMeter.Core.Utils;
using Avalonia.Remote.Protocol;

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

        private readonly GitHubClient _gitHubClient;

        public TotalLinesViewModel TotalLinesViewModel { get; }
        public TotalCommitsViewModel TotalCommitsViewModel { get; }
        public TotalContributorsViewModel TotalContributorsViewModel { get; }
        public LanguageBreakdownViewModel LanguageBreakdownViewModel { get; }
        public RecentActivityViewModel RecentActivityViewModel { get; }
        public LargestFilesViewModel LargestFilesViewModel { get; }
        public TopContributorsViewModel TopContributorsViewModel { get; }

        public MainWindowViewModel()
        {
            StatusColor = Colors.Status;
            RepoName = "-";
            _gitHubClient = new GitHubClient(App.Configuration?["PAT"]);
            TotalLinesViewModel = new TotalLinesViewModel();
            TotalCommitsViewModel = new TotalCommitsViewModel();
            TotalContributorsViewModel = new TotalContributorsViewModel();
            LanguageBreakdownViewModel = new LanguageBreakdownViewModel();
            RecentActivityViewModel = new RecentActivityViewModel();
            LargestFilesViewModel = new LargestFilesViewModel();
            TopContributorsViewModel = new TopContributorsViewModel();
        }

        [RelayCommand]
        private async Task Search()
        {

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
            var htmlData = await dataCollector.GetHtmlData();
            if (!htmlData.Succeeded || htmlData == null)
            {
                if (htmlData == null)
                    StatusMessage = Errors.Unexpected;
                else
                    StatusMessage = htmlData.ErrorMessage;
                StatusColor = Colors.Error;
                return;
            }
            var unpackedHtmlData = htmlData.Value;
            if (unpackedHtmlData == null)
            {
                StatusMessage = Errors.Unexpected;
                StatusColor = Colors.Error;
                return;
            }

            //get handles of the top contributors
            StatusMessage = "Fetching top contributors...";
            var topContributorData = await dataCollector.GetTopContributorData();
            if (!topContributorData.Succeeded || topContributorData == null)
            {
                if (topContributorData == null)
                    StatusMessage = Errors.Unexpected;
                else
                    StatusMessage = topContributorData.ErrorMessage;
                StatusColor = Colors.Error;
                return;
            }
            var unpackedTopContributorsData = topContributorData.Value;
            if(unpackedTopContributorsData == null)
            {
                StatusMessage = Errors.Unexpected;
                StatusColor = Colors.Error;
                return;
            }

            //commits in last 30 days
            StatusMessage = "Fetching commits in last 30 days...";
            var recentCommitsData = await dataCollector.GetRecentCommitsData();
            if (!recentCommitsData.Succeeded || recentCommitsData == null)
            {
                if (recentCommitsData == null)
                    StatusMessage = Errors.Unexpected;
                else
                    StatusMessage = recentCommitsData.ErrorMessage;
                StatusColor = Colors.Error;
                return;
            }
            var unpackedRecentCommits = recentCommitsData.Value;
            if (unpackedRecentCommits == null)
            {
                StatusMessage = Errors.Unexpected;
                StatusColor = Colors.Error;
                return;
            }

            //get data from linguist for language breakdown
            StatusMessage = "Fetching language data...";
            var linguistData = await dataCollector.GetLinguistData();
            if (!linguistData.Succeeded || linguistData == null)
            {
                if (linguistData == null)
                    StatusMessage = Errors.Unexpected;
                else
                    StatusMessage = linguistData.ErrorMessage;
                StatusColor = Colors.Error;
                return;
            }
            var unpackedLanguages = linguistData.Value;
            if (unpackedLanguages == null)
            {
                StatusMessage = Errors.Unexpected;
                StatusColor = Colors.Error;
                return;
            }

            //reading file tree (slow!)
            StatusMessage = "Reading file tree... (this may take a while)";
            var fileTreeData = await dataCollector.GetRootFolderContents();
            if (!fileTreeData.Succeeded || fileTreeData == null)
            {
                if (fileTreeData == null)
                    StatusMessage = Errors.Unexpected;
                else
                    StatusMessage = fileTreeData.ErrorMessage;
                StatusColor = Colors.Error;
                return;
            }
            var unpackedFileTree = fileTreeData.Value;
            if (unpackedFileTree == null)
            {
                StatusMessage = Errors.Unexpected;
                StatusColor = Colors.Error;
                return;
            }

            //traverse tree to get largest files
            StatusMessage = "Analyzing file sizes";
            var largestFilesByLinesPq = new PriorityQueue<File, int>();
            dataCollector.GetLargestFiles(unpackedFileTree, largestFilesByLinesPq);

            StatusMessage = string.Empty;

            //batch update ui
            RepoName = repoHandle.Substring(1);
            TotalLinesViewModel.Update(unpackedFileTree.LinesOfCode, unpackedFileTree.LinesOfWhitespace);
            TotalCommitsViewModel.Update(unpackedHtmlData.Commits, unpackedRecentCommits.Count);
            TotalContributorsViewModel.Update(unpackedHtmlData);
            LanguageBreakdownViewModel.Update(unpackedLanguages);
            RecentActivityViewModel.Update(unpackedRecentCommits);
            LargestFilesViewModel.Update(largestFilesByLinesPq);
            TopContributorsViewModel.Update(unpackedTopContributorsData);

        }

    }
}
