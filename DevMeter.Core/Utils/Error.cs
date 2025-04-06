using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevMeter.Core.Utils
{
    public static class Errors
    {
        public const string CantReadTopContributors = "Error reading top contributors";
        public const string CantReadRootFolderContents = "Error reading contents of root folder";
        public const string CantReadFolderContents = "Error reading contents of a subfolder";
        public const string CantReadFileContents = "Error reading contents of a file";
        public const string CantReadLineCount = "Error reading line count";
        public const string IncorrectType = "Error reading file tree: an incorrect type was used";
        public const string Unexpected = "Unexpected error";
    }
}
