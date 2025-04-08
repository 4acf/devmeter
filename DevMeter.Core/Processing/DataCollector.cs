using DevMeter.Core.Github;
using DevMeter.Core.Github.Models;
using DevMeter.Core.Models;
using DevMeter.Core.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.Marshalling;
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
        private const int _numberOfLargestFiles = 10;

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
            if(errorMessage != null && errorMessage.ToLower().Contains("rate limit")) 
            {
                errorMessage = "Rate limit reached";
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

        public async Task<Result<List<GitHubCommit>>> GetRecentCommitsData()
        {

            var allRecentCommits = new List<GitHubCommit>();

            int page = 1;
            while (true)
            {
                var recentCommitsResponse = await _gitHubClient.GetCommits(_repoHandle, page, 30, 100);
                if (!recentCommitsResponse.Succeeded || string.IsNullOrEmpty(recentCommitsResponse.SerializedData))
                {
                    var result = new Result<List<GitHubCommit>>(false, HandleError(recentCommitsResponse), null);
                    return result;
                }

                var deserializedRecentCommits = JsonSerializer.Deserialize<List<GitHubCommit>>(recentCommitsResponse.SerializedData);
                if (deserializedRecentCommits == null)
                {
                    break;
                }
                allRecentCommits.AddRange(deserializedRecentCommits);
                if (deserializedRecentCommits.Count < 100)
                {
                    break;
                }
                page++;
            }

            return new Result<List<GitHubCommit>>(true, null, allRecentCommits);

        }

        public async Task<Result<Dictionary<string, long>>> GetLinguistData()
        {
            var languagesResponse = await _gitHubClient.GetLanguages(_repoHandle);
            if(!languagesResponse.Succeeded || string.IsNullOrEmpty(languagesResponse.SerializedData))
            {
                var result = new Result<Dictionary<string, long>>(false, HandleError(languagesResponse), null);
                return result;
            }

            var deserializedLanguagesResponse = JsonSerializer.Deserialize<Dictionary<string, long>>(languagesResponse.SerializedData);
            return new Result<Dictionary<string, long>>(true, null, deserializedLanguagesResponse);
        }

        public async Task<Result<Folder>> GetRootFolderContents()
        {
            var rootFolderContentsResponse = await _gitHubClient.GetRootFolderContents(_repoHandle);
            if(!rootFolderContentsResponse.Succeeded || string.IsNullOrEmpty(rootFolderContentsResponse.SerializedData))
            {
                var result = new Result<Folder>(false, HandleError(rootFolderContentsResponse), null);
                return result;
            }

            var deserializedRootFolderContents = JsonSerializer.Deserialize<List<GitHubContents>>(rootFolderContentsResponse.SerializedData);
            if(deserializedRootFolderContents == null)
            {
                var result = new Result<Folder>(false, Errors.CantReadRootFolderContents, null);
                return result;
            }

            int linesOfCode = 0;
            int linesOfWhitespace = 0;
            var filesystemObjects = new List<FilesystemObject>();
            foreach(var content in deserializedRootFolderContents)
            {

                if(content.Type == Filetypes.Dir)
                {
                    var subfolderData = await GetFolderContents(content);
                    if (!subfolderData.Succeeded || subfolderData == null)
                    {
                        var result = new Result<Folder>(
                            false,
                            subfolderData == null ? Errors.Unexpected : subfolderData.ErrorMessage,
                            null
                        );
                        return result;
                    }

                    var subfolder = subfolderData.Value;
                    if (subfolder == null)
                    {
                        var result = new Result<Folder>(false, Errors.CantReadFolderContents, null);
                        return result;
                    }

                    filesystemObjects.Add(subfolder);
                    linesOfCode += subfolder.LinesOfCode;
                    linesOfWhitespace += subfolder.LinesOfWhitespace;

                }
                else
                {
                    var fileData = await GetFile(content);
                    if (!fileData.Succeeded || fileData == null)
                    {
                        var result = new Result<Folder>(
                            false,
                            fileData == null ? Errors.Unexpected : fileData.ErrorMessage,
                            null
                        );
                        return result;
                    }

                    var file = fileData.Value;
                    if (file == null)
                    {
                        var result = new Result<Folder>(false, Errors.CantReadFileContents, null);
                        return result;
                    }

                    filesystemObjects.Add(file);
                    linesOfCode += file.LinesOfCode;
                    linesOfWhitespace += file.LinesOfWhitespace;

                }

            }

            var folder = new Folder(
                "root",
                linesOfCode,
                linesOfWhitespace,
                filesystemObjects
                );

            return new Result<Folder>(true, null, folder);

        }

        private async Task<Result<Folder>> GetFolderContents(GitHubContents input)
        {
            var folderContentsResponse = await _gitHubClient.GetFolderContents(input.Url);
            if (!folderContentsResponse.Succeeded || string.IsNullOrEmpty(folderContentsResponse.SerializedData))
            {
                var result = new Result<Folder>(false, HandleError(folderContentsResponse), null);
                return result;
            }

            var deserializedFolderContents = JsonSerializer.Deserialize<List<GitHubContents>>(folderContentsResponse.SerializedData);
            if (deserializedFolderContents == null)
            {
                var result = new Result<Folder>(false, Errors.CantReadRootFolderContents, null);
                return result;
            }

            int linesOfCode = 0;
            int linesOfWhitespace = 0;
            var filesystemObjects = new List<FilesystemObject>();
            foreach (var content in deserializedFolderContents)
            {

                if (content.Type == Filetypes.Dir)
                {
                    var subfolderData = await GetFolderContents(content);
                    if (!subfolderData.Succeeded || subfolderData == null)
                    {
                        var result = new Result<Folder>(
                            false,
                            subfolderData == null ? Errors.Unexpected : subfolderData.ErrorMessage,
                            null
                            );
                        return result;
                    }

                    var subfolder = subfolderData.Value;
                    if (subfolder == null)
                    {
                        var result = new Result<Folder>(false, Errors.CantReadFolderContents, null);
                        return result;
                    }

                    filesystemObjects.Add(subfolder);
                    linesOfCode += subfolder.LinesOfCode;
                    linesOfWhitespace += subfolder.LinesOfWhitespace;

                }
                else
                {
                    var fileData = await GetFile(content);
                    if (!fileData.Succeeded || fileData == null)
                    {
                        var result = new Result<Folder>(
                            false,
                            fileData == null ? Errors.Unexpected : fileData.ErrorMessage,
                            null
                            );
                        return result;
                    }

                    var file = fileData.Value;
                    if (file == null)
                    {
                        var result = new Result<Folder>(false, Errors.CantReadFileContents, null);
                        return result;
                    }

                    filesystemObjects.Add(file);
                    linesOfCode += file.LinesOfCode;
                    linesOfWhitespace += file.LinesOfWhitespace;

                }

            }

            var folder = new Folder(
                input.Name,
                linesOfCode,
                linesOfWhitespace,
                filesystemObjects
                );

            return new Result<Folder>(true, null, folder);
        }

        private async Task<Result<Models.File>> GetFile(GitHubContents content)
        {

            if(content.Type == Filetypes.Dir)
            {
                return new Result<Models.File>(false, Errors.IncorrectType, null);
            }

            var fileHtmlResponse = await _gitHubClient.GetPageHtml(content.HtmlUrl);
            if (!fileHtmlResponse.Succeeded || string.IsNullOrEmpty(fileHtmlResponse.HtmlData))
            {
                var result = new Result<Models.File>(false, HandleError(fileHtmlResponse), null);
                return result;
            }

            var htmlDocumentParser = new HtmlDocumentParser(fileHtmlResponse.HtmlData);

            var fileData = htmlDocumentParser.ExtractLineCountFromHtml();
            if (fileData == null)
            {
                //special case, not being able to parse the html for line count does not warrant a crash
                var blankFile = new Models.File(
                    content.Name,
                    0,
                    0,
                    content.Name
                    );
                var result = new Result<Models.File>(true, null, blankFile);
                return result;
            }

            var file = new Models.File(
                content.Name,
                fileData.LinesOfCode, 
                fileData.LinesOfWhitespace,
                content.Name //todo: logic for determining filetype
                );
            return new Result<Models.File>(true, null, file);

        }

        public void GetLargestFiles(Folder folder, PriorityQueue<Models.File, int> priorityQueue)
        {

            foreach(var filesystemObject in folder.FilesystemObjects)
            {
                if(filesystemObject is Models.File file)
                {
                    if(file.LinesOfCode > 0)
                    {
                        priorityQueue.Enqueue(file, file.LinesOfCode);
                    }
                    while(priorityQueue.Count > _numberOfLargestFiles)
                    {
                        priorityQueue.Dequeue();
                    }
                }
                else if(filesystemObject is Folder subfolder)
                {
                    GetLargestFiles(subfolder, priorityQueue);
                }
            }
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

    public class FileData 
    {
        public int LinesOfCode { get; }
        public int LinesOfWhitespace { get; }

        public FileData(int linesOfCode, int linesOfWhitespace)
        {
            LinesOfCode = linesOfCode;
            LinesOfWhitespace = linesOfWhitespace;
        }
    }


}
