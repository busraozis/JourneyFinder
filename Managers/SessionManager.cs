using JourneyFinder.Factories;
using JourneyFinder.Helpers;
using JourneyFinder.Managers.Interfaces;
using JourneyFinder.Services.Interfaces;

namespace JourneyFinder.Managers;

public class SessionManager : ISessionManager
{
    private readonly ISessionService _sessionService;
    private readonly IRedisCacheService _redisCacheService;
    private readonly ICookieManager _cookieManager;
    private readonly IRequestContextHelper _requestContextHelper;
    private readonly IDeviceRequestFactory  _deviceRequestFactory;

    public SessionManager(ISessionService sessionService, IRedisCacheService redisCacheService, IRequestContextHelper requestContextHelper, IDeviceRequestFactory deviceRequestFactory, ICookieManager cookieManager)
    {
        _sessionService = sessionService;
        _redisCacheService = redisCacheService;
        _requestContextHelper = requestContextHelper;
        _deviceRequestFactory = deviceRequestFactory;
        _cookieManager = cookieManager;
    }

    public async Task<(string SessionId, string DeviceId)> GetOrCreateSessionAsync(HttpContext context)
    {
        var (sessionId, deviceId) = _cookieManager.GetSessionAndDeviceIds(context);

        if (await _redisCacheService.SessionExistsAsync(sessionId, deviceId))
            return (sessionId, deviceId);

        var requestInfo = _requestContextHelper.ExtractRequestInfo(context);
        var deviceRequest = _deviceRequestFactory.Create(requestInfo);

        var response = await _sessionService.GetSessionAsync(deviceRequest);
        if (response == null)
            throw new Exception("Session not found");

        sessionId = response.SessionId;
        deviceId = response.DeviceId;

        await _redisCacheService.SetSessionValidAsync(sessionId, deviceId, TimeSpan.FromDays(30));
        _cookieManager.WriteSessionCookies(context, sessionId, deviceId, TimeSpan.FromDays(30));

        return (sessionId, deviceId);
    }
}
