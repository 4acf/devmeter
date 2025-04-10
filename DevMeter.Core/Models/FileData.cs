namespace DevMeter.Core.Models
{
    public class FileData(int linesOfCode, int linesOfWhitespace)
    {
        public int LinesOfCode => linesOfCode;
        public int LinesOfWhitespace => linesOfWhitespace;
    }
}
