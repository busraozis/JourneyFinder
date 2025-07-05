using JourneyFinder.Models.Dtos;
using JourneyFinder.Models.Responses;
using JourneyFinder.Services.Interfaces;
using Microsoft.Extensions.Caching.Distributed;

namespace JourneyFinder.Services;

public class RedisCacheService(IDistributedCache cache) : IRedisCacheService
{
    public async Task<LastSearchResult?> GetLastSearchAsync(string sessionId, string deviceId)
    {
        var key = $"session:{sessionId}:{deviceId}:lastSearch";
        var searchData = await cache.GetStringAsync(key);

        if (string.IsNullOrEmpty(searchData)) return null;

        var parts = searchData.Split('|');
        if (parts.Length != 3) return null;
        
        var originName = parts[0];
        var destinationName = parts[1];
        var departureDateOk = DateTime.TryParse(parts[2], out var departureDate);

        if (!departureDateOk) return null;

        return new LastSearchResult
        {
            OriginName = originName,
            DestinationName = destinationName,
            DepartureDate = departureDate
        };
    }

    public async Task<bool> SessionExistsAsync(string sessionId, string deviceId)
    {
        var key = GetSessionKey(sessionId, deviceId);
        var value = await cache.GetStringAsync(key);
        return !string.IsNullOrEmpty(value);
    }

    public async Task SetSessionValidAsync(string sessionId, string deviceId, TimeSpan duration)
    {
        var key = GetSessionKey(sessionId, deviceId);
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = duration
        };
        await cache.SetStringAsync(key, "valid", options);
    }
    
    private string GetSessionKey(string sessionId, string deviceId) => $"session:{sessionId}:{deviceId}";
}