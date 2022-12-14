using Meebey.SmartIrc4net;
using System;

namespace IRCBotApp { 
    class Program {
        private static void Main(string[] args) {
            IRCBot bot = new();
            bot.Run();    
        }
    }
}