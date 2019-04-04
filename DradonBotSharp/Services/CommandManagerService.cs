using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
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

        public CommandManagerService(IServiceProvider service)
        {
            _bot = service.GetRequiredService<Bot>();
            _socketClient = service.GetRequiredService<DiscordSocketClient>();
            _commands = service.GetRequiredService<CommandService>();
            _mySqlDatabase = service.GetRequiredService<SQLDatabaseService>();
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
            Console.WriteLine($"Message Data : {MessageId.Value} : {MessageId.Id}");
            Console.WriteLine($"Emote data : {reaction.Emote.Name}");

            if (reaction.Emote.Name == "⭐")
            {
                Console.WriteLine("it's a star!");
            }
        }
    }
}
