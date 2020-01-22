using System;
using System.Collections.Generic;
using System.Text;
using Discord;
using DradonBotSharp.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DradonBotSharp.Core
{
    internal sealed class GuildInfo
    {
        [JsonConverter(typeof(StringEnumConverter))]
        private List<GuildTags> guildTags = new List<GuildTags>();

	    private ulong GuildID;
		
		
    }

	public static class GuildExtension
	{

	}
}
