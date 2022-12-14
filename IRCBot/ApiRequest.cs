using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace IRCBot {
    class ApiRequest {
        static async Task<string> GetApiResponse(string url) {
            // Create a new HttpClient to make the API request
            HttpClient client = new();
            // Make the request and return the response as a string
            return await client.GetStringAsync(url);
        }

        public class RandomDog {
            public int fileSizeBytes { get; set; }
            public string url { get; set; }
            public static async Task<string> Get() {
                // Store the response as a string
                string response = await GetApiResponse("https://random.dog/woof.json");
                // Populate the RandomDog object with the response from the API
                RandomDog picture = JsonSerializer.Deserialize<RandomDog>(response);
                // Return the URL
                return picture.url;
            }
        }
        public class RandomCat {
            public string file { get; set; }
            public static async Task<string> Get() {
                // Store the response as a string
                string response = await GetApiResponse("http://aws.random.cat/meow");
                // Populate the RandomDog object with the response from the API
                RandomCat picture = JsonSerializer.Deserialize<RandomCat>(response);
                // Return the URL
                return picture.file;
            }
        }
    }
}
