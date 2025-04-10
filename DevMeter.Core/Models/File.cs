namespace DevMeter.Core.Models
{
    public class File(string name, int linesOfCode, int linesOfWhitespace, string filetype)
            : FilesystemObject(name, linesOfCode, linesOfWhitespace)
    {
        public string Filetype => filetype;
    }
}
