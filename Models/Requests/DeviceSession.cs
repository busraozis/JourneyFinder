using System.Text.Json.Serialization;

namespace JourneyFinder.Models.Requests;

public class DeviceSession
{
    [JsonPropertyName("session-id")]
    public string SessionId { get; set; } = string.Empty;
    
    [JsonPropertyName("device-id")]
    public string DeviceId { get; set; } = string.Empty;
}