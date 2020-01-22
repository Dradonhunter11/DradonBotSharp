using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HypixelSharp.Exceptions;
using HypixelSharp.reply;
using HypixelSharp.Reply;
using HypixelSharp.Reply.Skyblock;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HypixelSharp
{
    public class HypixelAPI
    {
        private readonly Guid API_KEY;

        private readonly string BASE_URL = "https://api.hypixel.net/";

        private readonly HttpClient client;

        public HypixelAPI(Guid guid)
        {
            API_KEY = guid;
            client = new HttpClient();
        }

        public async Task<PlayerReply> GetPlayerByName(string player)
        {
            return await Get<PlayerReply>(typeof(PlayerReply), "player", "name", player);
        }

        public async Task<SkyBlockProfileReply> GetSkyBlockProfile(string profile)
        {
            return await Get<SkyBlockProfileReply>(typeof(SkyBlockProfileReply), "skyblock/profile", "profile", profile);
        }

        public Task<R> Get<R>(Type type, string request, params object[] args) where R : AbstractReply
        {
            
            try
            {
                if (args.Length % 2 != 0)
                {
                    throw new ArgumentException("Need both key and value for parameters");
                }

                StringBuilder url = new StringBuilder(BASE_URL);
                url.Append(request);
                url.Append("?key=").Append(API_KEY);

                for (int i = 0; i < args.Length - 1; i += 2)
                {
                    url.Append("&").Append(args[i]).Append("=").Append(args[i + 1]);
                }

                return Task.Run<R>(() =>
                {
                    string content = client.GetAsync(url.ToString()).Result.Content.ReadAsStringAsync().Result;
                    if (type == typeof(ResourceReply))
                    {
                        return new ResourceReply(JObject.Parse(content)) as R;
                    }
                    else
                    {
                        return JsonConvert.DeserializeObject<R>(content);
                    }
                    return null;
                });
            }
            catch (Exception e)
            {
                return Task.FromException<R>(e);
            }
            return null;
        }

        private void CheckReply<T>(T reply) where T : AbstractReply
        {
            if (reply != null)
            {
                if (reply.isThrottle())
                {
                    throw new APIThrottleException();
                }
                else if (!reply.isSuccess())
                {
                    throw new HypixelAPIException(reply.getCause());
                }
            }
        }
    }
}
