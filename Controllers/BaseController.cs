using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace JourneyFinder.Controllers;

public class BaseController : Controller
{
    protected string UserLanguage
    {
        get
        {
            var cultureFeature = HttpContext.Features.Get<IRequestCultureFeature>();
            return cultureFeature?.RequestCulture.UICulture.Name ?? "tr-TR";
        }
    }
}