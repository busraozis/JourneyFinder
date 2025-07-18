using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using JourneyFinder.Models.Requests;
using JourneyFinder.Models.Responses;
using JourneyFinder.Options;
using JourneyFinder.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace JourneyFinder.Services;

public class SessionService(
    IHttpClientFactory httpClientFactory,
    IOptions<ObiletApiOptions> apiOptions,
    ILogger<SessionService> logger)
    : ISessionService
{
    private readonly ObiletApiOptions _apiOptions = apiOptions.Value;

    public async Task<SessionResponse?> GetSessionAsync(DistribusionDeviceRequest request)
    {
        var client = httpClientFactory.CreateClient();
        client.BaseAddress = new Uri(_apiOptions.BaseUrl);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _apiOptions.ApiKey);

        var endpoint = _apiOptions.Endpoints.GetSession;
        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        try
        {
            var response = await client.PostAsync(endpoint, content);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                logger.LogError("Failed to retrieve session. StatusCode: {StatusCode}, Response: {Response}", response.StatusCode, errorContent);
                return null;
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            var sessionResponse = JsonSerializer.Deserialize<BaseResponse<SessionResponse>>(responseContent, options);

            if (sessionResponse?.Data != null)
            {
                return new SessionResponse
                {
                    SessionId = sessionResponse.Data.SessionId,
                    DeviceId = sessionResponse.Data.DeviceId
                };
            }

            logger.LogWarning("Session response was null or invalid.");
            return null;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An exception occurred while retrieving session.");
            throw;
        }
    }
}