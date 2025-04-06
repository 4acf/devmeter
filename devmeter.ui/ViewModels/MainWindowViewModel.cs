﻿using CommunityToolkit.Mvvm.ComponentModel;
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

        public TotalCommitsViewModel TotalCommitsViewModel { get; }
        public TotalContributorsViewModel TotalContributorsViewModel { get; }
        public LanguageBreakdownViewModel LanguageBreakdownViewModel { get; }
        public FilesOverTimeViewModel FilesOverTimeViewModel { get; }
        public LargestFilesViewModel LargestFilesViewModel { get; }
        public TopContributorsViewModel TopContributorsViewModel { get; }

        public MainWindowViewModel()
        {
            RepoName = "-";
            _gitHubClient = new GitHubClient(App.Configuration?["PAT"]);
            TotalCommitsViewModel = new TotalCommitsViewModel();
            TotalContributorsViewModel = new TotalContributorsViewModel();
            LanguageBreakdownViewModel = new LanguageBreakdownViewModel();
            FilesOverTimeViewModel = new FilesOverTimeViewModel();
            LargestFilesViewModel = new LargestFilesViewModel();
            TopContributorsViewModel = new TopContributorsViewModel();
        }

        private static string HandleError(GitHubApiResponse response)
        {
            var errorMessage = response.ErrorMessage;
            if(errorMessage == null && !string.IsNullOrEmpty(response.SerializedData))
            {
                errorMessage = JsonSerializer.Deserialize<GitHubApiError>(response.SerializedData)?.Message;
            }
            return errorMessage ?? Errors.Unexpected;
        }

        private static string HandleError(GitHubHtmlResponse response)
        {
            var errorMessage = response.ErrorMessage ?? response.StatusCode.ToString();
            return errorMessage ?? Errors.Unexpected;
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

            var repoAssembler = new RepoAssembler(new Repo());
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
            var recentCommits = recentCommitsData.Value;


            //update ui (todo: define all rules for formatting this data in stringformatting class)
            var repo = repoAssembler.GetRepo();
            RepoName = repoHandle.Substring(1);
            TotalCommitsViewModel.TotalCommits = unpackedHtmlData.Commits;
            TotalCommitsViewModel.CommitsInLast30Days = $"+{String.Format($"{recentCommits:n0}")} in the last 30 days";
            TotalContributorsViewModel.TotalContributors = unpackedHtmlData.Contributors;
            TotalContributorsViewModel.AverageContributions = $"Average Contributions: {StringFormatting.DivideStrings(unpackedHtmlData.Commits, unpackedHtmlData.Contributors)}";
            TopContributorsViewModel.Update(unpackedTopContributorsData);

            ErrorMessage = string.Empty;

        }

    }
}
