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
        var locations = await busLocationManager.GetLocationsAsync(sessionId, deviceId, UserLanguage);
        var busLocationResponses = locations as BusLocationResponse[] ?? locations.ToArray();
        var lastSearch = await redisCacheService.GetLastSearchAsync(sessionId, deviceId, busLocationResponses);

        var model = viewModelBuilder.Build(busLocationResponses, lastSearch);
        
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