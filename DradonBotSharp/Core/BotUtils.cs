using System;
using System.Collections.Generic;
using System.Text;
using Discord;

namespace DradonBotSharp.Core
{
    static class BotUtils
    {
        public static EmbedFieldBuilder CreateEmbdedField(String name, Object value, bool IsInLine = true)
        {
            EmbedFieldBuilder builder = new EmbedFieldBuilder();
            builder.Name = name;
            builder.Value = value;
            builder.IsInline = IsInLine;
            return builder;
        }
    }
}
