using JourneyFinder.Models.Requests;
using JourneyFinder.Models.ViewModels;
using JourneyFinder.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Caching.Distributed;
using ConnectionInfo = JourneyFinder.Models.Requests.ConnectionInfo;

namespace JourneyFinder.Controllers;

public class HomeController : BaseController
{
    private readonly ISessionService _sessionService;
    private readonly IBusLocationService _busLocationService;
    private readonly IDistributedCache _cache;

    public HomeController(ISessionService sessionService, IBusLocationService busLocationService, IDistributedCache cache)
    {
        _sessionService = sessionService;
        _busLocationService = busLocationService;
        _cache = cache;
    }

    public async Task<IActionResult> Index()
    {
        var sessionId = Request.Cookies["SessionId"];
        var deviceId = Request.Cookies["DeviceId"];

        string redisKey = $"session:{sessionId}:{deviceId}";

        var cachedValue = await _cache.GetStringAsync(redisKey);

        if (string.IsNullOrEmpty(sessionId) || string.IsNullOrEmpty(deviceId) || string.IsNullOrEmpty(cachedValue))
        {
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty;
            var forwardedIp = HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            var ipAddress = !string.IsNullOrEmpty(forwardedIp) ? forwardedIp : ip;

            string userAgent = HttpContext.Request.Headers["User-Agent"].ToString();
            var uaParser = UAParser.Parser.GetDefault();
            var clientInfo = uaParser.Parse(userAgent);

            var deviceRequest = new DistribusionDeviceRequest
            {
                Connection = new ConnectionInfo()
                {
                    IpAddress = "192.168.1.188", //ipAddress,
                    Port = HttpContext.Connection.RemotePort
                },
                Browser = new BrowserInfo
                {
                    Name = clientInfo.UA.Family,
                    Version = "18.3" //$"{clientInfo.UA.Major}.{clientInfo.UA.Minor}.{clientInfo.UA.Patch}"
                },
                Type = 7,
                Application = new ApplicationInfo
                {
                    Version = "1.0.0.0",
                    EquipmentId = "distribusion"
                }
            };

            var sessionResponse = await _sessionService.GetSessionAsync(deviceRequest);

            if (sessionResponse == null || string.IsNullOrEmpty(sessionResponse.SessionId) || string.IsNullOrEmpty(sessionResponse.DeviceId))
            {
                return StatusCode(500, "Yeni session alınamadı");
            }

            sessionId = sessionResponse.SessionId;
            deviceId = sessionResponse.DeviceId;

            redisKey = $"session:{sessionId}:{deviceId}";

            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(30)
            };
            await _cache.SetStringAsync(redisKey, "valid", cacheOptions);

            var cookieOptions = new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddDays(30),
                HttpOnly = true,
                IsEssential = true,
                Secure = false,
                SameSite = SameSiteMode.Strict
            };
            Response.Cookies.Append("SessionId", sessionId, cookieOptions);
            Response.Cookies.Append("DeviceId", deviceId, cookieOptions);
        }

        var baseRequest = new BaseRequest<BusLocationRequest>
        {
            DeviceSession = new DeviceSession
            {
                SessionId = sessionId,
                DeviceId = deviceId
            },
            Date = DateTime.UtcNow.ToShortDateString(),
            Data = null, 
            Language = UserLanguage
        };

        var locations = await _busLocationService.GetBusLocationsAsync(baseRequest);
        
        var searchKey = $"session:{sessionId}:{deviceId}:lastSearch";
        var searchData = await _cache.GetStringAsync(searchKey);

        int? originId = null;
        int? destinationId = null;
        string? originName = null;
        string? destinationName = null;
        DateTime departureDate = DateTime.Today.AddDays(1); 

        if (!string.IsNullOrEmpty(searchData))
        {
            var parts = searchData.Split('|');
            if (parts.Length == 3 &&
                int.TryParse(parts[0], out var origin) &&
                int.TryParse(parts[1], out var destination) &&
                DateTime.TryParse(parts[2], out var departure))
            {
                originId = origin;
                destinationId = destination;
                departureDate = departure;
                originName = locations.FirstOrDefault(x => x.Id == originId)?.Name;
                destinationName = locations.FirstOrDefault(x => x.Id == destinationId)?.Name;

            }
        }

        var model = new HomeIndexViewModel
        {
            Locations = locations.Select(loc => new SelectListItem
            {
                Text = loc.Name,
                Value = loc.Id.ToString()
            }).ToList(),
            OriginId = originId,
            DestinationId = destinationId,
            DepartureDate = departureDate,
            OriginName = originName,
            DestinationName = destinationName,
        };

        return View(model);
    }
}