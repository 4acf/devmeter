﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace devmeter.core.Github.Models
{
    public record class GitHubCommit(
        [property: JsonPropertyName("sha")] string Sha
    );
}
