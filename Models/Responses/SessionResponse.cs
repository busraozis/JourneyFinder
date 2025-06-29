using System.Text.Json.Serialization;

namespace JourneyFinder.Models.Responses;

public class SessionResponse
{
    [JsonPropertyName("session-id")]
    public string SessionId { get; set; } = default!;

    [JsonPropertyName("device-id")]
    public string DeviceId { get; set; } = default!;

    [JsonPropertyName("affiliate")]
    public string? Affiliate { get; set; }

    [JsonPropertyName("device-type")]
    public int DeviceType { get; set; }

    [JsonPropertyName("device")]
    public string? Device { get; set; }

    [JsonPropertyName("ip-country")]
    public string IpCountry { get; set; } = default!;

    [JsonPropertyName("clean-session-id")]
    public long CleanSessionId { get; set; }

    [JsonPropertyName("clean-device-id")]
    public long CleanDeviceId { get; set; }

    [JsonPropertyName("ip-address")]
    public string? IpAddress { get; set; }
}