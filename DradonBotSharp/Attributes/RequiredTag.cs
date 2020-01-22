using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using DradonBotSharp.Enums;

namespace DradonBotSharp.Attributes
{
    
    /*[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    class RequiredTag : PreconditionAttribute
    {
        public GuildTags guildTag { get; }

        public RequiredTag(GuildTags tag)
        {
            guildTag = tag;
        }

        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
			if(context.Guild.Id)
	        return null;
        }
    }*/
}
