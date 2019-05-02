using Discord.Commands;
using DradonBotSharp.Services;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Discord;

namespace DradonBotSharp.Modules
{
    public class SQLModule : ModuleBase<SocketCommandContext>
    {
        public SQLDatabaseService database { get; set; }

        [Command("dbTest")]
        [RequireContext(ContextType.Guild)]
        public async Task Insert()
        {
            try
            {
                database.InsertNewGuild(Context.Guild);
                await ReplyAsync("Data base insert was a success!");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                await ReplyAsync("Data base insert was a failure...");
                throw;
            }
        }

        

        [Command("ChannelList")]
        [RequireContext(ContextType.Guild)]
        public async Task GetChannelList()
        {
            await ReplyAsync("", false, database.GetGuildChannelList(Context.Guild));
        }

        [Command("Tag")]
        [RequireContext(ContextType.Guild)]
        public async Task CreateCustomCommand(string commandContext, string commandName, [Remainder] string commandText = null)
        {
            switch (commandContext)
            {
                case "add":
                    if (database.CreateCustomCommand(Context.Guild, Context.User, commandName, commandText))
                        await ReplyAsync("The command was created");
                    else
                        await ReplyAsync("The command could not be created");
                    break;
                case "remove":
                    if (database.DeleteCustomCommand(Context.Guild, Context.User, commandName))
                        await ReplyAsync("The command was removed");
                    else
                        await ReplyAsync("The command was not found");
                    break;
                case "edit":
                    if (database.UpdateCustomCommand(Context.Guild, Context.User, commandName, commandText))
                        await ReplyAsync("The command was updated");
                    else
                        await ReplyAsync("The command was not found");
                    break;
            }
        }

        [Command("Tag")]
        [RequireContext(ContextType.Guild)]
        public async Task GetCustomCommand(string commandName)
        {
            await ReplyAsync(database.GetCommand(Context.Guild, commandName));
        }

        [Command("Feature")]
        [RequireContext(ContextType.Guild)]
        public async Task FeatureChannel(IGuildChannel channel1, IGuildChannel channel2)
        {

        }
    }
}
