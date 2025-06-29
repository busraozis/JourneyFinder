using JourneyFinder.Models.Responses;

namespace JourneyFinder.Models.ViewModels;

public class JourneyIndexViewModel
{
    public string? OriginName { get; set; }
    public string? DestinationName { get; set; }
    
    public DateTime SearchDate { get; set; }
    public List<BusJourneyResponse> Journeys { get; set; }
}