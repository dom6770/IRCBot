using System.IO;
using System.Text.Json;

public class Configuration {
    public string[] ServerList { get; set; }
    public int Port { get; set; }
    public string[] ChannelList { get; set; }
    public string Nick { get; set; }
    public string Realname { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }

}

public class JsonFileReader {
    public T Read<T>(string filePath) where T : class {
        // Check if the file exists
        if(!File.Exists(filePath))
            throw new FileNotFoundException($"The specified file could not be found: {filePath}");

        // Read the JSON file contents into a string
        var json = File.ReadAllText(filePath);

        // Deserialize the JSON string into an object of type T
        return JsonSerializer.Deserialize<T>(json);
    }
}