using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using DradonBotSharp.Core;
using DradonBotSharp.Services;

namespace DradonBotSharp.Modules.CoreModule
{
    public partial class CoreModule : ModuleBase<SocketCommandContext>
    {

        public JsonService json { get; set; }

        [Command("info")]
        [Summary("Give the information about the bot")]
        public async Task BotInfo() => await ReplyAsync("", false, Bot.Instance.BotInfo());

        [Command("ping")]
        public async Task Pong() => await ReplyAsync("pong");

        

        [Command("FeaturedChannel")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task FeatureChannel(string submitchannelID, string featuredchannelID, string emote, int numberOfReaction)
        {
            ulong submitID = ulong.Parse(submitchannelID.Replace("<#", "").Replace(">", ""));
            ulong featuredID = ulong.Parse(featuredchannelID.Replace("<#", "").Replace(">", ""));
            await ReplyAsync(json.AddChannel(submitID, featuredID, emote, numberOfReaction));
        }

        [Command("SetNick")]
        [RequireUserPermission(GuildPermission.ManageNicknames)]
        public async Task SetNickname(string name)
        {
            var guild = Bot.Instance.socket.GetGuild(Context.Guild.Id);
            var user = guild.GetUser(Bot.Instance.socket.CurrentUser.Id);
            await user.ModifyAsync(x => {
                x.Nickname = name;
            });
            await ReplyAsync($"Nickname of the bot has been changed");
        }
    }
}
