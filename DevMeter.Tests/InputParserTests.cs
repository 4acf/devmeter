using DevMeter.Core.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevMeter.Tests
{
    public class InputParserTests
    {

        [Fact]
        public void TryParse_HttpsAbsoluteUrl_ReturnsTrue()
        {
            string url = "https://github.com/4acf/devmeter";

            bool result = InputParser.TryParse(url, out _);

            Assert.True(result);
        }

        [Fact]
        public void TryParse_WwwAbsoluteUrl_ReturnsTrue()
        {
            string url = "https://www.github.com/4acf/devmeter";

            bool result = InputParser.TryParse(url, out _);

            Assert.True(result);
        }

        [Fact]
        public void TryParse_ValidUrl_OutputsRepoHandle()
        {
            string url = "https://github.com/4acf/devmeter";

            _ = InputParser.TryParse(url, out var result);

            Assert.Contains("4acf/devmeter", result);
        }

        [Fact]
        public void TryParse_EmptyString_ReturnsFalse()
        {
            bool result = InputParser.TryParse(string.Empty, out _);

            Assert.False(result);
        }

        [Fact]
        public void TryParse_EmptyString_OutputsEmptyStringError()
        {
            _ = InputParser.TryParse(string.Empty, out var result);

            Assert.Contains("Please provide an input", result);
        }

        [Fact]
        public void TryParse_NonUrlString_ReturnsFalse()
        {
            var url = "url";

            bool result = InputParser.TryParse(url, out _);

            Assert.False(result);
        }

        [Fact]
        public void TryParse_NonUrlString_OutputsNonUrlError()
        {
            var url = "url";

            _ = InputParser.TryParse(url, out var result);

            Assert.Contains("Invalid URI", result);
        }

        [Fact]
        public void TryParse_NonGitHubUrl_ReturnsFalse()
        {
            var url = "https://google.com";

            bool result = InputParser.TryParse(url, out _);

            Assert.False(result);
        }

        [Fact]
        public void TryParse_NonGitHubUrl_OutputsNonUrlError()
        {
            var url = "https://google.com";

            _ = InputParser.TryParse(url, out var result);

            Assert.Contains("Invalid host", result);
        }

        [Fact]
        public void TryParse_NonRepoUrl_ReturnsFalse()
        {
            string url = "https://github.com/4acf/";

            bool result = InputParser.TryParse(url, out _);

            Assert.False(result);
        }

        [Fact]
        public void TryParse_NonRepoUrl_ReturnsRegexError()
        {
            string url = "https://github.com/4acf/";

            _ = InputParser.TryParse(url, out var result);

            Assert.Contains("Path of search URI must match the format '/OWNER/REPO'", result);
        }

    }
}
