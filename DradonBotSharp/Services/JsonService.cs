﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DradonBotSharp.Core;
using DradonBotSharp.Market;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DradonBotSharp.Services
{
    /// <summary>
    /// Every json operation are done in this class
    /// </summary>
    public class JsonService
    {
        #region readonly variable
        private readonly Bot _bot;
        private readonly DiscordSocketClient _socketClient;
        private readonly IServiceProvider _services;
        private readonly CommandService _commands;
        private readonly EasierRoleService _role;

        private readonly List<SpecialFeaturedChannel> channels;
        internal readonly List<TradeDataCache> marketData;
        internal readonly List<GuildConfig> configs;
        #endregion

        /// <summary>
        /// Constructor, import all the the required service from above
        /// </summary>
        /// <param name="service"></param>
        public JsonService(IServiceProvider service)
        {
            _bot = service.GetRequiredService<Bot>();
            _socketClient = service.GetRequiredService<DiscordSocketClient>();
            _commands = service.GetRequiredService<CommandService>();
            _role = service.GetRequiredService<EasierRoleService>();
            _services = service;

            channels = new List<SpecialFeaturedChannel>();
            marketData = new List<TradeDataCache>();
            configs = new List<GuildConfig>();
        }


        /// <summary>
        /// Load json file on startup, basically load config
        /// </summary>
        /// <returns></returns>
        public async Task Initialize()
        {
            string jsonPath = Path.Combine(Environment.CurrentDirectory, "json");

            if (!Directory.Exists(jsonPath))
            {
                return;
            }

            JsonTextReader reader;

            #region Featured Channel init
            if (File.Exists(Path.Combine(jsonPath, "feature.json")))
            {
                InitFeaturedChannel(jsonPath);
            }
            #endregion

            #region market init
            if (File.Exists(Path.Combine(jsonPath, "market.json")))
            {
                reader = new JsonTextReader(new StringReader(File.OpenText(Path.Combine(jsonPath, "market.json")).ReadToEnd()));
                JArray array = JArray.Load(reader);
                foreach (var value in array)
                {
                    JsonSerializer serializer = JsonSerializer.Create();
                    marketData.Add(serializer.Deserialize<TradeDataCache>(value.CreateReader()));

                }

                foreach (var specialFeaturedChannel in channels)
                {
                    Console.WriteLine("===");
                    Console.WriteLine(specialFeaturedChannel.SubmitChannel);
                    Console.WriteLine(specialFeaturedChannel.FeaturedChannel);
                }
            }         
            #endregion

        }

        private void InitFeaturedChannel(string jsonPath)
        {
            JsonTextReader reader;
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

        /// <summary>
        /// All operation for the featured channel feature, to move in FeaturedChannel.cs
        /// </summary>
        #region featured channel
        public void SaveFeaturedChannelConfig()
        {
            string json = JsonConvert.SerializeObject(channels, Formatting.Indented);
            string jsonPath = Path.Combine(Environment.CurrentDirectory, "json");

            if (!Directory.Exists(Path.Combine(Environment.CurrentDirectory, "json")))
            {
                Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, "json"));
            }

            File.WriteAllText(Path.Combine(jsonPath, "feature.json"), json, Encoding.Unicode);
        }



        public string AddChannel(ulong submit, ulong featured, string emote, int amount)
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
            SaveFeaturedChannelConfig();
            return $"This channel is now featured in <#{featured}> with the emmote {emote}";
        }

        

        public bool IsChannelFeatured(ulong id)
        {
            return channels.Any(i => i.SubmitChannel == id);
        }

        public bool MetReactionRequirement(ulong channelID, int reactionAmount)
        {
            return channels.Any(i => channelID == i.SubmitChannel && reactionAmount >= i.minimumReactionRequirement);
        }

        public bool IsAlreadyFeatured(IUserMessage message)
        {
            return channels.Any(i => i.featuredMessageID.Any(j => j == message.Id));
        }

        public ulong GetFeaturedChannel(ulong id)
        {
            return channels.Single(i => i.SubmitChannel == id).FeaturedChannel;
        }

        public void AddMessageID(ulong channelID, ulong messageID)
        {
            channels.Single(i => i.SubmitChannel == channelID).featuredMessageID.Add(messageID);
        }
        #endregion

        #region market
        public void SaveMarketConfig()
        {
            string json = JsonConvert.SerializeObject(marketData, Formatting.Indented);
            string jsonPath = Path.Combine(Environment.CurrentDirectory, "json");

            if (!Directory.Exists(Path.Combine(Environment.CurrentDirectory, "json")))
            {
                Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, "json"));
            }

            File.WriteAllText(Path.Combine(jsonPath, "market.json"), json, Encoding.Unicode);
        }
        #endregion

        /// <summary>
        /// a w.i.p module that make mass role joining easier, using emote (and won't be bound to a message)
        /// </summary>
        #region Easier Role Joining

        internal void SaveRole(List<JoinableRolePrefix> role)
        {
            string json = JsonConvert.SerializeObject(role, Formatting.Indented);
            string jsonPath = Path.Combine(Environment.CurrentDirectory, "json");

            if (!Directory.Exists(Path.Combine(Environment.CurrentDirectory, "json")))
            {
                Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, "json"));
            }

            File.WriteAllText(Path.Combine(jsonPath, "joinableRole.json"), json, Encoding.Unicode);
        }

        public void LoadRole()
        {

        }

        #endregion

        /// <summary>
        /// Various method to help with json operation
        /// </summary>
        #region Various helper utility
        public String GetInternalObject(string json, string fieldName)
        {
            JsonReader reader = new JsonTextReader(new StringReader(json));
            JObject jsonObject = JObject.Load(reader);
            jsonObject.ContainsKey(fieldName);
            return jsonObject[fieldName].ToString();
        }

        public String GetInternalArray(string json)
        {
            JsonReader reader = new JsonTextReader(new StringReader(json));
            JArray jsonArray = JArray.Load(reader);
            return jsonArray.ToString();
        }

        public JArray GetJArray(string json)
        {
            JsonReader reader = new JsonTextReader(new StringReader(json));
            JArray jsonArray = JArray.Load(reader);
            return jsonArray;
        }

        public Object GetInternalValue(string json, string fieldName)
        {
            JsonReader reader = new JsonTextReader(new StringReader(json));

            JObject jsonObject = JObject.Load(reader);
            if (jsonObject.ContainsKey(fieldName))
            {
                return jsonObject.SelectToken(fieldName).Value<Object>();
            }

            return "null";
        }
        #endregion
    }

    /// <summary>
    /// All for making role joining easier, was it worth to make a class?
    /// </summary>
    internal class JoinableRolePrefix
    {
        public ulong GuildToken { get; set; }
        public Dictionary<string, List<ulong>> roleList = new Dictionary<string, List<ulong>>();

        public bool GetListFromPrefix(string rolePrefix, out List<ulong> availableRoleList)
        {
            if (roleList.ContainsKey(rolePrefix))
            {
                availableRoleList = roleList[rolePrefix];
                return true;
            }
            availableRoleList = null; 
            return false;
        }
    }

    /// <summary>
    /// Simple data class for featured channel
    /// </summary>
    internal class SpecialFeaturedChannel
    {
        public ulong SubmitChannel { get; set; }
        public ulong FeaturedChannel { get; set; }
        public List<ulong> featuredMessageID = new List<ulong>();
        public string emoteID { get; set; }
        public int minimumReactionRequirement { get; set; }
    }
}
