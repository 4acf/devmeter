using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevMeter.Core.Models
{
    public class Folder(string name, int linesOfCode, int linesOfWhitespace, List<FilesystemObject> filesystemObjects) 
        : FilesystemObject(name, linesOfCode, linesOfWhitespace)
    {
        public List<FilesystemObject> FilesystemObjects => filesystemObjects;
    }
}
