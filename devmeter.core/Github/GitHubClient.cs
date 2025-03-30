using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using devmeter.core.Github.Models;

namespace devmeter.core.Github
{
    public class GitHubClient
    {

        private static readonly Uri _baseUrl = new("https://api.github.com", UriKind.Absolute);
        private readonly HttpClient _httpClient;

        public GitHubClient()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "devmeter");
        }

        public async Task<GitHubApiResponse> GetRepositoryInformation(string absolutePath)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}repos{absolutePath}");
                var json = await response.Content.ReadAsStringAsync();
                return new GitHubApiResponse(response.IsSuccessStatusCode, null, json);
            }
            catch (HttpRequestException e)
            {
                return new GitHubApiResponse(false, e.Message, null);
            }
        }

        public async Task<string> GetCommits(string absolutePath)
        {
            return await _httpClient.GetStringAsync($"{_baseUrl}repos{absolutePath}/commits");
        }

        public async Task<string> GetContributors(string absolutePath)
        {
            return await _httpClient.GetStringAsync($"{_baseUrl}repos{absolutePath}/contributors");
        }

        public async Task<string> GetFileTreeRoot(string absolutePath)
        {
            return await _httpClient.GetStringAsync($"{_baseUrl}repos{absolutePath}/contents/");
        }

    }
}
