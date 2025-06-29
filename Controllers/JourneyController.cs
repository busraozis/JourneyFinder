using JourneyFinder.Filters;
using JourneyFinder.Models.Requests;
using JourneyFinder.Models.Responses;
using JourneyFinder.Models.ViewModels;
using JourneyFinder.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace JourneyFinder.Controllers;

public class JourneyController : BaseController
{
    private readonly IJourneyService _journeyService;
    private readonly IDistributedCache _cache;

    public JourneyController(IJourneyService journeyService, IDistributedCache cache)
    {
        _journeyService = journeyService;
        _cache = cache;
    }

    [ServiceFilter(typeof(JourneyValidationFilter))]
    [HttpPost]
    public async Task<IActionResult> Index(int originId, int destinationId, DateTime departureDate, string originName, string destinationName)
    {
        var sessionId = Request.Cookies["SessionId"];
        var deviceId = Request.Cookies["DeviceId"];

        if (string.IsNullOrEmpty(sessionId) || string.IsNullOrEmpty(deviceId))
        {
            return StatusCode(400, "Session bilgileri eksik");
        }
        
        var request = new BaseRequest<JourneyRequest>
        {
            DeviceSession = new DeviceSession
            {
                SessionId = sessionId,
                DeviceId = deviceId
            },
            Date = DateTime.UtcNow.ToShortDateString(),
            Language = UserLanguage,
            Data = new JourneyRequest
            {
                OriginId = originId,
                DestinationId = destinationId,
                DepartureDate = departureDate
            }
        };

        var journeys = await _journeyService.GetBusJourneysAsync(request);

        var searchKey = $"session:{sessionId}:{deviceId}:lastSearch";
        var searchValue = $"{originId}|{destinationId}|{departureDate:yyyy-MM-dd}";

        var cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(30)
        };

        await _cache.SetStringAsync(searchKey, searchValue, cacheOptions);
        
        var vm = new JourneyIndexViewModel
        {
            OriginName = originName,
            DestinationName = destinationName,
            SearchDate = departureDate,
            Journeys = new List<BusJourneyResponse>(journeys.Where(j => j.Journey.Departure.Date == departureDate.Date).OrderBy(j  => j.Journey.Departure))
        };

        return View(vm);
    }
}
