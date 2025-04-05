using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevMeter.Core.Models
{
    public class File : FilesystemObject
    {
        public string Filetype { get; set; } = string.Empty;
    }
}
