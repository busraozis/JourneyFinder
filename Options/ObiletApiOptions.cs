namespace JourneyFinder.Options;

public class ObiletApiOptions
{
    public string BaseUrl { get; set; }
    public EndpointsConfig Endpoints { get; set; }

    public class EndpointsConfig
    {
        public string GetBusLocations { get; set; }
        public string GetSession { get; set; }
        public string GetJourneys { get; set; }
    }
}