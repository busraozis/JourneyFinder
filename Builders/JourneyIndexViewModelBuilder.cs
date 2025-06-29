using JourneyFinder.Models.Responses;
using JourneyFinder.Models.ViewModels;

namespace JourneyFinder.Builders;

public interface IJourneyViewModelBuilder
{
    JourneyIndexViewModel Build(string originName, string destinationName, DateTime departureDate,
        IEnumerable<BusJourneyResponse> journeys);
}

public class JourneyViewModelBuilder : IJourneyViewModelBuilder
{
    public JourneyIndexViewModel Build(string originName, string destinationName, DateTime departureDate, IEnumerable<BusJourneyResponse> journeys)
    {
        return new JourneyIndexViewModel
        {
            OriginName = originName,
            DestinationName = destinationName,
            SearchDate = departureDate,
            Journeys = journeys
                .Where(j => j.Journey.Departure.Date == departureDate.Date)
                .OrderBy(j => j.Journey.Departure)
                .ToList()
        };
    }
}
