using System.Text.Json.Serialization;

namespace DevMeter.Core.Github.Models.Json
{
    public record class GitHubContributor(
        [property: JsonPropertyName("login")] string Username,
        [property: JsonPropertyName("contributions")] int Contributions
    );
}
