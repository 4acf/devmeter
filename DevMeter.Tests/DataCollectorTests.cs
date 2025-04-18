using DevMeter.Core.Github;
using DevMeter.Core.Github.Models;
using DevMeter.Core.Models;
using DevMeter.Core.Processing;
using DevMeter.Core.Utils;
using DevMeter.Tests.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DevMeter.Tests
{
    public class DataCollectorTests
    {

        private static DataCollector MakeDataCollector(IGitHubClient gitHubClient)
        {
            return new DataCollector(gitHubClient, string.Empty);
        }

        [Fact]
        public async void GetHtmlData_ReceivingValidHtml_ReturnsSuccessfulResult()
        {
            var stub = new FakeGitHubClient();
            stub.Success = true;
            stub.HtmlData = HtmlStrings.LinuxHomePageHtml;
            var dataCollector = MakeDataCollector(stub);

            var result = await dataCollector.GetHtmlData();

            Assert.True(result.Succeeded);
        }

        [Fact]
        public async void GetHtmlData_ReceivingInvalidNonNullHtml_ReturnsDefaultValues()
        {
            var stub = new FakeGitHubClient();
            stub.Success = true;
            stub.HtmlData = HtmlStrings.EmptyHtml;
            var dataCollector = MakeDataCollector(stub);

            var result = await dataCollector.GetHtmlData();

            Assert.True(result.Succeeded);
            Assert.NotNull(result.Value);
            Assert.Equal("0", result.Value.Commits);
            Assert.Equal("1", result.Value.Contributors);
        }

        [Fact]
        public async void GetHtmlData_ReceivingInvalidHtml_ReturnsUnsuccesfulResult()
        {
            var stub = new FakeGitHubClient();
            stub.Success = true;
            stub.HtmlData = string.Empty;
            var dataCollector = MakeDataCollector(stub);

            var result = await dataCollector.GetHtmlData();

            Assert.False(result.Succeeded);
        }

        [Fact]
        public async void GetTopContributorData_ReceivingValidJson_ReturnsSuccessfulResult()
        {
            var stub = new FakeGitHubClient();
            stub.Success = true;
            stub.SerializedData = JsonStrings.Contributors;
            var dataCollector = MakeDataCollector(stub);

            var result = await dataCollector.GetTopContributorData();

            Assert.True(result.Succeeded);
        }

        [Fact]
        public async void GetTopContributorsData_ReceivingNullJson_ReturnsUnsuccessfulResult()
        {
            var stub = new FakeGitHubClient();
            stub.Success = true;
            var dataCollector = MakeDataCollector(stub);

            var result = await dataCollector.GetTopContributorData();

            Assert.False(result.Succeeded);
        }

        [Fact]
        public async void GetTopContributorsData_BadRequest_ReturnsUnsuccessfulResult()
        {
            var stub = new FakeGitHubClient();
            stub.Success = false;
            var dataCollector = MakeDataCollector(stub);

            var result = await dataCollector.GetTopContributorData();

            Assert.False(result.Succeeded);
        }

        [Fact]
        public async void GetRecentCommitsData_ReceivingValidJson_ReturnsSuccessfulResult()
        {
            var stub = new FakeGitHubClient();
            stub.Success = true;
            stub.SerializedData = JsonStrings.RecentCommits;
            var dataCollector = MakeDataCollector(stub);

            var result = await dataCollector.GetRecentCommitsData();

            Assert.True(result.Succeeded);
        }

        [Fact]
        public async void GetRecentCommitsData_ReceivingNullJson_ReturnsUnsuccessfulResult()
        {
            var stub = new FakeGitHubClient();
            stub.Success = true;
            var dataCollector = MakeDataCollector(stub);

            var result = await dataCollector.GetRecentCommitsData();

            Assert.False(result.Succeeded);
        }

        [Fact]
        public async void GetRecentCommitsData_BadRequest_ReturnsUnsuccessfulResult()
        {
            var stub = new FakeGitHubClient();
            stub.Success = false;
            var dataCollector = MakeDataCollector(stub);

            var result = await dataCollector.GetRecentCommitsData();

            Assert.False(result.Succeeded);
        }


        [Fact]
        public async void GetLinguistData_ReceivingValidJson_ReturnsSuccessfulResult()
        {
            var stub = new FakeGitHubClient();
            stub.Success = true;
            stub.SerializedData = JsonStrings.Linguist;
            var dataCollector = MakeDataCollector(stub);

            var result = await dataCollector.GetLinguistData();

            Assert.True(result.Succeeded);
            Assert.NotNull(result.Value);
            Assert.True(result.Value.ContainsKey("C#"));
            Assert.Equal(5647331, result.Value["C#"]);
        }

        [Fact]
        public async void GetLinguistData_ReceivingNullJson_ReturnsUnsuccessfulResult()
        {
            var stub = new FakeGitHubClient();
            stub.Success = true;
            var dataCollector = MakeDataCollector(stub);

            var result = await dataCollector.GetLinguistData();

            Assert.False(result.Succeeded);
        }

        [Fact]
        public async void GetLinguistData_BadRequest_ReturnsUnsuccessfulResult()
        {
            var stub = new FakeGitHubClient();
            stub.Success = false;
            var dataCollector = MakeDataCollector(stub);

            var result = await dataCollector.GetLinguistData();

            Assert.False(result.Succeeded);
        }

    }

    public class FakeGitHubClient : IGitHubClient
    {
        public bool Success = false;
        public string HtmlData = string.Empty;
        public string SerializedData = string.Empty;

        public async Task<GitHubApiResponse> GetCommits(string repoHandle, int page, int timeSpan, int perPage)
        {
            if (Success && !string.IsNullOrEmpty(SerializedData))
            {
                return new GitHubApiResponse(Success, null, SerializedData);
            }
            else
            {
                return new GitHubApiResponse(Success, "Not Found", null);
            }
        }

        public async Task<GitHubApiResponse> GetContributors(string repoHandle, int perPage)
        {
            if (Success && !string.IsNullOrEmpty(SerializedData))
            {
                return new GitHubApiResponse(Success, null, SerializedData);
            }
            else
            {
                return new GitHubApiResponse(Success, "Not Found", null);
            }
        }

        public Task<GitHubApiResponse> GetFolderContents(string url)
        {
            throw new NotImplementedException();
        }

        public async Task<GitHubApiResponse> GetLanguages(string repoHandle)
        {
            if (Success && !string.IsNullOrEmpty(SerializedData))
            {
                return new GitHubApiResponse(Success, null, SerializedData);
            }
            else
            {
                return new GitHubApiResponse(Success, "Not Found", null);
            }
        }

        public async Task<GitHubHtmlResponse> GetMainPageHtml(string repoHandle)
        {
            if (Success && !string.IsNullOrEmpty(HtmlData))
            {
                return new GitHubHtmlResponse(Success, null, null, HtmlData);
            }
            else
            {
                return new GitHubHtmlResponse(Success, HttpStatusCode.NotFound, "NotFound", null);
            }
        }

        public Task<GitHubHtmlResponse> GetPageHtml(string htmlUrl)
        {
            throw new NotImplementedException();
        }

        public Task<GitHubApiResponse> GetRootFolderContents(string repoHandle)
        {
            throw new NotImplementedException();
        }
    }

}
