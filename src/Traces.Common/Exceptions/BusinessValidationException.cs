using System;
using System.Collections.Generic;

namespace Traces.Common.Exceptions
{
    public class BusinessValidationException : Exception
    {
        public BusinessValidationException(string message, Exception inner = null)
            : this(new[] { message }, inner)
        {
        }

        public BusinessValidationException(IEnumerable<string> messages, Exception inner = null)
            : base(string.Join(" | ", messages), inner)
        {
        }
    }
}