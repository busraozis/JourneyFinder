using UAParser;

namespace JourneyFinder.Helpers;

public static class UserAgentHelper
{
    public static string GetBrowserVersion(ClientInfo clientInfo)
    {
        var browser = clientInfo.UA;
        var name = browser.Family;
        var versionParts = new List<string>();

        if (!string.IsNullOrWhiteSpace(browser.Major))
            versionParts.Add(browser.Major);
        if (!string.IsNullOrWhiteSpace(browser.Minor))
            versionParts.Add(browser.Minor);
        if (!string.IsNullOrWhiteSpace(browser.Patch))
            versionParts.Add(browser.Patch);

        var version = versionParts.Count > 0 ? string.Join(".", versionParts) : "unknown";

        return $"{name} {version}";
    }
}
