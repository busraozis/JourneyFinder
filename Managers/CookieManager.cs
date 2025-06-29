using JourneyFinder.Managers.Interfaces;
using JourneyFinder.Settings;
using Microsoft.Extensions.Options;

namespace JourneyFinder.Managers;

public class CookieManager :  ICookieManager
{
    private readonly CookieSettings _cookieSettings;

    public CookieManager(IOptions<CookieSettings> settings)
    {
        _cookieSettings = settings.Value;
    }

    public void WriteSessionCookies(HttpContext context, string sessionId, string deviceId, TimeSpan duration)
    {
        var options = new CookieOptions
        {
            Expires = DateTimeOffset.UtcNow.Add(duration),
            HttpOnly = true,
            IsEssential = true,
            Secure = _cookieSettings.UseSecureCookies,
            SameSite = SameSiteMode.Strict
        };

        context.Response.Cookies.Append("SessionId", sessionId, options);
        context.Response.Cookies.Append("DeviceId", deviceId, options);
    }
    
    public (string SessionId, string DeviceId) GetSessionAndDeviceIds(HttpContext context)
    {
        var sessionId = context.Request.Cookies["SessionId"] ?? string.Empty;
        var deviceId = context.Request.Cookies["DeviceId"] ?? string.Empty;
        return (sessionId, deviceId);
    }

}