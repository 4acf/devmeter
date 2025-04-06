using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevMeter.Core.Utils
{
    public class Result<T>
    {
        public bool Succeeded { get; }
        public string? ErrorMessage { get; }
        public T? Value { get; }

        public Result(bool succeeded, string? errorMessage, T? value)
        {
            Succeeded = succeeded;
            ErrorMessage = errorMessage;
            Value = value;
        }

    }
}
