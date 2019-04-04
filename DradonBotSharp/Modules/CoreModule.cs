using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using DradonBotSharp.Core;
using DradonBotSharp.Services;

namespace DradonBotSharp.Modules
{
    public class CoreModule : ModuleBase<SocketCommandContext>
    {
        [Command("info")]
        [Summary("Give the information about the bot")]
        public async Task BotInfo() => await ReplyAsync("", false, Bot.instance.BotInfo());

        [Command("ping")]
        public async Task Pong() => await ReplyAsync("pong");
    }
}
