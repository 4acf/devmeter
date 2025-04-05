using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using DevMeter.Core.Github.Models;

namespace DevMeter.Core.Github
{
    public class GitHubClient
    {

        private static readonly Uri _baseApiUrl = new("https://api.github.com", UriKind.Absolute);
        private static readonly Uri _baseGitHubUrl = new("https://github.com", UriKind.Absolute);
        private readonly HttpClient _httpClient;

        public GitHubClient(string? pat)
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "devmeter");
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {pat}");
        }

        public async Task<GitHubHtmlResponse> GetMainPageHtml(string absolutePath)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseGitHubUrl}{absolutePath}");
                var html = await response.Content.ReadAsStringAsync();
                return new GitHubHtmlResponse(response.IsSuccessStatusCode, response.StatusCode, null, html);
            }
            catch (HttpRequestException e)
            {
                return new GitHubHtmlResponse(false, e.StatusCode, e.Message, null);
            }
        }

        public async Task<GitHubHtmlResponse> GetPageHtml(string path)
        {
            try
            {
                var response = await _httpClient.GetAsync(path);
                var html = await response.Content.ReadAsStringAsync();
                return new GitHubHtmlResponse(response.IsSuccessStatusCode, response.StatusCode, null, html);
            }
            catch (HttpRequestException e)
            {
                return new GitHubHtmlResponse(false, e.StatusCode, e.Message, null);
            }
        }

        public async Task<GitHubApiResponse> GetRepositoryInformation(string absolutePath)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseApiUrl}repos{absolutePath}");
                var json = await response.Content.ReadAsStringAsync();
                return new GitHubApiResponse(response.IsSuccessStatusCode, null, json);
            }
            catch (HttpRequestException e)
            {
                return new GitHubApiResponse(false, e.Message, null);
            }
        }

        public async Task<GitHubApiResponse> GetCommits(string absolutePath, int page = 1, int timeSpan = -1, int perPage = 100)
        {

            var url = new StringBuilder();
            url.Append($"{_baseApiUrl}repos{absolutePath}/commits?page={page}&per_page={perPage}");
            if(timeSpan != -1)
            {
                var since = DateTime.UtcNow.AddDays(-timeSpan);
                url.Append($"&since={since}");
            }

            Debug.WriteLine(url);

            try
            {
                var response = await _httpClient.GetAsync(url.ToString());
                var json = await response.Content.ReadAsStringAsync();
                return new GitHubApiResponse(response.IsSuccessStatusCode, null, json);
            }
            catch (HttpRequestException e)
            {
                return new GitHubApiResponse(false, e.Message, null);
            }
        }

        public async Task<GitHubApiResponse> GetContributors(string absolutePath, int perPage = 100)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseApiUrl}repos{absolutePath}/contributors?per_page={perPage}");
                var json = await response.Content.ReadAsStringAsync();
                return new GitHubApiResponse(response.IsSuccessStatusCode, null, json);
            }
            catch (HttpRequestException e)
            {
                return new GitHubApiResponse(false, e.Message, null);
            }
        }

        public async Task<string> GetFileTreeRoot(string absolutePath)
        {
            return await _httpClient.GetStringAsync($"{_baseApiUrl}repos{absolutePath}/contents/");
        }

    }
}
