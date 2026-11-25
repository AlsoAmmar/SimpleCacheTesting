using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using SimpleCacheTesting.Model;
using StackExchange.Redis;

namespace SimpleCacheTesting;

class Program
{
    public static string message = "";
    
    static async Task Main(string[] args)
    {
        IMemoryCache cache = new MemoryCache(new MemoryCacheOptions());
        using var redis = await ConnectionMultiplexer.ConnectAsync("localhost:6379");
        IDatabase db = redis.GetDatabase();
        
        Stopwatch sw = new  Stopwatch();
        
        while (true)
        {
            Console.WriteLine("Press any button to get users");
            
            if (Console.ReadKey().Key == ConsoleKey.Escape)
            {
                break;
            }
            
            sw.Reset();
            sw.Start();
            
            // IEnumerable<User> users = await GetUsersMemoryCache(cache);
            IEnumerable<User> users = await GetUsersDistributedCache(db);

            foreach (var user in users)
            {
                Console.WriteLine($"{user.Name} {user.Age}");
            }
            
            sw.Stop();

            Console.WriteLine($"\n{message}");
            Console.WriteLine($"\n{sw.ElapsedMilliseconds} ms");
        }
    }
    
    // in-memory caching
    public static async Task<IEnumerable<User>> GetUsersMemoryCache(IMemoryCache cache)
    {
        string cacheKey = "users";
        bool isCacheMiss = false;

        IEnumerable<User>? users = cache.GetOrCreate(cacheKey, entry =>
        {
            isCacheMiss = true;
            return GetData();
        });

        message = isCacheMiss ? "Returned uncached users" : "Returned cached users";
        return users!;
    }

    // Distributed caching with Redis
    public static async Task<IEnumerable<User>> GetUsersDistributedCache(IDatabase cache)
    {
        string cacheKey = "users";

        string? usersString = await cache.StringGetAsync(cacheKey);

        if (usersString is not null)
        {
            IEnumerable<User>? cachedUsers = JsonSerializer.Deserialize<IEnumerable<User>>(usersString);
            
            message = "Returned distributed cached users from Redis";
            return cachedUsers!;
        }

        IEnumerable<User> users = GetData();
        await cache.StringSetAsync(cacheKey, JsonSerializer.Serialize(users));
        
        message = "Returned uncached users";
        
        return users;
    }
    
    public static IEnumerable<User> GetData()
    {
        string json = File.ReadAllText("../../../data/data.json");
        IEnumerable<User> users = JsonSerializer.Deserialize<IEnumerable<User>>(json)!;

        return users;
    }
}