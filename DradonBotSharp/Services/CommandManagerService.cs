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
            _mySqlDatabase = service.GetRequiredService<SQLDatabaseService>();
            _json = service.GetRequiredService<JsonService>();
            _services = service;

            _socketClient.GuildMemberUpdated += UpdateRoleEvent;
            _socketClient.MessageReceived += MessageReceivedAsync;
            _socketClient.ReactionAdded += AddRectionEvent;
        }

        public async Task Initialize()
        {
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }

        public async Task MessageReceivedAsync(SocketMessage rawMessage)
        {
            
            if (!(rawMessage is SocketUserMessage message)) return;
            if (message.Source != MessageSource.User) return;

            var argPos = 0;

            if (!message.HasStringPrefix("()", ref argPos)) return;

            var context = new SocketCommandContext(_socketClient, message);

            await _commands.ExecuteAsync(context, argPos, _services);
        }

        public async Task UpdateRoleEvent(SocketGuildUser userBeforeUpdate, SocketGuildUser userAfterUpdate)
        {

        }

        public async Task AddRectionEvent(Cacheable<IUserMessage, ulong> MessageId, ISocketMessageChannel channel,
            SocketReaction reaction)
        {
            var guildChannel = channel as SocketGuildChannel;
            try
            {
                
                Console.WriteLine("---- An emote as been detected ----");
                Console.WriteLine($"Guild Data : {guildChannel.Guild.Name}");
                Console.WriteLine($"Guild ID : {guildChannel.Guild.Id}");
                Console.WriteLine($"Channel Data : {channel.Name}");
                Console.WriteLine($"Channel ID : {channel.Id}");
                Console.WriteLine($"Message Data : {channel.GetMessageAsync(MessageId.Id).Result.Content}");
                Console.WriteLine($"Message ID : {MessageId.Value} : {MessageId.Id}");
                Console.WriteLine($"User Data : {channel.GetMessageAsync(MessageId.Id).Result.Author.Username}");
                Console.WriteLine($"User ID : {channel.GetMessageAsync(MessageId.Id).Result.Author.Id}");
                Console.WriteLine($"Emote data : {reaction.Emote.Name}");
                Console.WriteLine("-----------------------------------");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            

            if (reaction.Emote.Name == "⭐" && _json.IsChannelFeatured((long) channel.Id))
            {
                Console.WriteLine("it's a star!");

                var message = channel.GetMessageAsync(MessageId.Id).Result as IUserMessage;
                try
                {

                    if (_json.MetReactionRequirement(message, message.Reactions[reaction.Emote].ReactionCount))
                    {
                        EmbedBuilder builder = new EmbedBuilder();
                        EmbedAuthorBuilder author = new EmbedAuthorBuilder();
                        author.Name = message.Author.Username;
                        author.IconUrl = message.Author.GetAvatarUrl();
                        builder.WithAuthor(author);
                        if(message.Content != "")
                            builder.AddField(BotUtils.CreateEmbdedField("Message Content", message.Content, false));
                        if(message.Attachments.Count != 0)
                            builder.WithImageUrl(message.Attachments.ToList()[0].ProxyUrl);
                        builder.WithUrl(message.GetJumpUrl());
                        builder.WithTitle("Go to the message");
                        await guildChannel.Guild.GetTextChannel((ulong) _json.GetFeaturedChannel((long) channel.Id))
                            .SendMessageAsync("", false, builder.Build());
                        _json.AddMessageID((long)channel.Id,(long)message.Id);
                        _json.Saveconfig();
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
