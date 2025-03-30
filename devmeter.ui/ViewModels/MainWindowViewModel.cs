using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using devmeter.core.Github;
using devmeter.core.Github.Models;
using devmeter.core.Processing;
using devmeter.ui.Core.Models;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
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

        public MainWindowViewModel()
        {
            RepoName = "-";
            _gitHubClient = new GitHubClient();
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

            var repo = repoAssembler.GetRepo();
            RepoName = repo.Name;
            ErrorMessage = string.Empty;

        }

    }
}
