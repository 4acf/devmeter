using DevMeter.Core.Github;
using DevMeter.Core.Github.Models;
using DevMeter.Core.Models;
using DevMeter.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DevMeter.Core.Processing
{
    public class DataCollector
    {

        private GitHubClient _gitHubClient;
        private string _repoHandle;
        private const int _topContributorsSize = 7;

        public DataCollector(GitHubClient gitHubClient, string repoHandle)
        {
            _gitHubClient = gitHubClient;
            _repoHandle = repoHandle;
        }

        private static string HandleError(GitHubApiResponse response)
        {
            var errorMessage = response.ErrorMessage;
            if (errorMessage == null && !string.IsNullOrEmpty(response.SerializedData))
            {
                errorMessage = JsonSerializer.Deserialize<GitHubApiError>(response.SerializedData)?.Message;
            }
            return errorMessage ?? Errors.Unexpected;
        }

        private static string HandleError(GitHubHtmlResponse response)
        {
            var errorMessage = response.ErrorMessage ?? response.StatusCode.ToString();
            if (errorMessage != null && errorMessage == "NotFound")
                errorMessage = "Not Found";
            return errorMessage ?? Errors.Unexpected;
        }

        public async Task<Result<HtmlData>> GetHtmlData()
        {
            var mainPageHtmlResponse = await _gitHubClient.GetMainPageHtml(_repoHandle);
            if (!mainPageHtmlResponse.Succeeded || string.IsNullOrEmpty(mainPageHtmlResponse.HtmlData))
            {
                var result = new Result<HtmlData>(false, HandleError(mainPageHtmlResponse), null);
                return result;
            }

            var htmlDocumentParser = new HtmlDocumentParser(mainPageHtmlResponse.HtmlData);

            var numCommitsString = htmlDocumentParser.ExtractCommitsFromHtml();
            if (string.IsNullOrEmpty(numCommitsString))
            {
                numCommitsString = "0";
            }

            var numContributorsString = htmlDocumentParser.ExtractContributorsFromHtml();
            if (string.IsNullOrEmpty(numContributorsString))
            {
                numContributorsString = "1";
            }

            var htmlData = new HtmlData(numCommitsString, numContributorsString);
            return new Result<HtmlData>(true, null, htmlData);
        }

        public async Task<Result<List<Contributor>>> GetTopContributorData()
        {
            var topContributorsResponse = await _gitHubClient.GetContributors(_repoHandle, _topContributorsSize);
            if (!topContributorsResponse.Succeeded || string.IsNullOrEmpty(topContributorsResponse.SerializedData))
            {
                var result = new Result<List<Contributor>>(false, HandleError(topContributorsResponse), null);
                return result;
            }

            var deserializedTopContributors = JsonSerializer.Deserialize<List<GitHubContributor>>(topContributorsResponse.SerializedData);
            if (deserializedTopContributors == null)
            {
                var result = new Result<List<Contributor>>(false, Errors.CantReadTopContributors, null);
                return result;
            }

            var topContributors = new List<Contributor>();
            for (int i = 0; i < deserializedTopContributors.Count; i++)
            {
                topContributors.Add(new Contributor
                {
                    Name = deserializedTopContributors[i].Username,
                    Contributions = deserializedTopContributors[i].Contributions,
                });
            }

            return new Result<List<Contributor>>(true, null, topContributors);

        }

        public async Task<Result<int>> GetRecentCommitsData()
        {
            int page = 1;
            int recentCommits = 0;
            while (true)
            {
                var recentCommitsResponse = await _gitHubClient.GetCommits(_repoHandle, page, 30, 100);
                if (!recentCommitsResponse.Succeeded || string.IsNullOrEmpty(recentCommitsResponse.SerializedData))
                {
                    var result = new Result<int>(false, HandleError(recentCommitsResponse), 0);
                    return result;
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

            return new Result<int>(true, null, recentCommits);

        }

    }

    public class HtmlData
    {
        public string Commits { get; }
        public string Contributors { get; }

        public HtmlData(string commits, string contributors)
        {
            Commits = commits;
            Contributors = contributors;
        }
    }

}
