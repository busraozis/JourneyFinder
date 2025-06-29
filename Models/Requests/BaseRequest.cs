using System.Text.Json.Serialization;

namespace JourneyFinder.Models.Requests;

public class BaseRequest<T>
{
        [JsonPropertyName("device-session")]
        public required DeviceSession DeviceSession { get; set; }
        
        [JsonPropertyName("date")]
        public string Date { get; set; } = string.Empty;
        
        [JsonPropertyName("language")]
        public string Language { get; set; } = "en-EN"; 
        
        [JsonPropertyName("data")]
        public T Data { get; set; } = default!;
}
