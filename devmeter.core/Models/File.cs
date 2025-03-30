using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace devmeter.ui.Core.Models
{
    public class File : IFilesystemObject
    {
        public string Name { get; set; } = string.Empty;
        public bool Disabled { get; set; }
        public string Filetype { get; set; } = string.Empty;
        public int LinesOfCode { get; set; }
        public int LinesOfWhitespace { get; set; }
    }
}
