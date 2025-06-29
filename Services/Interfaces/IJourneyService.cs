using JourneyFinder.Models.Requests;
using JourneyFinder.Models.Responses;

namespace JourneyFinder.Services.Interfaces;

public interface IJourneyService
{
    Task<List<BusJourneyResponse>> GetBusJourneysAsync(BaseRequest<JourneyRequest> request);
}