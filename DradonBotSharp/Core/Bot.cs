using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using DradonBotSharp.Core.Config;
using DradonBotSharp.Services;
using DradonBotSharp.Services.FileServices;
using HypixelSharp;

namespace DradonBotSharp.Core
{
    internal class Bot
    {
        private DiscordSocketClient _client;
        private IServiceProvider _serviceProvider;

        public String BotName => "Dradon bot"; //replace this here with the name of your bot

        public static readonly Bot Instance = new Bot();

        public static HypixelAPI hypixelAPI = new HypixelAPI(Guid.Parse("6becb8f4-0209-4c60-b023-45e75b9368ba"));

        public DiscordSocketClient socket => _client;


        private Bot()
        {
        }

        public Bot(IServiceProvider service)
        {

        }

        private ServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<Bot>()
                .AddSingleton<CommandManagerService>()
                .AddSingleton<CommandService>()
                //.AddSingleton<SQLDatabaseService>()
                .AddSingleton<JsonService>()
                .AddSingleton<EasierRoleService>()
                .AddSingleton<HastebinService>()
                .BuildServiceProvider();
        }

        internal async Task Main()
        {
            using (var services = ConfigureServices())
            {
                _client = services.GetRequiredService<DiscordSocketClient>();
                _serviceProvider = services;
                _serviceProvider.GetRequiredService<HastebinService>();

                await _client.LoginAsync(TokenType.Bot, Token.BotToken(), false);
                await _client.StartAsync();

                GuildConfigManager.instance.LoadGuildConfigs();

                await _serviceProvider.GetRequiredService<CommandManagerService>().Initialize();
                //await _serviceProvider.GetRequiredService<SQLDatabaseService>().Initialize();
                await _serviceProvider.GetService<JsonService>().Initialize();

                await Task.Delay(-1);
            }

            
        }

        internal Embed BotInfo()
        {
            EmbedBuilder builder = new EmbedBuilder();
            builder.WithAuthor("Dradonhunter11");
            builder.WithAuthor($"{Bot.Instance.BotName} \n\nDradonBotSharp is made by Dradonhunter11 using Discord.Net API");
            builder.WithColor(Color.Gold);
            builder.AddField(BotUtils.CreateEmbdedField("Version", new Version(0, 0, 1), true));
            return builder.Build();
        }
    }
}
