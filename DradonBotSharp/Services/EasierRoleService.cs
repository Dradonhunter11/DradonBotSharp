using System;
using System.Collections.Generic;
using System.Text;
using Discord.Commands;
using Discord.WebSocket;
using DradonBotSharp.Core;
using Microsoft.Extensions.DependencyInjection;

namespace DradonBotSharp.Services
{
    class EasierRoleService
    {

        private readonly Bot _bot;
        private readonly DiscordSocketClient _socketClient;
        private readonly IServiceProvider _services;
        private readonly CommandService _commands;


        public EasierRoleService(IServiceProvider service)
        {
            _bot = service.GetRequiredService<Bot>();
            _socketClient = service.GetRequiredService<DiscordSocketClient>();
            _commands = service.GetRequiredService<CommandService>();
            _services = service;
        }
    }
}
