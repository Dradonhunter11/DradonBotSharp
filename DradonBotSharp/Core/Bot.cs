using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using DradonBotSharp.Services;

namespace DradonBotSharp.Core
{
    internal class Bot
    {
        private DiscordSocketClient _client;
        private IServiceProvider _serviceProvider;

        public String BotName => "Dradon bot"; //replace this here with the name of your bot

        public static readonly Bot instance = new Bot();

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
                .AddSingleton<SQLDatabaseService>()
                .AddSingleton<JsonService>()
                .BuildServiceProvider();
        }

        internal async Task Main()
        {
            using (var services = ConfigureServices())
            {
                _client = services.GetRequiredService<DiscordSocketClient>();
                _serviceProvider = services;

                await _client.LoginAsync(TokenType.Bot, "MzMzMDQxMTA2MzYwOTkxNzQ2.DMaxSw.Ds4jdDV843fn-MRU7HqyXaiN5Yk", false);
                await _client.StartAsync();

                await _serviceProvider.GetRequiredService<CommandManagerService>().Initialize();
                await _serviceProvider.GetRequiredService<SQLDatabaseService>().Initialize();
                await _serviceProvider.GetService<JsonService>().Initialize();

                await Task.Delay(-1);
            }
        }

        internal Embed BotInfo()
        {
            EmbedBuilder builder = new EmbedBuilder();
            builder.WithAuthor("Dradonhunter11");
            builder.WithAuthor($"{Bot.instance.BotName} \n\nDradonBotSharp is made by Dradonhunter11 using Discord.Net API");
            builder.WithColor(Color.Gold);
            builder.AddField(BotUtils.CreateEmbdedField("Version", new Version(0, 0, 1), true));
            return builder.Build();
        }
    }
}
