using System;
using System.Collections.Generic;
using System.Text;
using HypixelSharp.reply;
using Newtonsoft.Json.Linq;

namespace HypixelSharp.Reply.Skyblock
{
    public class SkyBlockProfileReply : AbstractReply
    {
        public JObject profile;

        public JObject GetProfiles()
        {
            if (profile == null)
            {
                return null;
            }

            return profile;
        }
    }
}
