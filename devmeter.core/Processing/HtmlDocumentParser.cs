using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace DevMeter.Core.Processing
{

    public class HtmlDocumentParser
    {

        private static readonly HashSet<char> _numberChars =
        [
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
            ',',
        ];

        private readonly HtmlDocument _htmlDocument;

        public HtmlDocumentParser(string html)
        {
            _htmlDocument = new();
            _htmlDocument.LoadHtml(html);
        }

        public string ExtractCommitsFromHtml()
        {

            var spans = _htmlDocument.DocumentNode.Descendants("span")
                .Where(span => span.GetAttributeValue("class", "").Contains("fgColor-default"))
                .ToList();

            foreach(var span in spans)
            {
                if (span.InnerText.Contains("Commit"))
                {
                    var sb = new StringBuilder();
                    foreach(var letter in span.InnerText)
                    {
                        if (!_numberChars.Contains(letter))
                            break;
                        sb.Append(letter);
                    }
                    return sb.ToString();
                }
            }

            return string.Empty;

        }

        public string ExtractContributorsFromHtml()
        {

            var spans = _htmlDocument.DocumentNode.Descendants("span")
                .Where(span => span.GetAttributeValue("class", "").Contains("Counter ml-1"))
                .ToList();

            foreach(var span in spans)
            {
                if (span.ParentNode.InnerText.Contains("Contributors"))
                    return span.InnerText;
            }

            return string.Empty;
            
        }

        public FileData? ExtractLineCountFromHtml()
        {
            var locSpan = _htmlDocument.DocumentNode.Descendants("span")
                .Where(span => span.InnerText.Contains("lines") && span.InnerText.Contains("loc"))
                .FirstOrDefault();

            if (locSpan == null)
            {
                return null;
            }
                
            var innerText = locSpan.InnerText;
            var tokens = innerText.Split(' ', '(');

            if (!Int32.TryParse(tokens[0], out int linesOfCode) || !Int32.TryParse(tokens[3], out int sourceLinesOfCode))
            {
                return null;
            }

            return new FileData(sourceLinesOfCode, linesOfCode - sourceLinesOfCode);

        }

    }
}
