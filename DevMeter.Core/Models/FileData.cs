using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevMeter.Core.Models
{
    public class FileData(int linesOfCode, int linesOfWhitespace)
    {
        public int LinesOfCode => linesOfCode;
        public int LinesOfWhitespace => linesOfWhitespace;
    }
}
