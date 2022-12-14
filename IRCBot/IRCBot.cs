using Meebey.SmartIrc4net;
using System;
using System.Text;
using System.Threading;
using System.Threading.Channels;

namespace IRCBotApp {
    class IRCBot {
        public static IrcClient irc = new();

        public void Run() {
            Thread.CurrentThread.Name = "IRCBot";

            // UTF-8 enconding
            irc.Encoding = Encoding.UTF8;
            // wait time between messages, we can set this lower on own irc servers
            irc.SendDelay = 500;
            // we use channel sync, means we can use irc.GetChannel() and so on
            irc.ActiveChannelSyncing = true;

            // here we connect the events of the API to our written methods
            // most have own event handler types, because they ship different data
            irc.OnError += new ErrorEventHandler(EventHandler.OnError);
            irc.OnChannelMessage += new IrcEventHandler(EventHandler.OnMessage);
            irc.OnQueryMessage += new IrcEventHandler(EventHandler.OnQueryMessage);
            irc.OnRawMessage += new IrcEventHandler(EventHandler.OnRawMessage);

            var config = new JsonFileReader().Read<Configuration>(@".\configuration.json");

            // here we try to connect to the server, if it fails we handle the exception and exit the program
            try {
                irc.Connect(config.ServerList,config.Port);
            }
            catch(ConnectionException e) {
                Console.WriteLine("couldn't connect! Reason: " + e.Message);
                Exit();
            }

            // now we are connected to the irc server, let's login, and join channels, and do bot stuff
            try {
                // here we logon and register our nickname and auth it with Q
                irc.Login(config.Nick,config.Realname,0,config.Username);
                // to auth with Q we need to send a direct message to it. 
                irc.SendMessage(SendType.Message,"Q@CServe.quakenet.org",$"auth {config.Username} {config.Password}");

                // join the channel
                irc.RfcJoin(config.ChannelList);

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