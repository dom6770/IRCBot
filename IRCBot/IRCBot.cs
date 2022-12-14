using Meebey.SmartIrc4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IRCBotApp {
    class IRCBot {
        public static IrcClient irc = new();

        public static void OnQueryMessage(object sender,IrcEventArgs e) {
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
        public static void OnError(object sender,Meebey.SmartIrc4net.ErrorEventArgs e) {
            Console.WriteLine("Error: " + e.ErrorMessage);
            irc.SendMessage(SendType.Message,"dom",e.ErrorMessage);
            Exit();
        }

        public static async void OnMessage(object sender,IrcEventArgs e) {
            switch(e.Data.MessageArray[0].ToLower()) {

                // LIST
                case "!list":
                Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "] " + e.Data.Channel + " - " + e.Data.Nick + " | " + e.Data.Message);
                irc.SendMessage(SendType.Message,e.Data.Channel,"!dog !dogpic !dogpics");
                irc.SendMessage(SendType.Message,e.Data.Channel,"!randomdog !raaaaandom.dog");
                irc.SendMessage(SendType.Message,e.Data.Channel,"!randomcat !random.cat");
                irc.SendMessage(SendType.Message,e.Data.Channel,"!awoo !woof !meow");
                irc.SendMessage(SendType.Message,e.Data.Channel,"!oida");
                break;

                // RANDOM DOG
                case "!dog":
                case "!dogpic":
                case "!dogpics":
                case "!drecksvieh":
                case "!randomdog":
                case "!random.dog":
                Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "] " + e.Data.Channel + " - " + e.Data.Nick + " | " + e.Data.Message);
                irc.SendMessage(SendType.Message,e.Data.Channel,await RandomDog.Get() + " 🐾");
                break;

                // RANDOM CAT
                case "!randomcat":
                case "!random.cat":
                Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "] " + e.Data.Channel + " - " + e.Data.Nick + " | " + e.Data.Message);
                irc.SendMessage(SendType.Message,e.Data.Channel,RandomCat.Get() + " 🐾");
                break;

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
        public void Run(string[] args) {
            Thread.CurrentThread.Name = "IRCBot";

            // UTF-8 test
            irc.Encoding = Encoding.UTF8;
            // wait time between messages, we can set this lower on own irc servers
            irc.SendDelay = 500;
            // we use channel sync, means we can use irc.GetChannel() and so on
            irc.ActiveChannelSyncing = true;


            // here we connect the events of the API to our written methods
            // most have own event handler types, because they ship different data
            irc.OnError += new Meebey.SmartIrc4net.ErrorEventHandler(OnError);
            irc.OnChannelMessage += new IrcEventHandler(OnMessage);
            irc.OnQueryMessage += new IrcEventHandler(OnQueryMessage);

            string[] serverlist = new string[] { "irc.quakenet.org" }; // the server we want to connect to, could be also a simple string
            int port = 6667;

            // here we try to connect to the server, if it fails we handle the exception and exit the program
            try {
                irc.Connect(serverlist,port);
            }
            catch(ConnectionException e) {
                Console.WriteLine("couldn't connect! Reason: " + e.Message);
                Exit();
            }

            // now we are connected to the irc server, let's login, and join channels, and do bot stuff
            try {
                // here we logon and register our nickname and auth it with Q
                //irc.Login("Otis", "A stupid C# Bot by dom, UwU");
                irc.Login("Otis2","A stupid C# Bot by dom" + "",0,"BaseBot");
                irc.SendMessage(SendType.Message,"Q@CServe.quakenet.org","auth BaseBot " + args[0]);
                // join the channel
                irc.RfcJoin("#rainbow");
                //irc.RfcJoin("#ComputerBase");

                // here we tell the IRC API to go into a receive mode, all events
                // will be triggered by _this_ thread (main thread in this case)
                // Listen() blocks by default, you can also use ListenOnce() if you
                // need that does one IRC operation and then returns, so you need then 
                // an own loop 
                irc.Listen();

                // when Listen() returns our IRC session is over, to be sure we call
                // disconnect manually
                irc.Disconnect();
            }
            catch(ConnectionException) {
                // this exception is handled because Disconnect() can throw a not
                // connected exception
                Exit();
            }
            catch(Exception e) {
                // this should not happen by just in case we handle it nicely
                Console.WriteLine("Error occurred! Message: " + e.Message);
                Console.WriteLine("Exception: " + e.StackTrace);
                Exit();
            }
        }
        public static void Exit() {
            Console.WriteLine("Exiting...");
            Environment.Exit(0);
        }
    }
}