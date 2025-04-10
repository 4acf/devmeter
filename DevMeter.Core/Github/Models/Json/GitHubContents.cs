using System.Text.Json.Serialization;

namespace DevMeter.Core.Github.Models.Json
{
    public record class GitHubContents(
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("url")] string Url,
        [property: JsonPropertyName("html_url")] string HtmlUrl,
        [property: JsonPropertyName("type")] string Type
    );
}
