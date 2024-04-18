using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace AspWebApi
{
    public static class DistributedCacheExtensions
    {
        public static async Task<T?> GetAsync<T>(this IDistributedCache cache, string key) where T : class
        {
            var json = await cache.GetStringAsync(key);
            if (string.IsNullOrEmpty(json)) return default;
            return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            });
        }

        public static async Task SetAsync<T>(this IDistributedCache cache, string key, T value) where T: class 
        {
            var json = JsonSerializer.Serialize(value);
            await cache.SetStringAsync(key, json);
        }
    }
}
