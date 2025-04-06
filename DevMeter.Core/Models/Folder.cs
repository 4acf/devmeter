using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevMeter.Core.Models
{
    public class Folder : FilesystemObject
    {
        public List<FilesystemObject> FilesystemObjects { get; }

        public Folder(string name, int linesOfCode, int linesOfWhitespace, List<FilesystemObject> filesystemObjects) 
            : base(name, linesOfCode, linesOfWhitespace)
        {
            FilesystemObjects = filesystemObjects;
        }

    }
}
