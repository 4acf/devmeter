using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DevMeter.Core.Github.Models
{
    public class GitHubApiResponse(bool succeeded, string? errorMessage = null, string? serializedData = null)
    {
        public bool Succeeded => succeeded;
        public string? ErrorMessage => errorMessage;
        public string? SerializedData => serializedData;
    }
}
