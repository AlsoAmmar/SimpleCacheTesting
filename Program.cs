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
        
        Stopwatch sw = new  Stopwatch();
        
        while (true)
        {
            Console.WriteLine("Press any button to get users frfr");
            
            if (Console.ReadKey().Key == ConsoleKey.Escape)
            {
                break;
            }
            
            sw.Reset();
            sw.Start();
            
            IEnumerable<User> users = GetUsers(cache).Result;

            foreach (var user in users)
            {
                Console.WriteLine($"{user.Name} {user.Age}");
            }
            
            sw.Stop();

            Console.WriteLine($"\n{sw.ElapsedMilliseconds} ms");
        }
    }
    
    public static async Task<IEnumerable<User>> GetUsers(IMemoryCache cache)
    {
        string cacheKey = "users";

        if (cache.TryGetValue(cacheKey, out IEnumerable<User>? cachedUsers) && cachedUsers is not null)
        {
            Console.WriteLine("Returning cached users");
            return cachedUsers;
        }

        string json = await File.ReadAllTextAsync("../../../data/data.json");
        IEnumerable<User> users = JsonSerializer.Deserialize<IEnumerable<User>>(json)!;
        
        cache.Set(cacheKey, users);

        Console.WriteLine("Returning uncached users");
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