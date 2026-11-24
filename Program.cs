using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Caching.Memory;

namespace SimpleCacheTesting;

class Program
{
    static void Main(string[] args)
    {
        IMemoryCache cache = new MemoryCache(new MemoryCacheOptions());
        
        while (true)
        {
            Console.WriteLine("Press any button to get users frfr");
            
            Stopwatch sw = new  Stopwatch();
            sw.Start();
            
            if (Console.ReadKey().Key == ConsoleKey.Escape)
            {
                break;
            }
            
            IEnumerable<User> users = GetUsers(cache).Result;

            foreach (var user in users)
            {
                Console.WriteLine($"{user.Name} {user.Age}");
            }
            
            sw.Stop();

            Console.WriteLine(sw.ElapsedMilliseconds);
        }
    }
    
    public static async Task<IEnumerable<User>> GetUsers(IMemoryCache cache)
    {
        string cacheKey = "users";

        if (cache.TryGetValue(cacheKey, out IEnumerable<User>? cachedUsers) && cachedUsers is not null)
        {
            return cachedUsers;
        }

        string json = await File.ReadAllTextAsync("../../../data/data.json");
        IEnumerable<User> users = JsonSerializer.Deserialize<IEnumerable<User>>(json)!;
        
        cache.Set(cacheKey, users);

        return users;
    }

    public class User
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        
        [JsonPropertyName("name")]
        public string Name { get; set; }
        
        [JsonPropertyName("age")]
        public int Age { get; set; }
    }
}