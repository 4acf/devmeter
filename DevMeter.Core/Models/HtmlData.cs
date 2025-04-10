using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevMeter.Core.Models
{
    public class HtmlData(string commits, string contributors)
    {
        public string Commits => commits;
        public string Contributors => contributors;
    }
}
