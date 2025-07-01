using Microsoft.AspNetCore.Mvc.Filters;

namespace JourneyFinder.Filters;

public class PersistentCookieActionFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        var request = context.HttpContext.Request;
        var response = context.HttpContext.Response;

        EnsurePersistentCookie(request, response, "SessionId");
        EnsurePersistentCookie(request, response, "DeviceId");
    }

    private void EnsurePersistentCookie(HttpRequest request, HttpResponse response, string cookieName)
    {
        if (!request.Cookies.ContainsKey(cookieName))
        {
            var newValue = Guid.NewGuid().ToString();

            var cookieOptions = new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddDays(30),
                HttpOnly = true,
                IsEssential = true,
                Secure = true, 
                SameSite = SameSiteMode.Strict
            };

            response.Cookies.Append(cookieName, newValue, cookieOptions);
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
    }
}