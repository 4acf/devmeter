using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace devmeter.core.Github.Models
{
    public class GitHubHtmlResponse
    {
        public bool Succeeded { get; set; }
        public string? ErrorMessage { get; set; }
        public string? HtmlData { get; set; }
        public HttpStatusCode? StatusCode { get; set; }

        public GitHubHtmlResponse(bool succeeded, HttpStatusCode? statusCode = null, string? errorMessage = null, string? htmlData = null)
        {
            Succeeded = succeeded;
            StatusCode = statusCode;
            ErrorMessage = errorMessage;
            HtmlData = htmlData;
        }
    }
}
