using JourneyFinder.Models.Requests;
using JourneyFinder.Models.Responses;
using JourneyFinder.Services.Interfaces;

namespace JourneyFinder.Managers;

public interface IBusLocationManager
{
    Task<IEnumerable<BusLocationResponse>> SearchLocationsAsync(string? keyword, string sessionId, string deviceId, string language);

}

public class BusLocationManager(IBusLocationService service) : IBusLocationManager
{
    public async Task<IEnumerable<BusLocationResponse>> SearchLocationsAsync(string? keyword, string sessionId, string deviceId, string language)
    {
        var request = new BaseRequest<string?>
        {
            DeviceSession = new DeviceSession
            {
                SessionId = sessionId,
                DeviceId = deviceId
            },
            Date = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss"),
            Language = language,
            Data = keyword 
        };

        return await service.GetBusLocationsAsync(request);
    }
}
