using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace devmeter.core.Github.Models
{
    public class GitHubApiResponse
    {
        public bool Succeeded { get; set; }
        public string? ErrorMessage { get; set; }
        public string? SerializedData { get; set; }

        public GitHubApiResponse(bool succeeded, string? errorMessage = null, string? serializedData = null)
        {
            Succeeded = succeeded;
            ErrorMessage = errorMessage;
            SerializedData = serializedData;
        }

    }
}
