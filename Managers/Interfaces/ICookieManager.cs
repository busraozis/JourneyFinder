namespace JourneyFinder.Managers.Interfaces;

public interface ICookieManager
{
    (string SessionId, string DeviceId) GetSessionAndDeviceIds(HttpContext context);
    void WriteSessionCookies(HttpContext context, string sessionId, string deviceId, TimeSpan duration);
}