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
        private string? _errorMessage;

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

            if (!InputParser.TryParse(SearchString, out var result))
            {
                ErrorMessage = result;
                return;
            }
            var repoHandle = result;

            //each request is done sequentially to handle errors and rate limits 
            //https://docs.github.com/en/rest/using-the-rest-api/best-practices-for-using-the-rest-api?apiVersion=2022-11-28#avoid-concurrent-requests

            var dataCollector = new DataCollector(_gitHubClient, repoHandle);

            //get number of commits + number of contributors
            var htmlData = await dataCollector.GetHtmlData();
            if (!htmlData.Succeeded || htmlData == null)
            {
                if (htmlData == null)
                    ErrorMessage = Errors.Unexpected;
                else
                    ErrorMessage = htmlData.ErrorMessage;
                return;
            }
            var unpackedHtmlData = htmlData.Value;
            if (unpackedHtmlData == null)
            {
                ErrorMessage = Errors.Unexpected;
                return;
            }

            //get handles of the top contributors
            var topContributorData = await dataCollector.GetTopContributorData();
            if (!topContributorData.Succeeded || topContributorData == null)
            {
                if (topContributorData == null)
                    ErrorMessage = Errors.Unexpected;
                else
                    ErrorMessage = topContributorData.ErrorMessage;
                return;
            }
            var unpackedTopContributorsData = topContributorData.Value;
            if(unpackedTopContributorsData == null)
            {
                ErrorMessage = Errors.Unexpected;
                return;
            }

            //commits in last 30 days
            var recentCommitsData = await dataCollector.GetRecentCommitsData();
            if (!recentCommitsData.Succeeded || recentCommitsData == null)
            {
                if (recentCommitsData == null)
                    ErrorMessage = Errors.Unexpected;
                else
                    ErrorMessage = recentCommitsData.ErrorMessage;
                return;
            }
            var unpackedRecentCommits = recentCommitsData.Value;
            if (unpackedRecentCommits == null)
            {
                ErrorMessage = Errors.Unexpected;
                return;
            }

            //get data from linguist for language breakdown
            var linguistData = await dataCollector.GetLinguistData();
            if (!linguistData.Succeeded || linguistData == null)
            {
                if (linguistData == null)
                    ErrorMessage = Errors.Unexpected;
                else
                    ErrorMessage = linguistData.ErrorMessage;
                return;
            }
            var unpackedLanguages = linguistData.Value;
            if (unpackedLanguages == null)
            {
                ErrorMessage = Errors.Unexpected;
                return;
            }

            //reading file tree (slow!)
            var fileTreeData = await dataCollector.GetRootFolderContents();
            if (!fileTreeData.Succeeded || fileTreeData == null)
            {
                if (fileTreeData == null)
                    ErrorMessage = Errors.Unexpected;
                else
                    ErrorMessage = fileTreeData.ErrorMessage;
                return;
            }
            var unpackedFileTree = fileTreeData.Value;
            if (unpackedFileTree == null)
            {
                ErrorMessage = Errors.Unexpected;
                return;
            }

            //traverse tree to get largest files
            var largestFilesByLinesPq = new PriorityQueue<File, int>();
            dataCollector.GetLargestFiles(unpackedFileTree, largestFilesByLinesPq);

            //update ui (todo: define all rules for formatting this data in stringformatting class)
            RepoName = repoHandle.Substring(1);
            TotalLinesViewModel.TotalLines = $"{String.Format($"{unpackedFileTree.LinesOfCode + unpackedFileTree.LinesOfWhitespace:n0}")}";
            TotalLinesViewModel.TotalLinesExcludingWhitespace = $"Excluding Whitespace: {String.Format($"{unpackedFileTree.LinesOfCode:n0}")}";
            TotalCommitsViewModel.TotalCommits = unpackedHtmlData.Commits;
            TotalCommitsViewModel.Update(unpackedRecentCommits.Count);
            TotalContributorsViewModel.TotalContributors = unpackedHtmlData.Contributors;
            TotalContributorsViewModel.AverageContributions = $"Average Contributions: {StringFormatting.DivideStrings(unpackedHtmlData.Commits, unpackedHtmlData.Contributors)}";
            LanguageBreakdownViewModel.Update(unpackedLanguages);
            RecentActivityViewModel.Update(unpackedRecentCommits);
            LargestFilesViewModel.Update(largestFilesByLinesPq);
            TopContributorsViewModel.Update(unpackedTopContributorsData);

            ErrorMessage = string.Empty;

        }

    }
}
