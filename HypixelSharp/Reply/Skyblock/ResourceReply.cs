using HypixelSharp.reply;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace HypixelSharp.Reply.Skyblock
{
    class ResourceReply : AbstractReply
    {
        private JObject response;

        public ResourceReply(JObject response) {
            this.response = response;
            this.success = response.ContainsKey("success") && response.GetValue("success").Value<bool>();
            this.cause = response.ContainsKey("cause") ? response.GetValue("cause").Value<string>() : null;
        }

        public JObject GetResponse()
        {
            if (response == null)
            {
                return null;
            }

            return response;
        }


    }
}
