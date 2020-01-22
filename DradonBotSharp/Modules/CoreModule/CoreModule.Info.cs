﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using DradonBotSharp.Core;

namespace DradonBotSharp.Modules.CoreModule
{
	public partial class CoreModule : ModuleBase<SocketCommandContext>
	{
		[Command("GuildInfo")]
		[RequireContext(ContextType.Guild)]
		public async Task GuildInfo()
		{
			EmbedBuilder builder = new EmbedBuilder();
			builder.WithDescription($"{Context.Guild.Name} Info - Generated by DradonBotSharp");
			builder.AddField(BotUtils.CreateEmbdedField("Guild ID", Context.Guild.Id));
			builder.AddField(BotUtils.CreateEmbdedField("Population", Context.Guild.MemberCount));
			builder.AddField(BotUtils.CreateEmbdedField("Channel count", Context.Guild.Channels.Count));
			builder.AddField(BotUtils.CreateEmbdedField("Role amount", Context.Guild.Roles.Count));
			
			builder.WithColor(Color.Gold);
			builder.WithThumbnailUrl(Context.Guild.IconUrl);

			await ReplyAsync("", false, builder.Build());
		}

		[Command("ProfileInfo")]
		public async Task ProfileInfo()
		{
			IUser user = Context.User;
			EmbedBuilder embed = new EmbedBuilder();
			embed.WithImageUrl(user.GetAvatarUrl());
			embed.AddField(BotUtils.CreateEmbdedField("Account creation date", user.CreatedAt.ToString(), false));
			await ReplyAsync(null, false, embed.Build());
		}
	}
}
