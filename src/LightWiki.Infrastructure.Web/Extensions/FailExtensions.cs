using System;
using System.Linq;
using System.Net;
using System.Net.Mime;
using LightWiki.Infrastructure.Models;
using Microsoft.AspNetCore.Mvc;

namespace LightWiki.Infrastructure.Web.Extensions;

public static class FailExtensions
{
    public static IActionResult ToActionResult(this Fail fail)
    {
        switch (fail.FailCode)
        {
            case FailCode.BadRequest:
                return new BadRequestObjectResult(fail.Errors);

            case FailCode.NotFound:
                return new NotFoundResult();

            case FailCode.Forbidden:
                return new ContentResult
                {
                    StatusCode = (int)HttpStatusCode.Forbidden,
                    Content = string.Join(", ", fail.Errors.SelectMany(err => err.Value)),
                    ContentType = MediaTypeNames.Text.Plain,
                };

            case FailCode.Unauthorized:
                return new ContentResult()
                {
                    StatusCode = (int)HttpStatusCode.Unauthorized,
                    Content = string.Join(", ", fail.Errors.SelectMany(err => err.Value)),
                    ContentType = MediaTypeNames.Text.Plain,
                };

            default:
                throw new ArgumentOutOfRangeException(nameof(fail), "Invalid fail code");
        }
    }
}