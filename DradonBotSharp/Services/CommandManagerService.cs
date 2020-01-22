using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using DradonBotSharp.Core;
using DradonBotSharp.Core.Config;
using Microsoft.Extensions.DependencyInjection;

namespace DradonBotSharp.Services
{
    class CommandManagerService
    {
        private readonly Bot _bot;
        private readonly DiscordSocketClient _socketClient;
        private readonly IServiceProvider _services;
        private readonly CommandService _commands;
        private readonly SQLDatabaseService _mySqlDatabase;
        private readonly JsonService _json;


        public CommandManagerService(IServiceProvider service)
        {
            _bot = service.GetRequiredService<Bot>();
            _socketClient = service.GetRequiredService<DiscordSocketClient>();
            _commands = service.GetRequiredService<CommandService>();
            //_mySqlDatabase = service.GetRequiredService<SQLDatabaseService>();
            _json = service.GetRequiredService<JsonService>();
            _services = service;

            _socketClient.GuildMemberUpdated += UpdateRoleEvent;
            _socketClient.MessageReceived += MessageReceivedAsync;
            _socketClient.ReactionAdded += AddRectionEvent;
            _socketClient.UserJoined += SocketClientOnUserJoined;
            
        }

        private async Task SocketClientOnUserJoined(SocketGuildUser arg)
        {
            try
            {
                /*if (arg.Guild.Id == 574595004064989214l)
                {
					await arg.SendMessageAsync("**Welcome to the tmodloader 64bit discord!**\n" +
										 "If you have any issue, first consult the following documentation. If you still have issue, you can go in #support to report your issue and people will attempt to help you in a short delay.\n" +
										 "**Regular tModLoader github wiki** : <https://github.com/blushiemagic/tModLoader/wiki>\r\n" +
										 "**tModLoader FNA github repository** : <https://github.com/Dradonhunter11/tModLoader/tree/x64>\r\n" +
										 "**tModLoader broken mod list** : <https://docs.google.com/document/d/1D9pQpr3Hm5wGvP4kI7DKIhMUbS6T9fshkp8Yv4iJx-A/>\r\n\n" +
										 "If you come from fury tutorial and it ask you for registry key or you don't see some mod, pls consult <#574702758133760010> and update your game\r\n" +
										 "If you are getting `System.IO.IOException: Cannot create a file when that file already exists.`, delete the content of your log folder at `%UserProfile%\\My Games\\Terraria\\ModLoader`\r\n" +
					                     "Terra custom do not work with this as it is a 32bit app\r\n" +
					                     "If you get none of these issue above, please send your log file, otherwise we won't help you (require log policy)");
                }*/

                EmbedBuilder builder = new EmbedBuilder();
                builder.WithAuthor(arg.Username);
                builder.WithThumbnailUrl(arg.GetAvatarUrl());
                builder.WithColor(Color.Green);
                builder.AddField(BotUtils.CreateEmbdedField("User", arg.Username));
                await GuildConfigManager.instance.Log(Bot.Instance.socket.GetGuild(arg.Guild.Id).GetTextChannel(GuildConfigManager.instance.config[arg.Guild.Id].loggingChannelID), "New user joined the server", builder.Build());

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        public async Task Initialize()
        {
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }

        public async Task MessageReceivedAsync(SocketMessage rawMessage)
        {

            if (!(rawMessage is SocketUserMessage message)) return;
            if (message.Source != MessageSource.User) return;

            var context = new SocketCommandContext(_socketClient, message);

            if (!GuildConfigManager.instance.GuildHaveConfig(context.Guild.Id))
            {
                GuildConfigManager.instance.CreateGuildConfig(context.Guild.Id);
            }

            var argPos = 0;

            if (!message.HasStringPrefix(GuildConfigManager.instance.GetPrefix(context.Guild.Id), ref argPos)) return;

            await _commands.ExecuteAsync(context, argPos, _services);
        }

        public async Task UpdateRoleEvent(SocketGuildUser userBeforeUpdate, SocketGuildUser userAfterUpdate)
        {

        }

        public async Task AddRectionEvent(Cacheable<IUserMessage, ulong> MessageId, ISocketMessageChannel channel,
            SocketReaction reaction)
        {
            var guildChannel = channel as SocketGuildChannel;


            if (reaction.Emote.Name == "⭐" && _json.IsChannelFeatured(channel.Id))
            {
                Console.WriteLine("it's a star!");

                var message = channel.GetMessageAsync(MessageId.Id).Result as IUserMessage;
                try
                {

                    if (_json.MetReactionRequirement(channel.Id, message.Reactions[reaction.Emote].ReactionCount) && !_json.IsAlreadyFeatured(message))
                    {
                        EmbedBuilder builder = new EmbedBuilder();
                        EmbedAuthorBuilder author = new EmbedAuthorBuilder();
                        author.Name = message.Author.Username;
                        author.IconUrl = message.Author.GetAvatarUrl();
                        builder.WithAuthor(author);
                        if (message.Content != "")
                            builder.AddField(BotUtils.CreateEmbdedField("Message Content", message.Content, false));
                        if (message.Attachments.Count != 0)
                            builder.WithImageUrl(message.Attachments.ToList()[0].ProxyUrl);
                        builder.WithUrl(message.GetJumpUrl());
                        builder.WithTitle("Go to the message");
                        await guildChannel.Guild.GetTextChannel(_json.GetFeaturedChannel(channel.Id))
                            .SendMessageAsync("", false, builder.Build());
                        _json.AddMessageID(channel.Id, message.Id);
                        _json.SaveFeaturedChannelConfig();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

            }
        }
    }
}
