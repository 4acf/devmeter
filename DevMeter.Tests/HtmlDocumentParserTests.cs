using DevMeter.Core.Models;
using DevMeter.Core.Processing;
using DevMeter.Tests.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevMeter.Tests
{

    public class HtmlDocumentParserTests
    {

        //factory method here in case constructor for HtmlDocumentParser changes (e.g. accepts another variable that can be configured here instead of in the tests)
        private static HtmlDocumentParser MakeHtmlDocumentParser(string html)
        {
            return new HtmlDocumentParser(html);
        }

        public static IEnumerable<object[]> ContainsLineCountData()
        {
            yield return new object[] { HtmlStrings.SmallLineCountHtml, new FileData(8, 0) };
            yield return new object[] { HtmlStrings.LargeLineCountHtml, new FileData(1194, 450) };
        }

        [Fact]
        public void ExtractCommitsFromHtml_ContainsCommitsSpan_ReturnsCommitsAsString()
        {
            var htmlDocumentParser = MakeHtmlDocumentParser(HtmlStrings.LinuxHomePageHtml);

            var result = htmlDocumentParser.ExtractCommitsFromHtml();

            Assert.Contains("1,351,500", result);
        }

        [Fact]
        public void ExtractCommitsFromHtml_NoCommitsSpan_ReturnsNull()
        {;
            var htmlDocumentParser = MakeHtmlDocumentParser(HtmlStrings.EmptyHtml);

            var result = htmlDocumentParser.ExtractCommitsFromHtml();

            Assert.Null(result);
        }

        [Fact]
        public void ExtractCommitsFromHtml_MalformedCommitsSpan_ReturnsEmptyString()
        {
            var html = "<!DOCTYPE html>\r\n<html>\r\n<head>\r\n    <title></title>\r\n</head>\r\n<body>\r\n    <span class=\"fgColor-default\">Commit None</span>\r\n</body>\r\n</html>";
            var htmlDocumentParser = MakeHtmlDocumentParser(html);

            var result = htmlDocumentParser.ExtractCommitsFromHtml();

            Assert.Empty(result!);
        }

        [Theory]
        [InlineData(HtmlStrings.XUnitHomePageHtml, "151")]
        [InlineData(HtmlStrings.LinuxHomePageHtml, "5,000+")]
        public void ExtractContributorsFromHtml_ContainsContributorsSpan_ReturnsContributorsAsString(string html, string expected)
        {
            var htmlDocumentParser = MakeHtmlDocumentParser(html);

            var result = htmlDocumentParser.ExtractContributorsFromHtml();

            Assert.Equal(expected, result);
        }

        [Fact]
        public void ExtractContributorsFromHtml_NoContributorsSpan_ReturnsNull()
        {
            var htmlDocumentParser = MakeHtmlDocumentParser(HtmlStrings.DevMeterHomePageHtml);

            var result = htmlDocumentParser.ExtractContributorsFromHtml();

            Assert.Null(result);
        }

        [Theory]
        [MemberData(nameof(ContainsLineCountData))]
        public void ExtractLineCountFromHtml_ContainsLineCount_ReturnsCorrectLineCounts(string html, FileData expected)
        {
            var htmlDocumentParser = MakeHtmlDocumentParser(html);

            var result = htmlDocumentParser.ExtractLineCountFromHtml();

            Assert.Equivalent(expected, result);
        }

        [Fact]
        public void ExtractLineCountFromHtml_NoLineCount_ReturnsNull()
        {
            var htmlDocumentParser = MakeHtmlDocumentParser(HtmlStrings.DevMeterImageFileHtml);

            var result = htmlDocumentParser.ExtractLineCountFromHtml();

            Assert.Null(result);
        }

        [Fact]
        public void ExtractLineCountFromHtml_MalformedLineCountSpan_ReturnsNull()
        {
            var html = "<!DOCTYPE html>\r\n<html>\r\n<head>\r\n    <title></title>\r\n</head>\r\n<body>\r\n    <span>20 lines 20 loc</span>\r\n</body>\r\n</html>";
            var htmlDocumentParser = MakeHtmlDocumentParser(html);

            var result = htmlDocumentParser.ExtractLineCountFromHtml();

            Assert.Null(result);
        }
    
    }
}
