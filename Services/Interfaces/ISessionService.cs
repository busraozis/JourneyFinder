using JourneyFinder.Models.Requests;
using JourneyFinder.Models.Responses;

namespace JourneyFinder.Services.Interfaces;

public interface ISessionService
{
    Task<SessionResponse?> GetSessionAsync(DistribusionDeviceRequest request);
}