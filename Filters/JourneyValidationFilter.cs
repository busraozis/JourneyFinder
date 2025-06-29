namespace JourneyFinder.Filters;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class JourneyValidationFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (context.ActionArguments.TryGetValue("originId", out var originIdObj) &&
            context.ActionArguments.TryGetValue("destinationId", out var destinationIdObj))
        {
            int originId = (int)originIdObj;
            int destinationId = (int)destinationIdObj;

            if (originId <= 0 || destinationId <= 0)
            {
                context.Result = new BadRequestObjectResult("OriginId ve DestinationId pozitif olmalıdır.");
                return;
            }
        }

        if (context.ActionArguments.TryGetValue("departureDate", out var departureDateObj))
        {
            DateTime departureDate = (DateTime) departureDateObj;
            if (departureDate < DateTime.Today)
            {
                context.Result = new BadRequestObjectResult("Geçmiş bir tarih seçilemez.");
            }
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
    }
}
