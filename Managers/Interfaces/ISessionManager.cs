namespace JourneyFinder.Managers.Interfaces;

public interface ISessionManager
{
    Task<(string SessionId, string DeviceId)> GetOrCreateSessionAsync(HttpContext context);
}
