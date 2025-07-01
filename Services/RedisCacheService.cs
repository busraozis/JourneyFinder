using JourneyFinder.Models.Dtos;
using JourneyFinder.Models.Responses;
using JourneyFinder.Services.Interfaces;
using Microsoft.Extensions.Caching.Distributed;

namespace JourneyFinder.Services;

public class RedisCacheService(IDistributedCache cache) : IRedisCacheService
{
    public async Task<LastSearchResult?> GetLastSearchAsync(string sessionId, string deviceId, IEnumerable<BusLocationResponse> locations)
    {
        var key = $"session:{sessionId}:{deviceId}:lastSearch";
        var searchData = await cache.GetStringAsync(key);

        if (string.IsNullOrEmpty(searchData)) return null;

        var parts = searchData.Split('|');
        if (parts.Length != 3) return null;

        if (int.TryParse(parts[0], out var originId) &&
            int.TryParse(parts[1], out var destinationId) &&
            DateTime.TryParse(parts[2], out var departureDate))
        {
            var busLocationResponses = locations as BusLocationResponse[] ?? locations.ToArray();
            var originName = busLocationResponses.FirstOrDefault(x => x.Id == originId)?.Name;
            var destinationName = busLocationResponses.FirstOrDefault(x => x.Id == destinationId)?.Name;

            return new LastSearchResult
            {
                OriginId = originId,
                DestinationId = destinationId,
                DepartureDate = departureDate,
                OriginName = originName,
                DestinationName = destinationName
            };
        }

        return null;
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