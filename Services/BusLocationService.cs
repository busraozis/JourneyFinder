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
    IConfiguration configuration,
    IOptions<ObiletApiOptions> apiOptions)
    : IBusLocationService
{
    private readonly ObiletApiOptions _apiOptions = apiOptions.Value;

        public async Task<List<BusLocationResponse>> GetBusLocationsAsync(BaseRequest<BusLocationRequest> request)
        {
            var client = httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_apiOptions.BaseUrl);

            var apiClientToken = configuration["ObiletApiKey"];
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", apiClientToken);

            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(_apiOptions.Endpoints.GetBusLocations, content);

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            var busLocationResponse = JsonSerializer.Deserialize<BaseResponse<List<BusLocationResponse>>>(responseContent, options);

            return busLocationResponse?.Data ?? new List<BusLocationResponse>();
        }
    
}