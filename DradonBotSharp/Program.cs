using System;
using DradonBotSharp.Core;

namespace DradonBotSharp
{
    class Program
    {
        static void Main(string[] args) => Bot.instance.Main().GetAwaiter().GetResult();
    }
}
