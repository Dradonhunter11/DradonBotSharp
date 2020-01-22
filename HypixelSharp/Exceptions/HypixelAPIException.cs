using System;
using System.Collections.Generic;
using System.Text;

namespace HypixelSharp.Exceptions
{
    class HypixelAPIException : Exception
    {
        public HypixelAPIException()
        {
        }

        public HypixelAPIException(string message) : base(message)
        {
        }

        public HypixelAPIException(string message, Exception cause) : base(message, cause)
        {
        }
    }
}
