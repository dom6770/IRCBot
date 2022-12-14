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

    class RandomDog {
        public int fileSizeBytes { get; set; }
        public string url { get; set; }
        public static async Task<string> Get() {
            string api = "https://random.dog/woof.json";

            Console.WriteLine("Getting api response..");
            string response = await GetApiResponse(api);
            Console.WriteLine("API repsone is: " + response);


            RandomDog picture = System.Text.Json.JsonSerializer.Deserialize<RandomDog>(response);
            Console.WriteLine("URL is: " + picture.url);

            return picture.url;
        }

        static async Task<string> GetApiResponse(string url) {
            // Create a new HttpClient to make the API request
            HttpClient client = new();
            // Make the request and get the response as a string
            return await client.GetStringAsync(url);
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
}