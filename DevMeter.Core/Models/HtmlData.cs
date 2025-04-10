namespace DevMeter.Core.Models
{
    public class HtmlData(string commits, string contributors)
    {
        public string Commits => commits;
        public string Contributors => contributors;
    }
}
