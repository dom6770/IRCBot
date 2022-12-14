using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace IRCBotApp { 
    class Program {
        private static void Main(string[] args) {
            IRCBot bot = new();
            bot.Run(args);    
        }
    }
}