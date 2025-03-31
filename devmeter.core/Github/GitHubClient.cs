﻿using System;
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

        public readonly int PerPage;
        private static readonly Uri _baseApiUrl = new("https://api.github.com", UriKind.Absolute);
        private static readonly Uri _baseGitHubUrl = new("https://github.com", UriKind.Absolute);
        private readonly HttpClient _httpClient;

        public GitHubClient(int perPage)
        {
            PerPage = perPage;
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "devmeter");
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

        public async Task<GitHubApiResponse> GetCommits(string absolutePath, int page = 1, int timeSpan = 0)
        {

            var since = DateTime.UtcNow.AddDays(-timeSpan);

            try
            {
                var response = await _httpClient.GetAsync($"{_baseApiUrl}repos{absolutePath}/commits?page={page}&since={since}&per_page={PerPage}");
                var json = await response.Content.ReadAsStringAsync();
                return new GitHubApiResponse(response.IsSuccessStatusCode, null, json);
            }
            catch (HttpRequestException e)
            {
                return new GitHubApiResponse(false, e.Message, null);
            }
        }

        public async Task<GitHubApiResponse> GetContributors(string absolutePath, int page)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseApiUrl}repos{absolutePath}/contributors?page={page}");
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
