using JourneyFinder.Dtos;
using JourneyFinder.Models.Responses;

namespace JourneyFinder.Managers.Interfaces;

public interface IJourneyManager
{
    Task<IEnumerable<BusJourneyResponse>> GetJourneysAsync(string sessionId, string deviceId, JourneyDto dto);
}
