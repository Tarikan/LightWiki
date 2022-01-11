using System;
using System.Net;
using LightWiki.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LightWiki.Infrastructure.Web.Extensions;

public static class AppBuilderExtensions
{
    public static IApplicationBuilder UseHealthCheck(
        this IApplicationBuilder app,
        string endpoint = "/ping",
        string response = "ok")
    {
        app.Map(endpoint, ping => ping.Run(context => context.Response.WriteAsync(response)));

        return app;
    }

    public static IApplicationBuilder UseExceptionInterception(
        this IApplicationBuilder app,
        IHostEnvironment hostEnvironment)
    {
        if (hostEnvironment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseGlobalExceptionHandling();
        }

        return app;
    }

    private static void UseGlobalExceptionHandling(this IApplicationBuilder app)
    {
        app.UseExceptionHandler(new ExceptionHandlerOptions
        {
            ExceptionHandler = async context =>
            {
                var exceptionFeature = context.Features.Get<IExceptionHandlerFeature>();
                var routeData = context.GetRouteData() ?? new RouteData();

                var actionContext = new ActionContext(context, routeData, new ActionDescriptor());
                var result = BuildResult(exceptionFeature.Error);

                var executor = context.RequestServices.GetService<IActionResultExecutor<ObjectResult>>();

                await executor.ExecuteAsync(actionContext, result);
            },
        });
    }

    private static ObjectResult BuildResult(Exception exception)
    {
        return exception switch
        {
            UnauthorizedException => new ObjectResult(new
            {
                Message = "Error processing request. Unauthorized.",
            })
            {
                StatusCode = (int)HttpStatusCode.Unauthorized,
            },
            BadRequestException => new ObjectResult(new
            {
                Message = "Error processing request. Bad request.",
            })
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
            },
            _ => new ObjectResult(new
            {
                Message = "Error processing request. Server error.",
            })
            {
                StatusCode = (int)HttpStatusCode.InternalServerError,
            },
        };
    }
}