using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

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
