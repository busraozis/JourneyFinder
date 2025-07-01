using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using JourneyFinder.Models.Requests;
using JourneyFinder.Models.Responses;
using JourneyFinder.Options;
using JourneyFinder.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace JourneyFinder.Services;

public class JourneyService(
    IHttpClientFactory httpClientFactory,
    IOptions<ObiletApiOptions> apiOptions,
    ILogger<JourneyService> logger)
    : IJourneyService
{
    private readonly ObiletApiOptions _apiOptions = apiOptions.Value;

    public async Task<List<BusJourneyResponse>> GetBusJourneysAsync(BaseRequest<JourneyRequest> request)
    {
        var client = httpClientFactory.CreateClient();
        client.BaseAddress = new Uri(_apiOptions.BaseUrl);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _apiOptions.ApiKey);

        var endpoint = _apiOptions.Endpoints.GetJourneys;
        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        try
        {
            var response = await client.PostAsync(endpoint, content);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                logger.LogError("Failed to get journeys. StatusCode: {StatusCode}, Response: {Response}", response.StatusCode, errorContent);
                return new List<BusJourneyResponse>();
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            var journeyResponse = JsonSerializer.Deserialize<BaseResponse<List<BusJourneyResponse>>>(responseContent, options);

            return journeyResponse?.Data ?? new List<BusJourneyResponse>();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An exception occurred while calling GetBusJourneys.");
            throw;
        }
    }
}
