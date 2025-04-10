using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DevMeter.Core.Github.Models.Json
{
    public record class GitHubContributor(
        [property: JsonPropertyName("login")] string Username,
        [property: JsonPropertyName("contributions")] int Contributions
    );
}
