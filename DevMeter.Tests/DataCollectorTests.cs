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
            stub.SerializedData = "[\r\n  {\r\n    \"login\": \"4acf\",\r\n    \"id\": 136143054,\r\n    \"node_id\": \"U_kgDOCB1gzg\",\r\n    \"avatar_url\": \"https://avatars.githubusercontent.com/u/136143054?v=4\",\r\n    \"gravatar_id\": \"\",\r\n    \"url\": \"https://api.github.com/users/4acf\",\r\n    \"html_url\": \"https://github.com/4acf\",\r\n    \"followers_url\": \"https://api.github.com/users/4acf/followers\",\r\n    \"following_url\": \"https://api.github.com/users/4acf/following{/other_user}\",\r\n    \"gists_url\": \"https://api.github.com/users/4acf/gists{/gist_id}\",\r\n    \"starred_url\": \"https://api.github.com/users/4acf/starred{/owner}{/repo}\",\r\n    \"subscriptions_url\": \"https://api.github.com/users/4acf/subscriptions\",\r\n    \"organizations_url\": \"https://api.github.com/users/4acf/orgs\",\r\n    \"repos_url\": \"https://api.github.com/users/4acf/repos\",\r\n    \"events_url\": \"https://api.github.com/users/4acf/events{/privacy}\",\r\n    \"received_events_url\": \"https://api.github.com/users/4acf/received_events\",\r\n    \"type\": \"User\",\r\n    \"user_view_type\": \"public\",\r\n    \"site_admin\": false,\r\n    \"contributions\": 22\r\n  }\r\n]";
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
            stub.SerializedData = "[\r\n  {\r\n    \"sha\": \"7c7c22495e96788bdb855b983fe84a4f58b1ca93\",\r\n    \"node_id\": \"C_kwDOOJTvw9oAKDdjN2MyMjQ5NWU5Njc4OGJkYjg1NWI5ODNmZTg0YTRmNThiMWNhOTM\",\r\n    \"commit\": {\r\n      \"author\": {\r\n        \"name\": \"4acf\",\r\n        \"email\": \"136143054+4acf@users.noreply.github.com\",\r\n        \"date\": \"2025-03-16T05:04:13Z\"\r\n      },\r\n      \"committer\": {\r\n        \"name\": \"GitHub\",\r\n        \"email\": \"noreply@github.com\",\r\n        \"date\": \"2025-03-16T05:04:13Z\"\r\n      },\r\n      \"message\": \"ascii.frag formatting\",\r\n      \"tree\": {\r\n        \"sha\": \"6392f4d78a8b7b1fc2a9b70ebbd5c814c52a733a\",\r\n        \"url\": \"https://api.github.com/repos/4acf/ascii-frag/git/trees/6392f4d78a8b7b1fc2a9b70ebbd5c814c52a733a\"\r\n      },\r\n      \"url\": \"https://api.github.com/repos/4acf/ascii-frag/git/commits/7c7c22495e96788bdb855b983fe84a4f58b1ca93\",\r\n      \"comment_count\": 0,\r\n      \"verification\": {\r\n        \"verified\": true,\r\n        \"reason\": \"valid\",\r\n        \"signature\": \"-----BEGIN PGP SIGNATURE-----\\n\\nwsFcBAABCAAQBQJn1lvNCRC1aQ7uu5UhlAAAa9UQADAfXxcutqQh+ByB2O3qsECl\\nAZ/CC2LDQhSNaesRHx67HxZO3XbVPCsyvFTMnNNrM1+CF06qWjTWC2pXe37kaWs+\\nJ2b4RXr+4MjKHgyVoe7o26Tv81gKA0iPYBRYMRP5uTyzfuTfjUb7ACxnCIPbllsB\\nS/xW4c3YNOaHX/CPOVKeaOOmYy1qyX8j1LlESk8dhPoR9GkdLm2cNIYO0mTWxhQS\\nQwSmOtwKC80pHODcOyJ+T7iKPvRkV8VPI3FrmCkEtx3muCJg5yOUFnZ4Fiao5t//\\n1brDdUmERCxH0UTFLGRjcVnfbO1ZyYgB99W77M52EuCXTQwTTnPt5zvSOBw+XqX/\\nr35ulhgxKCKD+LDjbCP8GGgCOmt2nqKiR2VTPumOqcnuGhIYxKs8JnpMMbCmiWii\\nYNwXbWGc/ETMlQiySBw0kQ6oGLj2gp5OI3f8Kia+ZA8driiNeNtq99UOrOY4aySW\\n12XNB574PmlO3By6JdYH/W7IWtRuiVIX/A0YGWiMqqvuUqK4orF9s46DWmX9jaLg\\nMst/90D9cv7Ms3pXgaMpFvULz5Kvof/cSQZ9suEbcZ8SRZBDM4qtUx+9mqfjswyf\\n68KarY7J2hRF7sOn+TncXCp6ibJWPHYslG9f8vSqNZVWtx7MI5NEcO2F0ulrjW06\\n+rQ6daSOKGxXrLrdkV1T\\n=YZPG\\n-----END PGP SIGNATURE-----\\n\",\r\n        \"payload\": \"tree 6392f4d78a8b7b1fc2a9b70ebbd5c814c52a733a\\nparent aa7f2ea3e0c4f792c7037d6feb181bef3facfb57\\nauthor 4acf <136143054+4acf@users.noreply.github.com> 1742101453 -0700\\ncommitter GitHub <noreply@github.com> 1742101453 -0700\\n\\nascii.frag formatting\",\r\n        \"verified_at\": \"2025-03-16T05:07:20Z\"\r\n      }\r\n    },\r\n    \"url\": \"https://api.github.com/repos/4acf/ascii-frag/commits/7c7c22495e96788bdb855b983fe84a4f58b1ca93\",\r\n    \"html_url\": \"https://github.com/4acf/ascii-frag/commit/7c7c22495e96788bdb855b983fe84a4f58b1ca93\",\r\n    \"comments_url\": \"https://api.github.com/repos/4acf/ascii-frag/commits/7c7c22495e96788bdb855b983fe84a4f58b1ca93/comments\",\r\n    \"author\": {\r\n      \"login\": \"4acf\",\r\n      \"id\": 136143054,\r\n      \"node_id\": \"U_kgDOCB1gzg\",\r\n      \"avatar_url\": \"https://avatars.githubusercontent.com/u/136143054?v=4\",\r\n      \"gravatar_id\": \"\",\r\n      \"url\": \"https://api.github.com/users/4acf\",\r\n      \"html_url\": \"https://github.com/4acf\",\r\n      \"followers_url\": \"https://api.github.com/users/4acf/followers\",\r\n      \"following_url\": \"https://api.github.com/users/4acf/following{/other_user}\",\r\n      \"gists_url\": \"https://api.github.com/users/4acf/gists{/gist_id}\",\r\n      \"starred_url\": \"https://api.github.com/users/4acf/starred{/owner}{/repo}\",\r\n      \"subscriptions_url\": \"https://api.github.com/users/4acf/subscriptions\",\r\n      \"organizations_url\": \"https://api.github.com/users/4acf/orgs\",\r\n      \"repos_url\": \"https://api.github.com/users/4acf/repos\",\r\n      \"events_url\": \"https://api.github.com/users/4acf/events{/privacy}\",\r\n      \"received_events_url\": \"https://api.github.com/users/4acf/received_events\",\r\n      \"type\": \"User\",\r\n      \"user_view_type\": \"public\",\r\n      \"site_admin\": false\r\n    },\r\n    \"committer\": {\r\n      \"login\": \"web-flow\",\r\n      \"id\": 19864447,\r\n      \"node_id\": \"MDQ6VXNlcjE5ODY0NDQ3\",\r\n      \"avatar_url\": \"https://avatars.githubusercontent.com/u/19864447?v=4\",\r\n      \"gravatar_id\": \"\",\r\n      \"url\": \"https://api.github.com/users/web-flow\",\r\n      \"html_url\": \"https://github.com/web-flow\",\r\n      \"followers_url\": \"https://api.github.com/users/web-flow/followers\",\r\n      \"following_url\": \"https://api.github.com/users/web-flow/following{/other_user}\",\r\n      \"gists_url\": \"https://api.github.com/users/web-flow/gists{/gist_id}\",\r\n      \"starred_url\": \"https://api.github.com/users/web-flow/starred{/owner}{/repo}\",\r\n      \"subscriptions_url\": \"https://api.github.com/users/web-flow/subscriptions\",\r\n      \"organizations_url\": \"https://api.github.com/users/web-flow/orgs\",\r\n      \"repos_url\": \"https://api.github.com/users/web-flow/repos\",\r\n      \"events_url\": \"https://api.github.com/users/web-flow/events{/privacy}\",\r\n      \"received_events_url\": \"https://api.github.com/users/web-flow/received_events\",\r\n      \"type\": \"User\",\r\n      \"user_view_type\": \"public\",\r\n      \"site_admin\": false\r\n    },\r\n    \"parents\": [\r\n      {\r\n        \"sha\": \"aa7f2ea3e0c4f792c7037d6feb181bef3facfb57\",\r\n        \"url\": \"https://api.github.com/repos/4acf/ascii-frag/commits/aa7f2ea3e0c4f792c7037d6feb181bef3facfb57\",\r\n        \"html_url\": \"https://github.com/4acf/ascii-frag/commit/aa7f2ea3e0c4f792c7037d6feb181bef3facfb57\"\r\n      }\r\n    ]\r\n  }]";
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
            stub.SerializedData = "{\r\n  \"C#\": 5647331\r\n}";
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
