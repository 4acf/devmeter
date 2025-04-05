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

        private const int _topContributorsSize = 7;

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
            return errorMessage ?? "Unexpected error";
        }

        private static string HandleError(GitHubHtmlResponse response)
        {
            var errorMessage = response.ErrorMessage ?? response.StatusCode.ToString();
            return errorMessage ?? "Unexpected error";
        }

        [RelayCommand]
        private async Task Search()
        {

            if(InputParser.TryParse(SearchString, out var parseResult) == false)
            {
                ErrorMessage = parseResult;
                return;
            }

            //each request is done sequentially to handle errors and rate limits 
            //https://docs.github.com/en/rest/using-the-rest-api/best-practices-for-using-the-rest-api?apiVersion=2022-11-28#avoid-concurrent-requests

            var repoAssembler = new RepoAssembler(new Repo());

            //general info
            var generalInfoResponse = await _gitHubClient.GetRepositoryInformation(parseResult);
            if(!generalInfoResponse.Succeeded || string.IsNullOrEmpty(generalInfoResponse.SerializedData))
            {
                ErrorMessage = HandleError(generalInfoResponse);
                return;
            }
            var deserializedGeneralInfo = JsonSerializer.Deserialize<GitHubGeneralInformation>(generalInfoResponse.SerializedData);
            if (deserializedGeneralInfo != null)
            {
                repoAssembler.UpdateGeneralInfo(deserializedGeneralInfo);
            }

            //total commits + contributions
            var mainPageHtmlResponse = await _gitHubClient.GetMainPageHtml(parseResult);
            if (!mainPageHtmlResponse.Succeeded || string.IsNullOrEmpty(mainPageHtmlResponse.HtmlData))
            {
                ErrorMessage = HandleError(mainPageHtmlResponse);
                return;
            }
            var htmlDocumentParser = new HtmlDocumentParser(mainPageHtmlResponse.HtmlData); //todo: make HtmlDocumentParser implement IDisposable so you can use a using statement
            var numCommitsString = htmlDocumentParser.ExtractCommitsFromHtml();
            if (!string.IsNullOrEmpty(numCommitsString))
            {
                repoAssembler.UpdateCommits(numCommitsString);
            }
            var numContributorsString = htmlDocumentParser.ExtractContributorsFromHtml();
            if (string.IsNullOrEmpty(numContributorsString))
            {
                numContributorsString = "1";
            }
            repoAssembler.UpdateContributors(numContributorsString);

            //get handles of the top contributors
            var topContributorsResponse = await _gitHubClient.GetContributors(parseResult, _topContributorsSize);
            if (!topContributorsResponse.Succeeded || string.IsNullOrEmpty(topContributorsResponse.SerializedData))
            {
                ErrorMessage = HandleError(topContributorsResponse);
                return;
            }
            var deserializedTopContributors = JsonSerializer.Deserialize<List<GitHubContributor>>(topContributorsResponse.SerializedData);
            if (deserializedTopContributors != null)
            {
                var topContributors = new List<Contributor>();
                for (int i = 0; i < deserializedTopContributors.Count; i++)
                {
                    topContributors.Add(new Contributor
                    {
                        Name = deserializedTopContributors[i].Username,
                        Contributions = deserializedTopContributors[i].Contributions,
                    });
                }
                repoAssembler.UpdateTopContributors(topContributors);
            }

            //commits in last 30 days
            int page = 1;
            int recentCommits = 0;
            while (true)
            {
                var recentCommitsResponse = await _gitHubClient.GetCommits(parseResult, page, 30, 100);
                if (!recentCommitsResponse.Succeeded || string.IsNullOrEmpty(recentCommitsResponse.SerializedData))
                {
                    ErrorMessage = HandleError(recentCommitsResponse);
                    return;
                }

                var deserializedRecentCommits = JsonSerializer.Deserialize<List<GitHubCommit>>(recentCommitsResponse.SerializedData);
                if (deserializedRecentCommits == null)
                {
                    break;
                }
                recentCommits += deserializedRecentCommits.Count;
                if (deserializedRecentCommits.Count < 100)
                {
                    break;
                }
                page++;
            }
            repoAssembler.UpdateCommitsInLast30Days(recentCommits);

            //update ui (todo: define all rules for formatting this data in stringformatting class)
            var repo = repoAssembler.GetRepo();
            RepoName = repo.Name;
            TotalCommitsViewModel.TotalCommits = repo.Commits;
            TotalCommitsViewModel.CommitsInLast30Days = $"+{String.Format($"{repo.CommitsInLast30Days:n0}")} in the last 30 days";
            TotalContributorsViewModel.TotalContributors = repo.Contributors;
            TotalContributorsViewModel.AverageContributions = $"Average Contributions: {StringFormatting.DivideStrings(repo.Commits, repo.Contributors)}";
            TopContributorsViewModel.Update(repo.TopContributors);

            ErrorMessage = string.Empty;

        }

    }
}
