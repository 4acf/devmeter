using System.Net;

namespace DevMeter.Core.Github.Models
{
    public class GitHubHtmlResponse(bool succeeded, HttpStatusCode? statusCode = null, string? errorMessage = null, string? htmlData = null)
    {
        public bool Succeeded => succeeded;
        public HttpStatusCode? StatusCode => statusCode;
        public string? ErrorMessage => errorMessage;
        public string? HtmlData => htmlData;
    }
}
