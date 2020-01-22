using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DradonBotSharp.Core
{
    internal static class Token
    {
        private static string TOKEN = Path.Combine(Environment.CurrentDirectory, "token.bot");

        public static string BotToken() => File.Exists(TOKEN) ? File.ReadAllText(TOKEN) : null;
    }
}
