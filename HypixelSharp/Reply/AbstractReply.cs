using System;
using System.Collections.Generic;
using System.Text;

namespace HypixelSharp.reply
{
    public abstract class AbstractReply
    {
        protected bool throttle;
        protected bool success;
        protected string cause;

        public bool isThrottle()
        {
            return throttle;
        }

        public bool isSuccess()
        {
            return success;
        }

        public string getCause()
        {
            return cause;
        }

        
        public override string ToString()
        {
            return "AbstractReply{" +
                    "throttle=" + throttle +
                    ", success=" + success +
                    ", cause='" + cause + '\'' +
                    '}';
        }
    }
}
