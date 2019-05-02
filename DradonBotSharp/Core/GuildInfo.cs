using System;
using System.Collections.Generic;
using System.Text;
using DradonBotSharp.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DradonBotSharp.Core
{
    internal sealed class GuildInfo
    {
        public static List<GuildInfo> guildInfoList = new List<GuildInfo>();

        [JsonConverter(typeof(StringEnumConverter))]
        private List<GuildTags> guildTags = new List<GuildTags>();
        private ulong GuildID;



        
    }
}
