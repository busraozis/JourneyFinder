using JourneyFinder.Dtos;
using JourneyFinder.Filters;
using JourneyFinder.Managers.Interfaces;
using JourneyFinder.Models.Responses;
using JourneyFinder.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace JourneyFinder.Controllers;

public class JourneyController : BaseController
{
    private readonly IJourneyManager _journeyManager;

    public JourneyController(IJourneyManager journeyManager)
    {
        _journeyManager = journeyManager;
    }

    [ServiceFilter(typeof(JourneyValidationFilter))]
    [HttpPost]
    public async Task<IActionResult> Index(int originId, int destinationId, DateTime departureDate, string originName, string destinationName)
    {
        var sessionId = Request.Cookies["SessionId"];
        var deviceId = Request.Cookies["DeviceId"];

        if (string.IsNullOrEmpty(sessionId) || string.IsNullOrEmpty(deviceId))
        {
            return StatusCode(400, "Missing session info");
        }

        var dto = new JourneyDto
        {
            OriginId = originId,
            DestinationId = destinationId,
            DepartureDate = departureDate,
            Language = UserLanguage
        };

        var journeys = await _journeyManager.GetJourneysAsync(sessionId, deviceId, dto);
        
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
