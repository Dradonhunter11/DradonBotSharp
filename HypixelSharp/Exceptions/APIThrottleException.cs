using System;
using System.Collections.Generic;
using System.Text;

namespace HypixelSharp.Exceptions
{
    class APIThrottleException : HypixelAPIException
    {
        public APIThrottleException() : base("You have passed the API throttle limit!")
        {
        }
    }
}
