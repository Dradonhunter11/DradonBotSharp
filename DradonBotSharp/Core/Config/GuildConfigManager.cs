using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DradonBotSharp.Enums;
using Newtonsoft.Json;

namespace DradonBotSharp.Core.Config
{
    class GuildConfigManager
    {
        public static readonly GuildConfigManager instance = new GuildConfigManager();

        internal Dictionary<ulong, GuildConfig> config;

        private GuildConfigManager()
        {
            config = new Dictionary<ulong, GuildConfig>();
        }

        public bool LoadGuildConfigs()
        {
            try
            {
                Dictionary<string, bool> permissionSet = new GuildConfig(0).permissionSet;

                string path = Path.Combine(Environment.CurrentDirectory, "Config", "guildconfig.json");

                if (!File.Exists(path))
                {
                    return false;
                }

                string json = File.ReadAllText(path);

                config = JsonConvert.DeserializeObject<Dictionary<ulong, GuildConfig>>(json);

                AddNewPermissionToLegacyConfig(permissionSet);

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        private void AddNewPermissionToLegacyConfig(Dictionary<string, bool> permissionSet)
        {
            foreach (string permission in permissionSet.Keys)
            {
                foreach (ulong configKey in config.Keys)
                {
                    if (!config[configKey].permissionSet.ContainsKey(permission))
                    {
                        config[configKey].permissionSet.Add(permission, false);
                    }
                }
            }
        }

        public bool SaveGuildConfigs()
        {
            try
            {
                string json = JsonConvert.SerializeObject(config, Formatting.Indented);
                string path = Path.Combine(Environment.CurrentDirectory, "Config");

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                File.WriteAllText(Path.Combine(path, "guildconfig.json"), json);

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        public bool CheckIfGuildHaveTag(ulong guildID, GuildTags tag)
        {
            if (config.ContainsKey(guildID))
            {
                return config[guildID].tags.Contains(tag);
            }
            return false;
        }

        internal void CreateGuildConfig(ulong guildID)
        {
            if (config.ContainsKey(guildID)) { return; }
            config.Add(guildID, new GuildConfig(guildID));
            SaveGuildConfigs();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="guildID"></param>
        /// <param name="permission"></param>
        /// <returns>
        /// Return message
        /// </returns>
        internal string SetPermission(ulong guildID, string permission)
        {
            if (!config.ContainsKey(guildID)) { return "This guild does not have a config yet."; }

            if (!config[guildID].permissionSet.ContainsKey(permission)) { return "This permission does not exist."; }

            config[guildID].permissionSet[permission] = !config[guildID].permissionSet[permission];
            SaveGuildConfigs();
            return $"**{permission}** has been successfully to `{config[guildID].permissionSet[permission]}`";
        }

        internal bool HasPermission(ulong guildID, string permission)
        {
            if (!config.ContainsKey(guildID)) return false;
            if (!config[guildID].permissionSet.ContainsKey(permission)) return false;

            return config[guildID].permissionSet[permission];
        }

        internal string GetPrefix(ulong guildID)
        {
            if (config.ContainsKey(guildID))
            {
                return config[guildID].prefix;
            }

            return "db!";
        }

        internal bool SetPrefix(ulong guildID, string newPrefix)
        {
            if (config.ContainsKey(guildID) && config[guildID].permissionSet["ChangePrefixPermission"])
            {
                config[guildID].prefix = newPrefix;
                SaveGuildConfigs();
                return true;
            }

            return false;
        }

        internal bool SetLoggingChannel(ulong guildID, ulong channelID)
        {
            if (config.ContainsKey(guildID) && config[guildID].permissionSet["LoggingPermission"])
            {
                config[guildID].loggingChannelID = channelID;
                SaveGuildConfigs();
                return true;
            }
            return false;
        }

        internal async Task Log(SocketCommandContext context, string message = null, Embed embed = null)
        {
            if (config.ContainsKey(context.Guild.Id) 
                && config[context.Guild.Id].permissionSet["LoggingPermission"] 
                && config[context.Guild.Id].loggingChannelID != 0
                && context.Guild.Channels.Any(i => i.Id == config[context.Guild.Id].loggingChannelID))
            {
                await context.Guild.GetTextChannel(config[context.Guild.Id].loggingChannelID).SendMessageAsync(message, false, embed);
            }
        }

        internal async Task Log(SocketTextChannel context, string message = null, Embed embed = null)
        {
            if (config.ContainsKey(context.Guild.Id)
                && config[context.Guild.Id].permissionSet["LoggingPermission"]
                && config[context.Guild.Id].loggingChannelID != 0
                && context.Guild.Channels.Any(i => i.Id == config[context.Guild.Id].loggingChannelID))
            {
                await context.SendMessageAsync(message, false, embed);
            }
        }

        internal bool GuildHaveConfig(ulong guildID)
        {
            return config.ContainsKey(guildID);
        }
    }
}
