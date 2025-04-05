using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace devmeter.Core.Models
{
    public class Repo
    {
        public string Name { get; set; } = string.Empty;
        public string Commits { get; set; } = string.Empty;
        public int CommitsInLast30Days { get; set; }
        public string Contributors { get; set; } = string.Empty;
        public List<Contributor> TopContributors { get; set; } = [];
        public Folder? Root { get; set; }
    }
}
