using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace devmeter.ui.Core.Models
{
    public class Folder : IFilesystemObject
    {
        public string Name { get; set; } = string.Empty;
        public bool Disabled { get; set; }
        public int LinesOfCode { get; set; }
        public int LinesOfWhitespace { get; set; }
        public List<IFilesystemObject> FilesystemObjects { get; set; } = [];
    }
}
