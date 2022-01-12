using System;
using System.Collections.Generic;
using LightWiki.Infrastructure.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace LightWiki.Infrastructure.Web.Swagger;

public static class SwaggerExtensions
{
    public static SwaggerGenOptions ConfigureJwt(this SwaggerGenOptions swaggerGenOptions)
    {
        swaggerGenOptions.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n 
                      Enter 'Bearer' [space] and then your token in the text input below.
                      \r\n\r\nExample: 'Bearer 12345abcdef'",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer",
        });
        swaggerGenOptions.AddSecurityRequirement(new OpenApiSecurityRequirement()
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer",
                    },
                    Scheme = "oauth2",
                    Name = "Bearer",
                    In = ParameterLocation.Header,
                },
                new List<string>()
            },
        });

        return swaggerGenOptions;
    }

    public static SwaggerGenOptions ConfigureCognitoAuth(
        this SwaggerGenOptions swaggerGenOptions,
        OAuthConfiguration settings)
    {
        swaggerGenOptions.AddSecurityDefinition("Cognito", new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.OAuth2,
            Extensions = new Dictionary<string, IOpenApiExtension>()
            {
                { "x-tokenName", new OpenApiString("id_token") },
            },
            Flows = new OpenApiOAuthFlows
            {
                AuthorizationCode = new OpenApiOAuthFlow
                {
                    AuthorizationUrl =
                        new Uri($"{settings.AuthServerDomain}/oauth2/authorize"),
                    TokenUrl = new Uri($"{settings.AuthServerDomain}/oauth2/token"),
                    Scopes = new Dictionary<string, string>
                    {
                        { "openid", "openid" },
                        { "profile", "profile" },
                        { "email", "email" },
                        { "phone", "phone" },
                    },
                },
            },
        });

        swaggerGenOptions.AddSecurityRequirement(new OpenApiSecurityRequirement()
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Cognito",
                    },
                    Scheme = "oauth2",
                    Name = "Bearer",
                    In = ParameterLocation.Header,
                },
                new List<string>()
            },
        });

        return swaggerGenOptions;
    }
}