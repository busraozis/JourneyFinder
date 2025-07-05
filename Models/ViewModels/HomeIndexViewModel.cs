using Microsoft.AspNetCore.Mvc.Rendering;

namespace JourneyFinder.Models.ViewModels;

public class HomeIndexViewModel
{
    public List<SelectListItem>? Locations { get; set; }
    public int? OriginId { get; set; }
    public int? DestinationId { get; set; }
    public string? OriginName { get; set; } 
    public string? DestinationName { get; set; }
    public DateTime? DepartureDate { get; set; } = DateTime.Today.AddDays(1);
}