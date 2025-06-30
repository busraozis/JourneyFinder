namespace JourneyFinder.Helpers;

public class RequestContext
{
    public string IpAddress { get; set; } = "";
    public int Port { get; set; }
    public string UserAgent { get; set; } = "";
}

public interface IRequestContextHelper
{
    RequestContext ExtractRequestInfo(HttpContext context);
}

public class RequestContextHelper : IRequestContextHelper
{
    public RequestContext ExtractRequestInfo(HttpContext context)
    {
        var userAgent = context.Request.Headers["User-Agent"].FirstOrDefault() ?? "FakeUserAgent/1.0";
        var ip = context.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";;
        var forwardedIp = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        var ipAddress = !string.IsNullOrEmpty(forwardedIp) ? forwardedIp : ip;

        return new RequestContext
        {
            IpAddress = ipAddress,
            Port = context.Connection.RemotePort,
            UserAgent = userAgent
        };
    }
}
