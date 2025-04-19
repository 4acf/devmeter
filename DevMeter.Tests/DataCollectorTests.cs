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

        [Fact]
        public async void GetFolderContents_ReceivingNullJson_ReturnsUnsuccessfulResult()
        {
            var stub = new FakeGitHubClient();
            stub.Success = true;
            var dataCollector = MakeDataCollector(stub);

            var result = await dataCollector.GetFolderContents(null, new CancellationToken());

            Assert.False(result.Succeeded);
        }

        [Fact]
        public async void GetFolderContents_BadRequest_ReturnsUnsuccesfulResult()
        {
            var stub = new FakeGitHubClient();
            stub.Success = false;
            var dataCollector = MakeDataCollector(stub);

            var result = await dataCollector.GetFolderContents(null, new CancellationToken());

            Assert.False(result.Succeeded);
        }

        [Fact]
        public void GetLargestFiles_Receiving10Files_Returns10LargestFiles()
        {
            var filesystemObjects = new List<FilesystemObject>()
            {
                new Core.Models.File("file0", 10, 10, "C#"),
                new Core.Models.File("file1", 20, 20, "C#"),
                new Core.Models.File("file2", 30, 30, "C#"),
                new Core.Models.File("file3", 40, 40, "C#"),
                new Core.Models.File("file4", 50, 50, "C#"),
                new Core.Models.File("file5", 60, 60, "C#"),
                new Core.Models.File("file6", 70, 70, "C#"),
                new Core.Models.File("file7", 80, 80, "C#"),
                new Core.Models.File("file8", 90, 90, "C#"),
                new Core.Models.File("file9", 100, 100, "C#"),
            };
            var folder = new Folder("root", 550, 550, filesystemObjects);
            var stub = new FakeGitHubClient();
            var dataCollector = MakeDataCollector(stub);
            var pq = new PriorityQueue<Core.Models.File, int>();

            dataCollector.GetLargestFiles(folder, pq);

            int i = 1;
            while(pq.Count > 0)
            {
                Assert.Equal(i * 10, pq.Dequeue().LinesOfCode);
                i++;
            }
        }

        [Fact]
        public void GetLargestFiles_Receiving11Files_Returns10LargestFiles()
        {
            var filesystemObjects = new List<FilesystemObject>()
            {
                new Core.Models.File("file0", 10, 10, "C#"),
                new Core.Models.File("file1", 20, 20, "C#"),
                new Core.Models.File("file2", 30, 30, "C#"),
                new Core.Models.File("file3", 40, 40, "C#"),
                new Core.Models.File("file4", 50, 50, "C#"),
                new Core.Models.File("file5", 60, 60, "C#"),
                new Core.Models.File("file6", 70, 70, "C#"),
                new Core.Models.File("file7", 80, 80, "C#"),
                new Core.Models.File("file8", 90, 90, "C#"),
                new Core.Models.File("file9", 100, 100, "C#"),
                new Core.Models.File("file10", 110, 110, "C#"),
            };
            var folder = new Folder("root", 660, 660, filesystemObjects);
            var stub = new FakeGitHubClient();
            var dataCollector = MakeDataCollector(stub);
            var pq = new PriorityQueue<Core.Models.File, int>();

            dataCollector.GetLargestFiles(folder, pq);

            int i = 2;
            while (pq.Count > 0)
            {
                Assert.Equal(i * 10, pq.Dequeue().LinesOfCode);
                i++;
            }
        }

        [Fact]
        public void GetLargestFiles_ReceivingTree_Returns10LargestFiles()
        {
            var nestedFilesystemObjects = new List<FilesystemObject>()
            {
                new Core.Models.File("file0", 10, 10, "C#"),
                new Core.Models.File("file1", 20, 20, "C#"),
                new Core.Models.File("file2", 30, 30, "C#"),
                new Core.Models.File("file3", 40, 40, "C#"),
                new Core.Models.File("file4", 50, 50, "C#"),
                new Core.Models.File("file5", 60, 60, "C#"),
                new Core.Models.File("file6", 70, 70, "C#"),
                new Core.Models.File("file7", 80, 80, "C#"),
                new Core.Models.File("file8", 90, 90, "C#"),
                new Core.Models.File("file9", 100, 100, "C#"),
            };

            var filesystemObjects = new List<FilesystemObject>()
            {
                new Folder("folder0", 550, 550, nestedFilesystemObjects),
            };

            var folder = new Folder("root", 550, 550, filesystemObjects);
            var stub = new FakeGitHubClient();
            var dataCollector = MakeDataCollector(stub);
            var pq = new PriorityQueue<Core.Models.File, int>();

            dataCollector.GetLargestFiles(folder, pq);

            int i = 1;
            while (pq.Count > 0)
            {
                Assert.Equal(i * 10, pq.Dequeue().LinesOfCode);
                i++;
            }
        }

    }

    public class FakeGitHubClient : IGitHubClient
    {
        public bool Success = false;
        public string HtmlData = string.Empty;
        public string SerializedData = string.Empty;

        public Task<GitHubApiResponse> GetCommits(string repoHandle, int page, int timeSpan, int perPage)
        {
            if (Success && !string.IsNullOrEmpty(SerializedData))
            {
                return Task.FromResult(new GitHubApiResponse(Success, null, SerializedData));
            }
            else
            {
                return Task.FromResult(new GitHubApiResponse(Success, "Not Found", null));
            }
        }

        public Task<GitHubApiResponse> GetContributors(string repoHandle, int perPage)
        {
            if (Success && !string.IsNullOrEmpty(SerializedData))
            {
                return Task.FromResult(new GitHubApiResponse(Success, null, SerializedData));
            }
            else
            {
                return Task.FromResult(new GitHubApiResponse(Success, "Not Found", null));
            }
        }

        public Task<GitHubApiResponse> GetFolderContents(string url)
        {
            if (Success && !string.IsNullOrEmpty(SerializedData))
            {
                return Task.FromResult(new GitHubApiResponse(Success, null, SerializedData));
            }
            else
            {
                return Task.FromResult(new GitHubApiResponse(Success, "Not Found", null));
            }
        }

        public Task<GitHubApiResponse> GetLanguages(string repoHandle)
        {
            if (Success && !string.IsNullOrEmpty(SerializedData))
            {
                return Task.FromResult(new GitHubApiResponse(Success, null, SerializedData));
            }
            else
            {
                return Task.FromResult(new GitHubApiResponse(Success, "Not Found", null));
            }
        }

        public Task<GitHubHtmlResponse> GetMainPageHtml(string repoHandle)
        {
            if (Success && !string.IsNullOrEmpty(HtmlData))
            {
                return Task.FromResult(new GitHubHtmlResponse(Success, null, null, HtmlData));
            }
            else
            {
                return Task.FromResult(new GitHubHtmlResponse(Success, HttpStatusCode.NotFound, "NotFound", null));
            }
        }

        public Task<GitHubHtmlResponse> GetPageHtml(string htmlUrl)
        {
            if (Success && !string.IsNullOrEmpty(HtmlData))
            {
                return Task.FromResult(new GitHubHtmlResponse(Success, null, null, HtmlData));
            }
            else
            {
                return Task.FromResult(new GitHubHtmlResponse(Success, HttpStatusCode.NotFound, "NotFound", null));
            }
        }

        public Task<GitHubApiResponse> GetRootFolderContents(string repoHandle)
        {
            if (Success && !string.IsNullOrEmpty(SerializedData))
            {
                return Task.FromResult(new GitHubApiResponse(Success, null, SerializedData));
            }
            else
            {
                return Task.FromResult(new GitHubApiResponse(Success, "Not Found", null));
            }
        }
    }

}
