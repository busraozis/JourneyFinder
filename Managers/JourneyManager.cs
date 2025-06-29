using JourneyFinder.Dtos;
using JourneyFinder.Managers.Interfaces;
using JourneyFinder.Models.Requests;
using JourneyFinder.Models.Responses;
using JourneyFinder.Services.Interfaces;
using Microsoft.Extensions.Caching.Distributed;

namespace JourneyFinder.Managers;

public class JourneyManager : IJourneyManager
{
    private readonly IJourneyService _journeyService;
    private readonly IDistributedCache _cache;

    public JourneyManager(IJourneyService journeyService, IDistributedCache cache)
    {
        _journeyService = journeyService;
        _cache = cache;
    }

    public async Task<IEnumerable<BusJourneyResponse>> GetJourneysAsync(string sessionId, string deviceId, JourneyDto dto)
    {
        var request = new BaseRequest<JourneyRequest>
        {
            DeviceSession = new DeviceSession
            {
                SessionId = sessionId,
                DeviceId = deviceId
            },
            Date = DateTime.UtcNow.ToShortDateString(),
            Language = dto.Language,
            Data = new JourneyRequest
            {
                OriginId = dto.OriginId,
                DestinationId = dto.DestinationId,
                DepartureDate = dto.DepartureDate
            }
        };

        var journeys = await _journeyService.GetBusJourneysAsync(request);

        var searchKey = $"session:{sessionId}:{deviceId}:lastSearch";
        var searchValue = $"{dto.OriginId}|{dto.DestinationId}|{dto.DepartureDate:yyyy-MM-dd}";

        await _cache.SetStringAsync(searchKey, searchValue, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(30)
        });

        return journeys;
    }
}
