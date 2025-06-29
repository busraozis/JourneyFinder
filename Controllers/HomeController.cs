using JourneyFinder.Managers.Interfaces;
using JourneyFinder.Models.Requests;
using JourneyFinder.Models.ViewModels;
using JourneyFinder.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace JourneyFinder.Controllers;

public class HomeController : BaseController
{
    private readonly IBusLocationService _busLocationService;
    private readonly ISessionManager _sessionManager;
    private readonly IRedisCacheService _redisCacheService;

    public HomeController(ISessionManager sessionManager, IBusLocationService busLocationService, IRedisCacheService redisCacheService)
    {
        _sessionManager = sessionManager;
        _busLocationService = busLocationService;
        _redisCacheService = redisCacheService;
    }

    public async Task<IActionResult> Index()
    {
        var (sessionId, deviceId) = await _sessionManager.GetOrCreateSessionAsync(HttpContext);

        var baseRequest = new BaseRequest<BusLocationRequest>
        {
            DeviceSession = new DeviceSession
            {
                SessionId = sessionId,
                DeviceId = deviceId
            },
            Date = DateTime.UtcNow.ToShortDateString(),
            Data = null!, 
            Language = UserLanguage
        };

        var locations = await _busLocationService.GetBusLocationsAsync(baseRequest);
        var lastSearch = await _redisCacheService.GetLastSearchAsync(sessionId, deviceId, locations);


        var model = new HomeIndexViewModel
        {
            Locations = locations.Select(loc => new SelectListItem
            {
                Text = loc.Name,
                Value = loc.Id.ToString()
            }).ToList(),
            OriginId = lastSearch?.OriginId,
            DestinationId = lastSearch?.DestinationId,
            DepartureDate = lastSearch?.DepartureDate,
            OriginName = lastSearch?.OriginName,
            DestinationName = lastSearch?.DestinationName,
        };

        return View(model);
    }
}