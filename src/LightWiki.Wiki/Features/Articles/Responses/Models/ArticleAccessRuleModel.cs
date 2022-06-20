using LightWiki.Domain.Enums;
using LightWiki.Features.Users.Responses.Models;
using LightWiki.Shared.Models;
using Newtonsoft.Json;

namespace LightWiki.Features.Articles.Responses.Models;

public class ArticleAccessRuleModel
{
    public int RuleId { get; set; }

    public PartyType PartyType { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public UserModel User { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public GroupModel Group { get; set; }

    public ArticleAccessRule ArticleAccessRule { get; set; }
}