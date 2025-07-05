namespace JourneyFinder.Models.Dtos;

public class JourneyDto
{
    public int OriginId { get; set; }
    public int DestinationId { get; set; }
    public DateTime DepartureDate { get; set; }
    public string Language { get; set; }
    public string OriginName { get; set; }
    public string DestinationName { get; set; }
}