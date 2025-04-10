using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DevMeter.Core.Models
{
    public class File(string name, int linesOfCode, int linesOfWhitespace, string filetype)
            : FilesystemObject(name, linesOfCode, linesOfWhitespace)
    {
        public string Filetype => filetype;
    }
}
