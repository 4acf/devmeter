using DevMeter.Core.Github.Models;
using System.Text;

namespace DevMeter.Core.Github
{
    public class GitHubClient : IGitHubClient
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

        public async Task<GitHubHtmlResponse> GetMainPageHtml(string repoHandle)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseGitHubUrl}{repoHandle}");
                var html = await response.Content.ReadAsStringAsync();
                return new GitHubHtmlResponse(response.IsSuccessStatusCode, response.StatusCode, null, html);
            }
            catch (HttpRequestException e)
            {
                return new GitHubHtmlResponse(false, e.StatusCode, e.Message, null);
            }
        }

        public async Task<GitHubHtmlResponse> GetPageHtml(string htmlUrl)
        {
            try
            {
                var response = await _httpClient.GetAsync(htmlUrl);
                var html = await response.Content.ReadAsStringAsync();
                return new GitHubHtmlResponse(response.IsSuccessStatusCode, response.StatusCode, null, html);
            }
            catch (HttpRequestException e)
            {
                return new GitHubHtmlResponse(false, e.StatusCode, e.Message, null);
            }
        }

        public async Task<GitHubApiResponse> GetCommits(string repoHandle, int page = 1, int timeSpan = -1, int perPage = 100)
        {

            var url = new StringBuilder();
            url.Append($"{_baseApiUrl}repos{repoHandle}/commits?page={page}&per_page={perPage}");
            if (timeSpan != -1)
            {
                var since = DateTime.UtcNow.AddDays(-timeSpan);
                url.Append($"&since={since}");
            }

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

        public async Task<GitHubApiResponse> GetContributors(string repoHandle, int perPage = 100)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseApiUrl}repos{repoHandle}/contributors?per_page={perPage}");
                var json = await response.Content.ReadAsStringAsync();
                return new GitHubApiResponse(response.IsSuccessStatusCode, null, json);
            }
            catch (HttpRequestException e)
            {
                return new GitHubApiResponse(false, e.Message, null);
            }
        }

        public async Task<GitHubApiResponse> GetLanguages(string repoHandle)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseApiUrl}repos{repoHandle}/languages");
                var json = await response.Content.ReadAsStringAsync();
                return new GitHubApiResponse(response.IsSuccessStatusCode, null, json);
            }
            catch (HttpRequestException e)
            {
                return new GitHubApiResponse(false, e.Message, null);
            }
        }

        public async Task<GitHubApiResponse> GetRootFolderContents(string repoHandle)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseApiUrl}repos{repoHandle}/contents");
                var json = await response.Content.ReadAsStringAsync();
                return new GitHubApiResponse(response.IsSuccessStatusCode, null, json);
            }
            catch (HttpRequestException e)
            {
                return new GitHubApiResponse(false, e.Message, null);
            }
        }

        public async Task<GitHubApiResponse> GetFolderContents(string url)
        {
            try
            {
                var response = await _httpClient.GetAsync(url);
                var json = await response.Content.ReadAsStringAsync();
                return new GitHubApiResponse(response.IsSuccessStatusCode, null, json);
            }
            catch (HttpRequestException e)
            {
                return new GitHubApiResponse(false, e.Message, null);
            }
        }

    }
}
