using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace devmeter.ui.Core.Models
{
    public class Repo
    {
        public string Name { get; set; } = string.Empty;
        public string Commits { get; set; } = string.Empty;
        public string Contributors { get; set; } = string.Empty;
        public List<Contributor> TopContributors { get; set; } = [];
        public List<IFilesystemObject> FilesystemObjects { get; set; } = [];
    }
}
