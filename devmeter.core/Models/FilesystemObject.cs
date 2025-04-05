using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevMeter.Core.Models
{
    public abstract class FilesystemObject
    {
        string Name { get; set; } = string.Empty;
        int LinesOfCode { get; set; }
        int LinesOfWhitespace { get; set; }
    }
}
