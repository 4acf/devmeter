using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace devmeter.core.Github
{
    public sealed class GitHubClient
    {
        private readonly Uri _baseUrl = new("https://api.github.com", UriKind.Absolute);
    }
}
