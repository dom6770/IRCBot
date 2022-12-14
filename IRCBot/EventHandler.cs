using IRCBot;
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

        public static void OnRawMessage(object sender,Meebey.SmartIrc4net.IrcEventArgs e) {
            string input = e.Data.RawMessage;

            if(input.Contains("stockholm.se.quakenet.org") || input.Contains("Q!TheQBot@CServe.quakenet.org"))
                Console.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss")}] {e.Data.RawMessage}");
        }

        public static async void OnMessage(object sender,Meebey.SmartIrc4net.IrcEventArgs e) {

            string input = e.Data.MessageArray[0].ToLower();

            string[] dogCommands = { "!dog","!dogpic","!dogpics","!drecksvieh","!randomdog","!random.dog" };
            string[] catCommands = { "!cat","!catpic","!catpics","!drecksvieh","!randomcat","!random.cat" };
            string[] reactionCommands = { "!awoo","!woof","!meow" };
            string[] allCommands = dogCommands.Concat(catCommands).Concat(reactionCommands).Append("!oida").ToArray();

            // If any command is send, display it in the console with datetime, channel, nick and message.
            if(allCommands.Contains(input))
                Console.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss")}] {e.Data.Channel} - {e.Data.Nick} | {e.Data.Message}");

            // If the dog command is triggered, return a dog picture from the api
            if(dogCommands.Contains(input))
                irc.SendMessage(SendType.Message,e.Data.Channel,await ApiRequest.RandomDog.Get() + " 🐾"); ;

            // If the cat command is triggered, return a cat picture from the api
            if(catCommands.Contains(input))
                irc.SendMessage(SendType.Message,e.Data.Channel,await ApiRequest.RandomCat.Get() + " 🐾");

            // If a reaction is trigerred, turn it as receivied (!woof -> woof)
            if(reactionCommands.Contains(input))
                irc.SendMessage(SendType.Message,e.Data.Channel,input.Substring(1));

            // oida command. 
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