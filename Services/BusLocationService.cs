using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using JourneyFinder.Models.Requests;
using JourneyFinder.Models.Responses;
using JourneyFinder.Options;
using JourneyFinder.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace JourneyFinder.Services;

public class BusLocationService(
    IHttpClientFactory httpClientFactory,
    IOptions<ObiletApiOptions> apiOptions,
    ILogger<BusLocationService> logger)
    : IBusLocationService
{
    private readonly ObiletApiOptions _apiOptions = apiOptions.Value;

    public async Task<List<BusLocationResponse>> GetBusLocationsAsync(BaseRequest<string?> request)
    {
        var client = httpClientFactory.CreateClient();
        client.BaseAddress = new Uri(_apiOptions.BaseUrl);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _apiOptions.ApiKey);

        var endpoint = _apiOptions.Endpoints.GetBusLocations;
        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        try
        {
            var response = await client.PostAsync(endpoint, content);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                logger.LogError("Failed to get bus locations. StatusCode: {StatusCode}, Response: {Response}", response.StatusCode, errorContent);
                return new List<BusLocationResponse>();
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            var busLocationResponse = JsonSerializer.Deserialize<BaseResponse<List<BusLocationResponse>>>(responseContent, options);

            return busLocationResponse?.Data ?? new List<BusLocationResponse>();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An exception occurred while calling GetBusLocations.");
            throw;
        }
    }
}