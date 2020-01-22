using System;
using System.Collections.Generic;
using System.Text;
using HypixelSharp.reply;
using Newtonsoft.Json.Linq;

namespace HypixelSharp.Reply
{
    public class PlayerReply : AbstractReply
    {
        public JObject player;

        public JObject GetResponse()
        {
            if (player == null)
            {
                return null;
            }

            return player;
        }
    }
}
