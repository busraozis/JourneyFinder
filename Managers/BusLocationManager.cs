using JourneyFinder.Models.Requests;
using JourneyFinder.Models.Responses;
using JourneyFinder.Services.Interfaces;

namespace JourneyFinder.Managers;

public interface IBusLocationManager
{
    Task<IEnumerable<BusLocationResponse>> GetLocationsAsync(string sessionId, string deviceId, string language);
}

public class BusLocationManager(IBusLocationService service) : IBusLocationManager
{
    public async Task<IEnumerable<BusLocationResponse>> GetLocationsAsync(string sessionId, string deviceId, string language)
    {
        var request = new BaseRequest<BusLocationRequest>
        {
            DeviceSession = new DeviceSession { SessionId = sessionId, DeviceId = deviceId },
            Date = DateTime.UtcNow.ToShortDateString(),
            Language = language,
            Data = null!
        };

        return await service.GetBusLocationsAsync(request);
    }
}
