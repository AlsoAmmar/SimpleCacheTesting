using System.Text.Json.Serialization;

namespace SimpleCacheTesting.Model;

public class User
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("name")]
    public string Name { get; set; }
    
    [JsonPropertyName("age")]
    public int Age { get; set; }
}