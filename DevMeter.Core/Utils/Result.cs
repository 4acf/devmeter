using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevMeter.Core.Utils
{
    public class Result<T>(bool succeeded, string? errorMessage, T? value)
    {
        public bool Succeeded => succeeded;
        public string? ErrorMessage => errorMessage;
        public T? Value => value;
    }
}
