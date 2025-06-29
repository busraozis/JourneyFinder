using JourneyFinder.Models.Requests;
using JourneyFinder.Models.Responses;

namespace JourneyFinder.Services.Interfaces;

public interface IBusLocationService
{
    Task<List<BusLocationResponse>> GetBusLocationsAsync(BaseRequest<BusLocationRequest> request);
}