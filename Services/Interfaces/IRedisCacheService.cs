using JourneyFinder.Models.Dtos;
using JourneyFinder.Models.Responses;

namespace JourneyFinder.Services.Interfaces;

public interface IRedisCacheService
{
    Task<LastSearchResult?> GetLastSearchAsync(string sessionId, string deviceId);
    Task<bool> SessionExistsAsync(string sessionId, string deviceId);
    Task SetSessionValidAsync(string sessionId, string deviceId, TimeSpan duration);
}