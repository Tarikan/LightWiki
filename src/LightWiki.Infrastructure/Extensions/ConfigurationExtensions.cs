using System;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace LightWiki.Infrastructure.Extensions
{
    public static class ConfigurationExtensions
    {
        public static IConfigurationBuilder AddSettingsSources(this IConfigurationBuilder configBuilder)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var binDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var settingsPath = Path.Combine(binDir!, "appsettings.json");
            var envSettingPath = Path.Combine(binDir!, $"appsettings.{environment}.json");

            configBuilder
                .AddJsonFile(settingsPath, optional: false, reloadOnChange: true)
                .AddJsonFile(envSettingPath, optional: true, reloadOnChange: true);

            return configBuilder;
        }
    }
}