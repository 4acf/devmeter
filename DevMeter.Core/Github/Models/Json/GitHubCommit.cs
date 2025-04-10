using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DevMeter.Core.Github.Models.Json
{
    public record class GitHubCommit(
        [property: JsonPropertyName("url")] string Url,
        [property: JsonPropertyName("html_url")] string HtmlUrl,
        [property: JsonPropertyName("commit")] GitHubInternalCommit InternalCommit
    )
    {
        public DateTime GetDate() => InternalCommit.Author.Date;
    }

    public record class GitHubInternalCommit(
        [property: JsonPropertyName("author")] GitHubAuthor Author
    );

    public record class GitHubAuthor(
        [property: JsonPropertyName("date")] DateTime Date
    );
}
