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
            irc.OnError += new ErrorEventHandler(EventHandler.OnError);
            irc.OnChannelMessage += new IrcEventHandler(EventHandler.OnMessage);
            irc.OnQueryMessage += new IrcEventHandler(EventHandler.OnQueryMessage);

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