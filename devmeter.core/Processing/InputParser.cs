using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace devmeter.core.Processing
{
    public static partial class InputParser
    {
        [GeneratedRegex("^/[^/]+/[^/]+$", RegexOptions.IgnoreCase)]
        private static partial Regex _repoSearchRegex();

        public static bool TryParse(string? input, out string? output)
        {

            output = null;

            if (string.IsNullOrEmpty(input))
            {
                Debug.WriteLine("Input error: No input provided");
                return false;
            }

            if (Uri.TryCreate(input, UriKind.Absolute, out var searchUri) == false)
            {
                Debug.WriteLine("Input error: Invalid URI");
                return false;
            }

            if (searchUri.Host != "github.com" && searchUri.Host != "www.github.com")
            {
                Debug.WriteLine("Input error: Invalid host");
                return false;
            }

            if (!_repoSearchRegex().IsMatch(searchUri.AbsolutePath))
            {
                Debug.WriteLine("Input error: Absolute path of search URI does not match the format '/OWNER/REPO'");
                return false;
            }

            output = searchUri.AbsolutePath;
            return true;

        }
    }
}
