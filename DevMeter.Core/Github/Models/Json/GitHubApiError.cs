using System.Text.Json.Serialization;

namespace DevMeter.Core.Github.Models.Json
{
    public record class GitHubApiError(
        [property: JsonPropertyName("message")] string Message,
        [property: JsonPropertyName("status")] string Status,
        [property: JsonPropertyName("documentation_url")] string DocumentationUrl
    );
}
