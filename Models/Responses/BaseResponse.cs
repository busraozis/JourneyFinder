using System.Text.Json.Serialization;

namespace JourneyFinder.Models.Responses;

public class BaseResponse<T>
{
    [JsonPropertyName("status")] public string Status { get; set; } = string.Empty;

    [JsonPropertyName("data")] public T Data { get; set; } = default!;

    [JsonPropertyName("message")] public string? Message { get; set; }

    [JsonPropertyName("user-message")] public string? UserMessage { get; set; }

    [JsonPropertyName("api-request-id")] public string? ApiRequestId { get; set; }

    [JsonPropertyName("controller")] public string Controller { get; set; } = string.Empty;
    
    [JsonPropertyName("client-request-id")]
    public string? ClientRequestId { get; set; }

    [JsonPropertyName("web-correlation-id")]
    public string? WebCorrelationId { get; set; }

    [JsonPropertyName("correlation-id")]
    public string CorrelationId { get; set; } = default!;

    [JsonPropertyName("parameters")]
    public object? Parameters { get; set; }

}