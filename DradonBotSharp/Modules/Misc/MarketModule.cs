using Discord.Commands;
using DradonBotSharp.Market;
using DradonBotSharp.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DradonBotSharp.Attributes;
using DradonBotSharp.Enums;

namespace DradonBotSharp.Modules
{
    //[RequiredTag(GuildTags.warframe)]
    public class MarketModule : ModuleBase<SocketCommandContext>
    {
        public JsonService json { get; set; }


        [Command("Market"), Priority(1000)]
        public async Task Market(string operation, int quantity, int price, string itemName)
        {
            switch (operation)
            {
                case "Add":
                    json.marketData.Add(new TradeDataCache(itemName.ToUpper(), quantity, price, Context.Guild.Id, Context.User.Id));
                    await ReplyAsync("Order Successfully added");
                    break;
                case "modify":
                    break;
                default:
                    await ReplyAsync("No valid operation found");
                    return;
            }
            json.SaveMarketConfig();
        }

        [Command("MarketAdmin")]
        public async Task AdminMarket()
        {

        }

        
        [Command("Market"), Priority(0)]
        public async Task Market(string operation, string itemName)
        {
            switch (operation)
            {
                case "list":
                    await ReplyAsync(ListMarket(itemName));
                    break;
                case "remove":
                    break;
                default:
                    await ReplyAsync("No valid operation found");
                    break;
            }
        }

        private void Modify(string itemName, int quantity)
        {
            TradeDataCache cache = json.marketData.Single(i =>
                i.guildID == Context.Guild.Id && i.ownerID == Context.User.Id && i.ItemName == itemName);
        }

        private string ListMarket(string itemName)
        {
            List<TradeDataCache> cache = json.marketData.Where(i =>
             i.guildID == Context.Guild.Id && i.ItemName == itemName.ToUpper()).ToList();

            string list = $"Market data for {itemName}\n" +
                          "```diff\n";

            foreach (var tradeDataCache in cache)
            {
                list +=
                    $"- {Context.Guild.GetUser(tradeDataCache.ownerID)} Quantity : {tradeDataCache.quantity} Price : {tradeDataCache.price}p\n";
            }

            list += "```";
            return list;
        }
    }
}
