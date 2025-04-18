using DevMeter.Core.Github.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DevMeter.Core.Github
{
    public interface IGitHubClient
    {
        public Task<GitHubHtmlResponse> GetMainPageHtml(string repoHandle);
        public Task<GitHubHtmlResponse> GetPageHtml(string htmlUrl);
        public Task<GitHubApiResponse> GetCommits(string repoHandle, int page, int timeSpan, int perPage);
        public Task<GitHubApiResponse> GetContributors(string repoHandle, int perPage);
        public Task<GitHubApiResponse> GetLanguages(string repoHandle);
        public Task<GitHubApiResponse> GetRootFolderContents(string repoHandle);
        public Task<GitHubApiResponse> GetFolderContents(string url);
    }
}
