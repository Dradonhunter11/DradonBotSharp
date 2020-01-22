using System;
using System.Collections.Generic;
using System.Text;
using DradonBotSharp.Enums;

namespace DradonBotSharp.Core
{
    public class GuildConfig
    {
        public List<GuildTags> tags;
        public ulong guildID;

	    public string prefix;

        public string welcomeMessage = "";

        public Dictionary<string, bool> permissionSet;

        public ulong loggingChannelID = 0;

        public GuildConfig(ulong guildID)
        {
            this.guildID = guildID;
            this.prefix = "db!";

            permissionSet = new Dictionary<string, bool>()
            {
                ["WelcomeMessageManagePermission"] = false,
                ["ChangePrefixPermission"] = false,
                ["DraconicDungeonRPGPermission"] = false,
                ["HastebinPermission"] = false,
                ["LoggingPermission"] = false,
                ["WelcomeMessagePermission"] = false
            };
        }
    }
}
