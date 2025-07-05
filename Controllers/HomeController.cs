using JourneyFinder.Builders;
using JourneyFinder.Managers;
using JourneyFinder.Managers.Interfaces;
using JourneyFinder.Models.Responses;
using JourneyFinder.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace JourneyFinder.Controllers;

public class HomeController(
    ISessionManager sessionManager,
    IRedisCacheService redisCacheService,
    IHomeViewModelBuilder viewModelBuilder,
    IBusLocationManager busLocationManager)
    : BaseController
{
    public async Task<IActionResult> Index()
    {
        var (sessionId, deviceId) = await sessionManager.GetOrCreateSessionAsync(HttpContext);
        var lastSearch = await redisCacheService.GetLastSearchAsync(sessionId, deviceId);
        var origin = await busLocationManager.SearchLocationsAsync(lastSearch?.OriginName ?? null, sessionId, deviceId, UserLanguage);
        var destination = await busLocationManager.SearchLocationsAsync(lastSearch?.DestinationName ?? null, sessionId, deviceId, UserLanguage);
        
        var originResponse = origin.ToList();
        var destinationResponse = destination.ToList();
        
        var locations = originResponse.Union(destinationResponse) as BusLocationResponse[];

        var model = viewModelBuilder.Build(locations, lastSearch);
        
        ViewData["ShowJourneyInfo"] = false;
        return View(model);
    }
    
    [HttpPost]
    public async Task<IActionResult> SearchLocations([FromBody] string? input)
    {
        var keyword = input;

        var (sessionId, deviceId) = await sessionManager.GetOrCreateSessionAsync(HttpContext);
        var results = await busLocationManager.SearchLocationsAsync(keyword, sessionId, deviceId, UserLanguage);

        var formatted = results.Select(r => new { text = r.LongName, value = r.Id });
        return Json(formatted);
    }
}