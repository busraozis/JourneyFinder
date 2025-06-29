using JourneyFinder.Builders;
using JourneyFinder.Dtos;
using JourneyFinder.Filters;
using JourneyFinder.Managers.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace JourneyFinder.Controllers;

public class JourneyController(IJourneyManager journeyManager, IJourneyViewModelBuilder journeyViewModelBuilder)
    : BaseController
{

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

        var journeys = await journeyManager.GetJourneysAsync(sessionId, deviceId, dto);
        var vm = journeyViewModelBuilder.Build(originName, destinationName, departureDate, journeys);

        return View(vm);
    }
}
