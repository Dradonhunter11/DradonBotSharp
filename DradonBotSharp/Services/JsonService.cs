using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DradonBotSharp.Core;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DradonBotSharp.Services
{
    public class JsonService
    {
        private readonly Bot _bot;
        private readonly DiscordSocketClient _socketClient;
        private readonly IServiceProvider _services;
        private readonly CommandService _commands;

        private readonly List<SpecialFeaturedChannel> channels;

        public JsonService(IServiceProvider service)
        {
            _bot = service.GetRequiredService<Bot>();
            _socketClient = service.GetRequiredService<DiscordSocketClient>();
            _commands = service.GetRequiredService<CommandService>();
            _services = service;

            channels = new List<SpecialFeaturedChannel>();
        }

        public void Saveconfig()
        {
            string json = JsonConvert.SerializeObject(channels, Formatting.Indented);
            string jsonPath = Path.Combine(Environment.CurrentDirectory, "json");

            if (!Directory.Exists(Path.Combine(Environment.CurrentDirectory, "json")))
            {
                Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, "json"));
            }

            File.WriteAllText(Path.Combine(jsonPath, "feature.json"), json, Encoding.Unicode);
        }

        public string AddChannel(long submit, long featured, string emote, int amount)
        {
            if (channels.Any(i => i.SubmitChannel == submit))
            {
                return "A featured channel has already been bound to this channel";
            }

            SpecialFeaturedChannel newChannel = new SpecialFeaturedChannel
            {
                SubmitChannel = submit,
                FeaturedChannel = featured,
                emoteID = emote,
                minimumReactionRequirement = amount
            };
            channels.Add(newChannel);
            Saveconfig();
            return $"This channel is now featured in <#{featured}> with the emmote {emote}";
        }

        public async Task Initialize()
        {
            string jsonPath = Path.Combine(Environment.CurrentDirectory, "json");

            if (!Directory.Exists(jsonPath))
            {
                return;
            }

            JsonTextReader reader;
            if (File.Exists(Path.Combine(jsonPath, "feature.json")))
            {
                reader = new JsonTextReader(new StringReader(File.OpenText(Path.Combine(jsonPath, "feature.json")).ReadToEnd()));
                JArray array = JArray.Load(reader);
                foreach (var value in array)
                {
                    JsonSerializer serializer = JsonSerializer.Create();
                    channels.Add(serializer.Deserialize<SpecialFeaturedChannel>(value.CreateReader()));
                    
                }

                foreach (var specialFeaturedChannel in channels)
                {
                    Console.WriteLine("===");
                    Console.WriteLine(specialFeaturedChannel.SubmitChannel);
                    Console.WriteLine(specialFeaturedChannel.FeaturedChannel);
                }
            }            
        }

        public bool IsChannelFeatured(long id)
        {
            return channels.Any(i => i.SubmitChannel == id);
        }

        public bool MetReactionRequirement(IUserMessage message, int reactionAmount)
        {
            return channels.Any(i =>
                !i.featuredMessageID.Any(j => j == (long) message.Id) &&
                reactionAmount >= i.minimumReactionRequirement);
        }

        public long GetFeaturedChannel(long id)
        {
            return channels.Single(i => i.SubmitChannel == id).FeaturedChannel;
        }
    }

    internal class SpecialFeaturedChannel
    {
        public long SubmitChannel { get; set; }
        public long FeaturedChannel { get; set; }
        public List<long> featuredMessageID = new List<long>();
        public string emoteID { get; set; }
        public int minimumReactionRequirement { get; set; }


    }
}
