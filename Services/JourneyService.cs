using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using JourneyFinder.Models.Requests;
using JourneyFinder.Models.Responses;
using JourneyFinder.Options;
using JourneyFinder.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace JourneyFinder.Services;

public class JourneyService : IJourneyService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ObiletApiOptions _apiOptions;

    public JourneyService(IHttpClientFactory httpClientFactory, IConfiguration configuration, IOptions<ObiletApiOptions> apiOptions)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _apiOptions = apiOptions.Value;
    }

    public async Task<List<BusJourneyResponse>> GetBusJourneysAsync(BaseRequest<JourneyRequest> request)
    {
        var client = _httpClientFactory.CreateClient();
        client.BaseAddress = new Uri(_apiOptions.BaseUrl);
        
        var apiClientToken = _configuration["ObiletApiKey"];
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", apiClientToken);

        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await client.PostAsync(_apiOptions.Endpoints.GetJourneys, content);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("API isteği başarısız oldu");
        }

        var responseContent = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        var journeyResponse = JsonSerializer.Deserialize<BaseResponse<List<BusJourneyResponse>>>(responseContent, options);

        return journeyResponse?.Data ?? new List<BusJourneyResponse>();
    }
}
