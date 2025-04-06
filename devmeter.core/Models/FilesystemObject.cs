using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevMeter.Core.Models
{
    public abstract class FilesystemObject
    {
        public string Name { get; }
        public int LinesOfCode { get; }
        public int LinesOfWhitespace { get; }

        protected FilesystemObject(string name, int linesOfCode, int linesOfWhitespace)
        {
            Name = name;
            LinesOfCode = linesOfCode;
            LinesOfWhitespace = linesOfWhitespace;
        }

    }
}
