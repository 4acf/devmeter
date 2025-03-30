using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace devmeter.ui.Core.Models
{
    public interface IFilesystemObject
    {
        string Name { get; set; }
        bool Disabled { get; set; }
        int LinesOfCode { get; set; }
        int LinesOfWhitespace { get; set; }
    }
}
