namespace DevMeter.Core.Models
{
    public abstract class FilesystemObject(string name, int linesOfCode, int linesOfWhitespace)
    {
        public string Name => name;
        public int LinesOfCode => linesOfCode;
        public int LinesOfWhitespace => linesOfWhitespace;
    }
}
