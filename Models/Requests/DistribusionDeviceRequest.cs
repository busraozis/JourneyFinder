using System.Text.Json.Serialization;

namespace JourneyFinder.Models.Requests;

public class ConnectionInfo
{
    [JsonPropertyName("ip-address")]
    public string? IpAddress { get; set; } = string.Empty;
    
    [JsonPropertyName("port")]
    public int Port { get; set; }
    
}

public class BrowserInfo
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("version")]
    public string Version { get; set; } = string.Empty;
}

public class ApplicationInfo
{
    [JsonPropertyName("version")]
    public string Version { get; set; } = string.Empty;

    [JsonPropertyName("equipment-id")]
    public string EquipmentId { get; set; } = string.Empty;
    
}

public class DistribusionDeviceRequest
{
    [JsonPropertyName("type")]
    public int Type { get; set; }

    [JsonPropertyName("connection")]
        public ConnectionInfo Connection { get; set; } = new ConnectionInfo();

    [JsonPropertyName("application")]
    public ApplicationInfo Application { get; set; } = new ApplicationInfo();
    
    [JsonPropertyName("browser")]
    public BrowserInfo Browser { get; set; } = new BrowserInfo();
}

