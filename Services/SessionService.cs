using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using JourneyFinder.Models.Requests;
using JourneyFinder.Models.Responses;
using JourneyFinder.Options;
using JourneyFinder.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace JourneyFinder.Services;

public class SessionService : ISessionService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ObiletApiOptions _apiOptions;

    public SessionService(IHttpClientFactory httpClientFactory, IConfiguration configuration, IOptions<ObiletApiOptions> apiOptions)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _apiOptions = apiOptions.Value;
    }
    
    public async Task<SessionResponse?> GetSessionAsync(DistribusionDeviceRequest request)
    {
        var client = _httpClientFactory.CreateClient();
        client.BaseAddress = new Uri(_apiOptions.BaseUrl);

        var apiClientToken = _configuration["ObiletApiKey"];
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", apiClientToken);

        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await client.PostAsync(_apiOptions.Endpoints.GetSession, content);

        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        var sessionResponse = JsonSerializer.Deserialize<BaseResponse<SessionResponse>>(responseContent, options);

        if (sessionResponse != null)
            return new SessionResponse
            {
                SessionId = sessionResponse.Data.SessionId,
                DeviceId = sessionResponse.Data.DeviceId
            };
        return null;
    }
}