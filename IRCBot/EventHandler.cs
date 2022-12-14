using Meebey.SmartIrc4net;
using System;
using System.Linq;

namespace IRCBotApp {
    class EventHandler:IRCBot {
        public static void OnError(object sender,Meebey.SmartIrc4net.ErrorEventArgs e) {
            Console.WriteLine($"Error: {e.ErrorMessage}");
            irc.SendMessage(SendType.Message,"dom",e.ErrorMessage);
            Exit();
        }

        public static async void OnMessage(object sender,Meebey.SmartIrc4net.IrcEventArgs e) {

            string input = e.Data.MessageArray[0].ToLower();

            string[] dogCommands = { "!dog","!dogpic","!dogpics","!drecksvieh","!randomdog","!random.dog" };
            string[] catCommands = { "!cat","!catpic","!catpics","!drecksvieh","!randomcat","!random.cat" };
            string[] reactionCommands = { "!awoo","!woof","!meow" };
            string[] allCommands = dogCommands.Concat(catCommands).Concat(reactionCommands).Append("!oida").ToArray();

            if(allCommands.Contains(input))
                Console.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss")}] {e.Data.Channel} - {e.Data.Nick} | {e.Data.Message}");

            if(dogCommands.Contains(input))
                irc.SendMessage(SendType.Message,e.Data.Channel,await RandomDog.Get() + " 🐾");

            if(catCommands.Contains(input))               
                irc.SendMessage(SendType.Message,e.Data.Channel,RandomCat.Get() + " 🐾");

            if(reactionCommands.Contains(input))
                irc.SendMessage(SendType.Message,e.Data.Channel,input.Substring(1));

            if(input.Contains("!oida")) {
                if(e.Data.MessageArray.Length > 1)
                    irc.SendMessage(SendType.Message,e.Data.Channel,"oida " + e.Data.MessageArray[1] + "!");
                else
                    irc.SendMessage(SendType.Message,e.Data.Channel,"heast, oida mi ned!");
            }
        }

        public static void OnQueryMessage(object sender,Meebey.SmartIrc4net.IrcEventArgs e) {
            if(e.Data.From == "dom!~dom@069-073.static.dsl.fonira.net") {
                Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "] Query: " + e.Data.Nick + " | " + e.Data.Message);
                switch(e.Data.MessageArray[0]) {
                    case "host":
                    irc.SendMessage(SendType.Message,e.Data.Nick,"e.Data.From: " + e.Data.From);
                    irc.SendMessage(SendType.Message,e.Data.Nick,"e.Data.Host: " + e.Data.Host);
                    irc.SendMessage(SendType.Message,e.Data.Nick,"e.Data.Ident: " + e.Data.Ident);
                    irc.SendMessage(SendType.Message,e.Data.Nick,"e.Data.Nick: " + e.Data.Nick);
                    break;
                    case "join":
                    irc.RfcJoin(e.Data.MessageArray[1]);
                    break;
                    case "leave":
                    case "part":
                    irc.RfcPart(e.Data.MessageArray[1]);
                    break;
                    case "die":
                    Exit();
                    break;
                }
            } else {
                irc.SendMessage(SendType.Message,e.Data.Nick,"Bark! You are not my owner.");
            }
        }
        
    }
}