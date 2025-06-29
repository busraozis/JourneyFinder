using System.Text.Json.Serialization;

namespace JourneyFinder.Models.Responses;

public class BusLocationResponse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("parent-id")]
    public int ParentId { get; set; }
    
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;
    
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonPropertyName("geo-location")]
    public GeoLocation GeoLocation { get; set; } = default!;
    
    [JsonPropertyName("zoom")] 
    public double Zoom { get; set; }
    
    [JsonPropertyName("tz-code")]
    public string TzCode { get; set; } = string.Empty;
    
    [JsonPropertyName("weather-code")]
    public string? WeatherCode { get; set; }
    
    [JsonPropertyName("rank")]
    public int Rank { get; set; }
    
    [JsonPropertyName("reference-code")]
    public string ReferenceCode { get; set; } = string.Empty;
    
    [JsonPropertyName("city-id")]
    public int CityId { get; set; }

    [JsonPropertyName("reference-country")]
    public string? ReferenceCountry { get; set; }
    
    [JsonPropertyName("country-id")]
    public int CountryId { get; set; }
    
    [JsonPropertyName("keywords")]
    public string Keywords { get; set; } = string.Empty;
    
    [JsonPropertyName("city-name")]
    public string CityName { get; set; } = string.Empty;
    
    [JsonPropertyName("languages")]
    public string? Languages { get; set; }
    
    [JsonPropertyName("country-name")]
    public string CountryName { get; set; } = string.Empty;
    
    [JsonPropertyName("code")]
    public string? Code { get; set; }
    
    [JsonPropertyName("show-country")]
    public bool ShowCountry { get; set; }
    
    [JsonPropertyName("area-code")]
    public string? AreaCode { get; set; }
    
    [JsonPropertyName("long-name")]
    public string LongName { get; set; } = string.Empty;
    
    [JsonPropertyName("is-city-center")]
    public bool IsCityCenter { get; set; }
}

public class GeoLocation
{
    [JsonPropertyName("latitude")] 
    public double Latitude { get; set; }
    [JsonPropertyName("longitude")] 
    public double Longitude { get; set; }
    [JsonPropertyName("zoom")] 
    public double Zoom { get; set; }
}