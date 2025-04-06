using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DevMeter.Core.Github.Models
{
    public record class GitHubApiError(
        [property: JsonPropertyName("message")] string Message,
        [property: JsonPropertyName("status")] string Status,
        [property: JsonPropertyName("documentation_url")] string DocumentationUrl
    );
}
