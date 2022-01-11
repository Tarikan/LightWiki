using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Serialization;

namespace LightWiki.Infrastructure.Models;

public class Fail
{
    private const string GeneralErrorsKey = "generalErrors";
    private static CamelCaseNamingStrategy ErrorKeysStrategy = new CamelCaseNamingStrategy();

    public Dictionary<string, string[]> Errors { get; }

    public FailCode FailCode { get; set; }

    public Fail(Dictionary<string, string[]> errors, FailCode failCode)
    {
        FailCode = failCode;
        Errors = errors;
    }

    public Fail(IEnumerable<string> errors, FailCode failCode)
        : this(new Dictionary<string, string[]> { { GeneralErrorsKey, errors.ToArray() } }, failCode)
    {
    }

    public Fail(ILookup<string, string> errors, FailCode failCode)
        : this(
            errors.ToDictionary(e => ErrorKeysStrategy.GetPropertyName(e.Key, false), e => e.ToArray()),
            failCode)
    {
    }

    public Fail(string message, FailCode failCode)
        : this(new Dictionary<string, string[]> { { GeneralErrorsKey, new[] { message } } }, failCode)
    {
    }
}