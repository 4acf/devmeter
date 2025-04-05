using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace devmeter.Core.Models
{
    public class Folder : FilesystemObject
    {
        public List<FilesystemObject> FilesystemObjects { get; set; } = [];
    }
}
