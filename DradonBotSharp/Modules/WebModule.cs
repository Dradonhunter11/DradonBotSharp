using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using DradonBotSharp.Core;
using DradonBotSharp.Services;

namespace DradonBotSharp.Modules
{
    public class WebModule : ModuleBase<SocketCommandContext>
    {
        public JsonService json { get; set; }

        [Command("pricecheck")]
        public async Task PriceCheck([Remainder] string name = null)
        {
            try
            {
                string uri = $"https://api.warframe.market/v1/items/{name.Replace(" ", "_").ToLower()}/statistics";
                string result = Get(uri);
                string parsedResult = json.GetJArray(json.GetInternalObject(json.GetInternalObject(json.GetInternalObject(result, "payload"), "statistics_closed"), "48hours"))[0].ToString();
                EmbedBuilder builder = new EmbedBuilder();
                builder.WithAuthor("Warframe.market API");
                builder.WithTitle(name.ToUpper());
                builder.AddField(BotUtils.CreateEmbdedField("Date",
                    json.GetInternalValue(parsedResult, "datetime")));
                builder.AddField(BotUtils.CreateEmbdedField("Quantity Sold",
                    json.GetInternalValue(parsedResult, "volume"), false));
                builder.AddField(BotUtils.CreateEmbdedField("Minimum Price",
                    json.GetInternalValue(parsedResult, "min_price")));
                builder.AddField(BotUtils.CreateEmbdedField("Maximum Price",
                    json.GetInternalValue(parsedResult, "max_price")));
                builder.AddField(BotUtils.CreateEmbdedField("Average Price",
                    json.GetInternalValue(parsedResult, "avg_price")));
                builder.AddField(BotUtils.CreateEmbdedField("Median Price",
                    json.GetInternalValue(parsedResult, "median"), false));
                await ReplyAsync(Context.User.Mention, false, builder.Build());
            }
            catch (Exception e)
            {

                await ReplyAsync(
                    "An error happened while doing the request to the Warframe.Market API. Generally caused by the fact you made an error in the name of the item you requested or that no one have sold this item in the last 48 hours.");
                Console.WriteLine(e);
            }
            //json.GetInternalArray(
            //    json.GetInternalObject(json.GetInternalObject(result, "payload"), "statistics_closed"), "48hours");
        }

        [Command("arbitration")]
        public async Task Arbitration()
        {
            try
            {
                string uri = $"http://arbitrationtracker.tk/api/";
                string result = Get(uri);
                string parsedResult = result;
                EmbedBuilder builder = new EmbedBuilder();
                builder.WithAuthor("http://arbitrationtracker.tk/");
                builder.WithTitle("Current Arbitration");
                builder.WithThumbnailUrl(
                    "http://content.warframe.com/MobileExport/Lotus/Materials/Emblems/EliteAlertIcon.png");
                builder.AddField(BotUtils.CreateEmbdedField("Node",
                    json.GetInternalValue(parsedResult, "planet") + " - " + json.GetInternalValue(parsedResult, "node")));
                builder.AddField(BotUtils.CreateEmbdedField("Faction",
                    json.GetInternalValue(parsedResult, "enemy")));
                builder.AddField(BotUtils.CreateEmbdedField("Mission Type",
                    json.GetInternalValue(parsedResult, "type")));

                double time = (double) json.GetInternalValue(parsedResult, "startedat");
                DateTime date = ConvertFromUnixTimestamp(time);
                string dateString = $"{date.Year}/{date.Month}/{date.Day}/{date.Hour}:{date.Minute}:{date.Second}";
                builder.AddField(BotUtils.CreateEmbdedField("Start Time",
                    dateString));

                time = (double)json.GetInternalValue(parsedResult, "endsat");
                date = ConvertFromUnixTimestamp(time);
                dateString = $"{date.Year}/{date.Month}/{date.Day}/{date.Hour}:{date.Minute}:{date.Second}";
                builder.AddField(BotUtils.CreateEmbdedField("EndTime",
                    dateString));

                await ReplyAsync(Context.User.Mention, false, builder.Build());
            }
            catch (Exception e)
            {

                await ReplyAsync(
                    "Sorry, this API is temporarily down.");
            }
            //http://content.warframe.com/MobileExport/Lotus/Materials/Emblems/EliteAlertIcon.png
        }

        public static DateTime ConvertFromUnixTimestamp(double timestamp)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return origin.AddSeconds(timestamp / 1000); // convert from milliseconds to seconds
        }

        //TODO : Move in WebService
        public string Get(string uri)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
