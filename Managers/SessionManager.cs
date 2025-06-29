using JourneyFinder.Factories;
using JourneyFinder.Helpers;
using JourneyFinder.Managers.Interfaces;
using JourneyFinder.Services.Interfaces;

namespace JourneyFinder.Managers;

public class SessionManager(
    ISessionService sessionService,
    IRedisCacheService redisCacheService,
    IRequestContextHelper requestContextHelper,
    IDeviceRequestFactory deviceRequestFactory,
    ICookieManager cookieManager)
    : ISessionManager
{
    public async Task<(string SessionId, string DeviceId)> GetOrCreateSessionAsync(HttpContext context)
    {
        var (sessionId, deviceId) = cookieManager.GetSessionAndDeviceIds(context);

        if (await redisCacheService.SessionExistsAsync(sessionId, deviceId))
            return (sessionId, deviceId);

        var requestInfo = requestContextHelper.ExtractRequestInfo(context);
        var deviceRequest = deviceRequestFactory.Create(requestInfo);

        var response = await sessionService.GetSessionAsync(deviceRequest);
        if (response == null)
            throw new Exception("Session not found");

        sessionId = response.SessionId;
        deviceId = response.DeviceId;

        await redisCacheService.SetSessionValidAsync(sessionId, deviceId, TimeSpan.FromDays(30));
        cookieManager.WriteSessionCookies(context, sessionId, deviceId, TimeSpan.FromDays(30));

        return (sessionId, deviceId);
    }
}
