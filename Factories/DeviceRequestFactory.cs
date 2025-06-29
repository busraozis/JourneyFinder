using JourneyFinder.Helpers;
using JourneyFinder.Models.Requests;
using ConnectionInfo = JourneyFinder.Models.Requests.ConnectionInfo;

namespace JourneyFinder.Factories;

public interface IDeviceRequestFactory
{
    DistribusionDeviceRequest Create(RequestContext context);
}

public class DeviceRequestFactory : IDeviceRequestFactory
{
    public DistribusionDeviceRequest Create(RequestContext context)
    {
        var clientInfo = UAParser.Parser.GetDefault().Parse(context.UserAgent);

        return new DistribusionDeviceRequest
        {
            Connection = new ConnectionInfo { IpAddress = context.IpAddress, Port = context.Port },
            Browser = new BrowserInfo
            {
                Name = clientInfo.UA.Family,
                Version = UserAgentHelper.GetBrowserVersion(clientInfo)
            },
            Type = 7,
            Application = new ApplicationInfo
            {
                Version = "1.0.0.0",
                EquipmentId = "distribusion"
            }
        };
    }
}
