using System.Net;
using System.Text;
using System.Text.Json;
using JourneyFinder.Models.Requests;
using JourneyFinder.Models.Responses;
using JourneyFinder.Options;
using JourneyFinder.Services;
using Microsoft.Extensions.Options;
using Moq;
using Microsoft.Extensions.Logging;


namespace JourneyFinder.Tests.Services;

[TestFixture]
public class BusLocationServiceTests
{
    private Mock<IHttpClientFactory> _httpClientFactoryMock;
    private Mock<ILogger<BusLocationService>> _loggerMock;
    private IOptions<ObiletApiOptions> _apiOptions;

    [SetUp]
    public void Setup()
    {
        _httpClientFactoryMock = new Mock<IHttpClientFactory>();
        _loggerMock = new Mock<ILogger<BusLocationService>>();

        var apiOptionsMock = new Mock<IOptions<ObiletApiOptions>>();
        apiOptionsMock.Setup(o => o.Value).Returns(new ObiletApiOptions
        {
            BaseUrl = "https://fakeapi.com",
            ApiKey = "dummy-key",
            Endpoints = new()
            {
                GetBusLocations = "/bus-locations"
            }
        });
        _apiOptions = apiOptionsMock.Object;
    }

    [Test]
    public async Task GetBusLocationsAsync_ReturnsList_WhenResponseIsSuccessful()
    {
        // Arrange
        var request = new BaseRequest<BusLocationRequest>
        {
            DeviceSession = new DeviceSession(),
            Data = new BusLocationRequest()
        };

        var expectedData = new List<BusLocationResponse>
        {
            new() { Id = 1, Name = "Istanbul" }
        };

        var mockResponse = new BaseResponse<List<BusLocationResponse>>
        {
            Data = expectedData
        };

        var json = JsonSerializer.Serialize(mockResponse);
        var handler = new MockHttpMessageHandler(json, HttpStatusCode.OK);
        var client = new HttpClient(handler)
        {
            BaseAddress = new Uri(_apiOptions.Value.BaseUrl)
        };

        _httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(client);

        var service = new BusLocationService(_httpClientFactoryMock.Object, _apiOptions, _loggerMock.Object);

        // Act
        var result = await service.GetBusLocationsAsync(request);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result[0].Name, Is.EqualTo("Istanbul"));
    }

    private class MockHttpMessageHandler : HttpMessageHandler
    {
        private readonly string _responseContent;
        private readonly HttpStatusCode _statusCode;

        public MockHttpMessageHandler(string content, HttpStatusCode statusCode)
        {
            _responseContent = content;
            _statusCode = statusCode;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new HttpResponseMessage
            {
                StatusCode = _statusCode,
                Content = new StringContent(_responseContent, Encoding.UTF8, "application/json")
            });
        }
    }
}
