using LightWiki.Infrastructure.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace LightWiki.Infrastructure.Web.Authentication;

public sealed class ConfigurableAuthorizeAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var config = context.HttpContext.RequestServices.GetService<AppConfiguration>();

        if (!config.AllowUnauthorizedUse && !context.HttpContext.User.Identity.IsAuthenticated)
        {
            context.Result = new UnauthorizedResult();
        }
    }
}