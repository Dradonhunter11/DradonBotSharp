using System;
using DradonBotSharp.Core;

namespace DradonBotSharp
{
    class Program
    {
        static void Main(string[] args) => Bot.Instance.Main().GetAwaiter().GetResult();
    }
}
