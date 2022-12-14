using Meebey.SmartIrc4net;
using System;
using System.Linq;

namespace IRCBotApp {
    class EventHandler:IRCBot {
        public static void OnError(object sender,Meebey.SmartIrc4net.ErrorEventArgs e) {
            Console.WriteLine("Error: " + e.ErrorMessage);
            irc.SendMessage(SendType.Message,"dom",e.ErrorMessage);
            Exit();
        }

        public static async void OnMessage(object sender,Meebey.SmartIrc4net.IrcEventArgs e) {

            string input = e.Data.MessageArray[0].ToLower();

            string[] dogCommand = { "!dog", "!dogpic", "!dogpics", "!drecksvieh", "!randomdog", "!random.dog" };
            string[] catCommand = { "!cat","!catpic","!catpics","!drecksvieh","!randomcat","!random.cat" };

            Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "] " + e.Data.Channel + " - " + e.Data.Nick + " | " + e.Data.Message);

            if(dogCommand.Contains(input))
                irc.SendMessage(SendType.Message,e.Data.Channel,await RandomDog.Get() + " 🐾");

            if(catCommand.Contains(input))               
                irc.SendMessage(SendType.Message,e.Data.Channel,RandomCat.Get() + " 🐾");



            switch(input) {
                // REACTION COMMANDS
                case "!awoo":
                Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "] " + e.Data.Channel + " - " + e.Data.Nick + " | " + e.Data.Message);
                irc.SendMessage(SendType.Message,e.Data.Channel,"Awoo!");
                break;
                case "!woof":
                Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "] " + e.Data.Channel + " - " + e.Data.Nick + " | " + e.Data.Message);
                irc.SendMessage(SendType.Message,e.Data.Channel,"Woof!");
                break;
                case "!meow":
                Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "] " + e.Data.Channel + " - " + e.Data.Nick + " | " + e.Data.Message);
                irc.SendMessage(SendType.Message,e.Data.Channel,"meow!");
                break;
                case "!oida":
                if(e.Data.MessageArray.Length > 1) {
                    Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "] " + e.Data.Channel + " - " + e.Data.Nick + " | " + e.Data.Message);
                    irc.SendMessage(SendType.Message,e.Data.Channel,"Oida " + e.Data.MessageArray[1] + "!");
                    break;
                } else {
                    Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "] " + e.Data.Channel + " - " + e.Data.Nick + " | " + e.Data.Message);
                    irc.SendMessage(SendType.Message,e.Data.Channel,"Oida!");
                    break;
                }
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