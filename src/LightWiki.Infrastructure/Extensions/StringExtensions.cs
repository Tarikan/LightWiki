using System.Text.RegularExpressions;
using Newtonsoft.Json.Serialization;

namespace LightWiki.Infrastructure.Extensions;

public static class StringExtensions
{
    private static readonly CamelCaseNamingStrategy CamelCaseStrategy = new CamelCaseNamingStrategy();

    public static string ToCamelCase(this string value) => CamelCaseStrategy.GetPropertyName(value, false);

#pragma warning disable CA1055
    public static string ToUrlFriendlyString(this string value)
#pragma warning restore CA1055
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        return Regex.Replace(value.Trim(), @"[^A-Za-z0-9_\.~]+", "-");
    }
}