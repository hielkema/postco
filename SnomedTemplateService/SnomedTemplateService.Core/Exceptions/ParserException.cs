using System;
using System.Collections.Generic;
using System.Text;

namespace SnomedTemplateService.Core.Exceptions
{
    public class ParserException : Exception
    {
        public ParserException()
        {
        }

        public ParserException(string message) : base(message)
        { 
        }

        public ParserException(string message, Exception inner)
        : base(message, inner)
        {
        }
    }
}
