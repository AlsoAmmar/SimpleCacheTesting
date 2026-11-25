using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Caching.Memory;
using SimpleCacheTesting.Model;

namespace SimpleCacheTesting;

class Program
{
    public static string message = "";
    
    static void Main(string[] args)
    {
        IMemoryCache cache = new MemoryCache(new MemoryCacheOptions());
        
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
            
            IEnumerable<User> users = GetUsersMemoryCache(cache).Result;

            foreach (var user in users)
            {
                Console.WriteLine($"{user.Name} {user.Age}");
            }
            
            sw.Stop();

            Console.WriteLine($"\n{message}");
            Console.WriteLine($"\n{sw.ElapsedMilliseconds} ms");
        }
    }
    
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

    public static IEnumerable<User> GetData()
    {
        string json = File.ReadAllText("../../../data/data.json");
        IEnumerable<User> users = JsonSerializer.Deserialize<IEnumerable<User>>(json)!;

        return users;
    }
}