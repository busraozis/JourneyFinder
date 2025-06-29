using System.Text.Json.Serialization;

namespace JourneyFinder.Models.Requests;

public class JourneyRequest
{
    [JsonPropertyName("origin-id")]
    public int OriginId { get; set; }
    [JsonPropertyName("destination-id")]
    public int DestinationId { get; set; }
    [JsonPropertyName("departure-date")]
    public DateTime DepartureDate { get; set; }
}