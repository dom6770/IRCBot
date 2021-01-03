using System;
using System.IO;
using System.Net;
using System.Threading;

using Meebey.SmartIrc4net;
using Newtonsoft.Json;

class RandomDogFromFile {
    public static string Get() {
        string[] dogpics = File.ReadAllLines(@"E:\DOCUMENTS\dogpics.txt");
        Random rand = new Random();
        int index = rand.Next(dogpics.Length);
        return dogpics[index];
    }
    public static void Add(string url) {
        Console.WriteLine("URL: " + url + " added!");
        File.AppendAllText(@"E:\DOCUMENTS\dogpics.txt", Environment.NewLine + url);
    }
}
class RandomDog {
    public int fileSizeBytes { get; set; }
    public string url { get; set; }
    public static string Get() {
        RandomDog picture = new RandomDog {};
        using(WebClient client = new WebClient()) {
            var json = client.DownloadString("https://random.dog/woof.json");
            JsonConvert.PopulateObject(json, picture);
        }
        //var client = _httpClientFactory.CreateClient();
        //client.BaseAddress = new Uri("https://random.dog/");
        //RandomDog result = await client.GetFromJsonAsync("/woof.json");
        //return result.url;

        // * der tage nimmt man httpclient, nicht webclient
        // *httpclient hat dann GetFromJsonAsync: smile:

        return picture.url;
    }
}
class RandomCat {
    public string file { get; set; }
    public static string Get() {
        RandomCat picture = new RandomCat {};
        using(WebClient client = new WebClient()) {
            string json = client.DownloadString("http://aws.random.cat/meow");
            JsonConvert.PopulateObject(json, picture);
        }
        return picture.file;
    }
}
class IRCBot {
    public static IrcClient irc = new IrcClient();

    public static void OnQueryMessage(object sender, IrcEventArgs e) {
        if(e.Data.From == "dom!~dom@069-073.static.dsl.fonira.net") {
            System.Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "] Query: " + e.Data.Nick + " | " + e.Data.Message);
            switch(e.Data.MessageArray[0]) {
                case "host":
                    irc.SendMessage(SendType.Message, e.Data.Nick, "e.Data.From: " + e.Data.From);
                    irc.SendMessage(SendType.Message, e.Data.Nick, "e.Data.Host: " + e.Data.Host);
                    irc.SendMessage(SendType.Message, e.Data.Nick, "e.Data.Ident: " + e.Data.Ident);
                    irc.SendMessage(SendType.Message, e.Data.Nick, "e.Data.Nick: " + e.Data.Nick);
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
                case "add":
                    if(e.Data.MessageArray.Length > 1) RandomDogFromFile.Add(e.Data.MessageArray[1]);
                    break;
            }
        } else {
            irc.SendMessage(SendType.Message, e.Data.Nick, "Bark! You are not my owner.");
        }
    }
    public static void OnError(object sender, Meebey.SmartIrc4net.ErrorEventArgs e) {
        System.Console.WriteLine("Error: " + e.ErrorMessage);
        Exit();
    }
    public static void OnMessage(object sender, IrcEventArgs e) {
        switch(e.Data.MessageArray[0].ToLower()) {

            // RANDOM DOG SELFHOSTED
            case "!dog":
            case "!dogpic":
            case "!dogpics":
                System.Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "] " + e.Data.Channel + " - " + e.Data.Nick + " | " + e.Data.Message);
                irc.SendMessage(SendType.Message, e.Data.Channel, RandomDogFromFile.Get() + " 🐾");
                break;
            case "!add":
                if(e.Data.MessageArray.Length > 1) RandomDogFromFile.Add(e.Data.MessageArray[1]);
                break;

            // RANDOM DOG
            case "!drecksvieh":
            case "!randomdog":
            case "!random.dog":
                System.Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "] " + e.Data.Channel + " - " + e.Data.Nick + " | " + e.Data.Message);
                irc.SendMessage(SendType.Message, e.Data.Channel, RandomDog.Get() + " 🐾");
                break;

            // RANDOM CAT
            case "!randomcat":
                Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "] " + e.Data.Channel + " - " + e.Data.Nick + " | " + e.Data.Message);
                irc.SendMessage(SendType.Message, e.Data.Channel, RandomCat.Get() + " 🐾");
                break;

            // DICK PIC
            case "!dickpic":
                System.Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "] " + e.Data.Channel + " - " + e.Data.Nick + " | " + e.Data.Message);
                irc.SendMessage(SendType.Message, e.Data.Channel, "https://i.neus.xyz/bO2FjL.jpg");
                break;

            // REACTION COMMANDS
            case "!awoo":
                Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "] " + e.Data.Channel + " - " + e.Data.Nick + " | " + e.Data.Message);
                irc.SendMessage(SendType.Message, e.Data.Channel, "Awoo!");
                break;
            case "!meow":
                Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "] " + e.Data.Channel + " - " + e.Data.Nick + " | " + e.Data.Message);
                irc.SendMessage(SendType.Message, e.Data.Channel, "meow!");
                break;
            case "!oida":
                if(e.Data.MessageArray.Length > 1) {
                    Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "] " + e.Data.Channel + " - " + e.Data.Nick + " | " + e.Data.Message);
                    irc.SendMessage(SendType.Message, e.Data.Channel, "Oida " + e.Data.MessageArray[1] + "!");
                    break;
                } else {
                    Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "] " + e.Data.Channel + " - " + e.Data.Nick + " | " + e.Data.Message);
                    irc.SendMessage(SendType.Message, e.Data.Channel, "Oida!");
                    break;
                }
        }

    }
    static void Main(string[] args) {
        Thread.CurrentThread.Name = "IRCBot";

        // UTF-8 test
        irc.Encoding = System.Text.Encoding.UTF8;

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
            irc.Connect(serverlist, port);
        } catch(ConnectionException e) {
            System.Console.WriteLine("couldn't connect! Reason: " + e.Message);
            Exit();
        }

        // now we are connected to the irc server, let's login, and join channels, and do bot stuff
        try {
            // here we logon and register our nickname and auth it with Q
            //irc.Login("Otis", "A stupid C# Bot by dom, UwU");
            irc.Login("Ollie", "A stupid C# Bot by dom, UwU", 0, "BaseBot");
            irc.SendMessage(SendType.Message, "Q@CServe.quakenet.org", "auth BaseBot " + args[0]);
            // join the channel
            irc.RfcJoin("#rainbow");
            irc.RfcJoin("#ComputerBase");

            // here we tell the IRC API to go into a receive mode, all events
            // will be triggered by _this_ thread (main thread in this case)
            // Listen() blocks by default, you can also use ListenOnce() if you
            // need that does one IRC operation and then returns, so you need then 
            // an own loop 
            irc.Listen();

            // when Listen() returns our IRC session is over, to be sure we call
            // disconnect manually
            irc.Disconnect();
        } catch(ConnectionException) {
            // this exception is handled because Disconnect() can throw a not
            // connected exception
            Exit();
        } catch(Exception e) {
            // this should not happen by just in case we handle it nicely
            System.Console.WriteLine("Error occurred! Message: " + e.Message);
            System.Console.WriteLine("Exception: " + e.StackTrace);
            Exit();
        }
    }
    public static void Exit() {
        System.Console.WriteLine("Exiting...");
        System.Environment.Exit(0);
    }
}
