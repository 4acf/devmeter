using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace devmeter.core.Processing
{
    public static class HtmlParser
    {

        private static readonly HashSet<char> _numberChars =
        [
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
            ',',
        ];

        public static string ExtractCommitsFromHtml(string html)
        {
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            var spans = htmlDocument.DocumentNode.Descendants("span")
                .Where(span => span.GetAttributeValue("class", "").Contains("fgColor-default"))
                .ToList();

            foreach(var span in spans)
            {
                if (span.InnerText.Contains("Commits"))
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
    }
}
