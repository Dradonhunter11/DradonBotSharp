using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using DradonBotSharp.Core.Config;

namespace DradonBotSharp.Modules.Config
{
    
    public class ConfigModule : ModuleBase<SocketCommandContext>
    {
        [Command("SetPermission")]
        [RequireUserPermission(GuildPermission.ManageGuild | GuildPermission.Administrator)]
        public async Task SetPermission(string permission)
        {
            await ReplyAsync(GuildConfigManager.instance.SetPermission(Context.Guild.Id, permission));
        }

        [Command("SetPrefix")]
        [RequireUserPermission(GuildPermission.ManageGuild | GuildPermission.Administrator)]
        public async Task SetPrefix(string prefix)
        {
            if (GuildConfigManager.instance.SetPrefix(Context.Guild.Id, prefix))
            {
                await ReplyAsync($"Prefix has been successfully set to `{prefix}`");
                return;
            }
            await ReplyAsync("This guild do not have the permission to change the bot prefix for this guild");
        }

        [Command("SetLoggingChannel")]
        [RequireUserPermission(GuildPermission.ManageGuild | GuildPermission.Administrator)]
        public async Task SetLoggingChannel(IChannel channel)
        {
            if (Context.Guild.Channels.Any(i => i.Id == channel.Id) && GuildConfigManager.instance.SetLoggingChannel(Context.Guild.Id, channel.Id))
            {
                await ReplyAsync($"Logging channel for {Context.Guild.Name} has been set to <#{channel.Id}>");
                return;
            }

            if (!Context.Guild.Channels.Any(i => i.Id == channel.Id))
            {
                await ReplyAsync("The channel you are trying to set does not exist in this server");
                return;
            }

            if (!GuildConfigManager.instance.HasPermission(Context.Guild.Id, "LoggingPermission"))
            {
                await ReplyAsync("This server does not have `LoggingPermission` active, to activate it, do the following command :\n" +
                                 $"`{GuildConfigManager.instance.GetPrefix(Context.Guild.Id)}Permission LoggingPermission`");
                return;
            }

            await ReplyAsync("An error occured while setting the logging channel");
        }
    }
}
