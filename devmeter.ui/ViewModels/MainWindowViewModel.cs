using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using devmeter.core.Github;
using devmeter.core.Github.Models;
using devmeter.core.Processing;
using devmeter.ui.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml;

namespace devmeter.ui.ViewModels
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

        public MainWindowViewModel()
        {
            RepoName = "-";
            _gitHubClient = new GitHubClient(100);
            TotalCommitsViewModel = new TotalCommitsViewModel();
            TotalContributorsViewModel = new TotalContributorsViewModel();
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
            if (generalInfoResponse.Succeeded && generalInfoResponse.SerializedData != null)
            {
                var deserializedGeneralInfo = JsonSerializer.Deserialize<GitHubGeneralInformation>(generalInfoResponse.SerializedData);
                if (deserializedGeneralInfo != null) 
                {
                    repoAssembler.UpdateGeneralInfo(deserializedGeneralInfo);
                }
            }
            else
            {
                var errorMessage = generalInfoResponse.ErrorMessage;
                if (errorMessage == null && generalInfoResponse.SerializedData != null)
                {
                    errorMessage = JsonSerializer.Deserialize<GitHubApiError>(generalInfoResponse.SerializedData)?.Message;
                }
                ErrorMessage = errorMessage ?? "Unexpected error";
                return;
            }

            //total commits + contributions
            var mainPageHtmlResponse = await _gitHubClient.GetMainPageHtml(parseResult);
            if (mainPageHtmlResponse.Succeeded && mainPageHtmlResponse.HtmlData != null)
            {
                var htmlDocumentParser = new HtmlDocumentParser(mainPageHtmlResponse.HtmlData);
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

            }
            else
            {
                var errorMessage = mainPageHtmlResponse.ErrorMessage ?? mainPageHtmlResponse.StatusCode.ToString();
                ErrorMessage = errorMessage ?? "Unexpected error";
                return;
            }

            //commits in last 30 days
            int page = 1;
            int recentCommits = 0;
            while (true)
            {
                var recentCommitsResponse = await _gitHubClient.GetCommits(parseResult, page, 30);
                if (recentCommitsResponse.Succeeded && recentCommitsResponse.SerializedData != null)
                {
                    var deserializedRecentCommits = JsonSerializer.Deserialize<List<GitHubCommit>>(recentCommitsResponse.SerializedData);
                    if (deserializedRecentCommits != null)
                    {
                        recentCommits += deserializedRecentCommits.Count;
                        if (deserializedRecentCommits.Count < _gitHubClient.PerPage)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    var errorMessage = recentCommitsResponse.ErrorMessage;
                    if (errorMessage == null && recentCommitsResponse.SerializedData != null)
                    {
                        errorMessage = JsonSerializer.Deserialize<GitHubApiError>(recentCommitsResponse.SerializedData)?.Message;
                    }
                    ErrorMessage = errorMessage ?? "Unexpected error";
                    return;
                }
            }
            repoAssembler.UpdateCommitsInLast30Days(recentCommits)

            //update ui
            var repo = repoAssembler.GetRepo();
            RepoName = repo.Name;
            ErrorMessage = string.Empty;
            TotalCommitsViewModel.TotalCommits = repo.Commits;
            TotalCommitsViewModel.CommitsInLast30Days = $"+{repo.CommitsInLast30Days} in the last 30 days";
            TotalContributorsViewModel.TotalContributors = repo.Contributors;

        }

    }
}
