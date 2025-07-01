using JourneyFinder.Models.Dtos;
using JourneyFinder.Models.Responses;
using JourneyFinder.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace JourneyFinder.Builders;

public interface IHomeViewModelBuilder
{
    HomeIndexViewModel Build(IEnumerable<BusLocationResponse> locations, LastSearchResult? lastSearch);
}

public class HomeViewModelBuilder : IHomeViewModelBuilder
{
    public HomeIndexViewModel Build(IEnumerable<BusLocationResponse> locations, LastSearchResult? lastSearch)
    {
        return new HomeIndexViewModel
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
            DestinationName = lastSearch?.DestinationName
        };
    }
}
