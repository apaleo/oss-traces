using System;
using System.Collections.Generic;

namespace Traces.Common.Exceptions
{
    public class ValidationException : Exception
    {
        public ValidationException(string message, Exception inner = null)
            : this(new[] { message }, inner)
        {
        }

        public ValidationException(IEnumerable<string> messages, Exception inner = null)
            : base(string.Join(" | ", messages), inner)
        {
        }
    }
}