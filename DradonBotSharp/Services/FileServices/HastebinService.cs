using System;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DradonBotSharp.Core;
using DradonBotSharp.Core.Config;
using Microsoft.Extensions.DependencyInjection;

namespace DradonBotSharp.Services.FileServices
{
	class HastebinService
	{
		private static readonly Regex _HasteKeyRegex = new Regex(@"{""key"":""(?<key>[a-z].*)""}", RegexOptions.Compiled);

		private readonly DiscordSocketClient _client;

        public HastebinService(IServiceProvider services)
		{
            _client = services.GetRequiredService<DiscordSocketClient>();
            _client.MessageReceived += HandleCommand;
		}

		~HastebinService()
		{
			_client.MessageReceived -= HandleCommand;
		}

		// TODO: Autohastebin .cs or .txt file attachments.
		private async Task HandleCommand(SocketMessage socketMessage)
		{
            // Valid message, no bot, no webhook, and valid channel
			if (!(socketMessage is SocketUserMessage message)
				|| message.Author.IsBot
				|| message.Author.IsWebhook
				|| !(message.Channel is SocketTextChannel channel))
				return;

			var context = new SocketCommandContext(_client, message);

			if(!GuildConfigManager.instance.HasPermission(context.Guild.Id, "HastebinPermission")) { return; }

			string contents = message.Content;
			bool shouldHastebin = false;
			bool autoDeleteUserMessage = false;
			string extra = "";

			var attachents = message.Attachments;
			if (attachents.Count == 1 && attachents.ElementAt(0) is Attachment attachment)
			{
				if (attachment.Filename.EndsWith(".log") || attachment.Filename.EndsWith(".cs") && attachment.Size < 100000)
				{
					using (var client = new HttpClient())
						contents = await client.GetStringAsync(attachment.Url);

					shouldHastebin = true;
					extra = $" `({attachment.Filename})`";
				}
			}

			if (string.IsNullOrWhiteSpace(contents))
				return;

			int count = 0;
			if (!shouldHastebin)
			{
				foreach (char c in contents)
				{
					if (c == '{') count++;
					if (c == '}') count++;
					if (c == '=') count++;
					if (c == ';') count++;
				}
				if (count > 1 && message.Content.Split('\n').Length > 16)
				{
					shouldHastebin = true;
					autoDeleteUserMessage = true;
				}
			}

			if (shouldHastebin)
			{
				string hastebinContent = contents.Trim('`');

				//var msg = await context.Channel.SendMessageAsync("Auto Hastebin in progress");
				using (var client = new HttpClient())
				{
					HttpContent content = new StringContent(hastebinContent);

					var response = await client.PostAsync("https://paste.mod.gg/documents", content);
					string resultContent = await response.Content.ReadAsStringAsync();

					var match = _HasteKeyRegex.Match(resultContent);

					if (!match.Success)
					{
                        return;
					}



					string hasteUrl = $"https://paste.mod.gg/{match.Groups["key"]}.cs";
                    EmbedBuilder builder = new EmbedBuilder();
                    builder.WithAuthor(context.User.Username);
                    builder.WithColor(Color.DarkBlue);
                    builder.WithThumbnailUrl(context.User.GetAvatarUrl());
                    builder.AddField(BotUtils.CreateEmbdedField("Hastebin URL link", hasteUrl));
					await context.Channel.SendMessageAsync($"Automatic Hastebin for {message.Author.Username}{extra}\nOriginal Module from Jopojelly", false, builder.Build());
                    if (GuildConfigManager.instance.HasPermission(context.Guild.Id, "LoggingPermission"))
                    {
                        await GuildConfigManager.instance.Log(context, $"{socketMessage.Attachments.First().Filename} has been successfully uploaded to hastebin (Requested by {socketMessage.Author.Username})", builder.Build());
                    }
                    if (autoDeleteUserMessage)
						await message.DeleteAsync();
				}
			}
		}
	}
}
