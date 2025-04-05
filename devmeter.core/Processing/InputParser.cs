using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DevMeter.Core.Processing
{
    public static partial class InputParser
    {
        [GeneratedRegex("^/[^/]+/[^/]+$", RegexOptions.IgnoreCase)]
        private static partial Regex _repoSearchRegex();

        public static bool TryParse(string? input, out string output)
        {

            if (string.IsNullOrEmpty(input))
            {
                output = "Please provide an input";
                return false;
            }

            if (Uri.TryCreate(input, UriKind.Absolute, out var searchUri) == false)
            {
                output = "Invalid URI";
                return false;
            }

            if (searchUri.Host != "github.com" && searchUri.Host != "www.github.com")
            {
                output = "Invalid host";
                return false;
            }

            if (!_repoSearchRegex().IsMatch(searchUri.AbsolutePath))
            {
                output = "Path of search URI must match the format '/OWNER/REPO'";
                return false;
            }

            output = searchUri.AbsolutePath;
            return true;

        }
    }
}
