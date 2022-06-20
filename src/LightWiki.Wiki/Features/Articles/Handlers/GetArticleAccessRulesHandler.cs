using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using LightWiki.Data;
using LightWiki.Domain.Enums;
using LightWiki.Features.Articles.Requests;
using LightWiki.Features.Articles.Responses.Models;
using LightWiki.Features.Users.Responses.Models;
using LightWiki.Infrastructure.Models;
using LightWiki.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;
using Sieve.Services;

namespace LightWiki.Features.Articles.Handlers;

public class GetArticleAccessRulesHandler : IRequestHandler<GetArticleAccessRules,
    OneOf<CollectionResult<ArticleAccessRuleModel>, Fail>>
{
    private readonly WikiContext _wikiContext;
    private readonly ISieveProcessor _sieveProcessor;
    private readonly IMapper _mapper;

    public GetArticleAccessRulesHandler(WikiContext wikiContext, ISieveProcessor sieveProcessor, IMapper mapper)
    {
        _wikiContext = wikiContext;
        _sieveProcessor = sieveProcessor;
        _mapper = mapper;
    }

    public async Task<OneOf<CollectionResult<ArticleAccessRuleModel>, Fail>> Handle(
        GetArticleAccessRules request,
        CancellationToken cancellationToken)
    {
        var accessesRequest = _wikiContext.ArticleAccesses
            .Include(a => a.Party)
                .ThenInclude(p => p.Users)
            .Include(a => a.Party)
                .ThenInclude(p => p.Groups)
            .Where(a => a.ArticleId == request.ArticleId);

        accessesRequest = _sieveProcessor.Apply(request, accessesRequest);

        var accesses = await accessesRequest.ToListAsync(cancellationToken: cancellationToken);

        var resultList = new List<ArticleAccessRuleModel>(accesses.Count);

        foreach (var access in accesses)
        {
            var group = access.Party.Groups.FirstOrDefault();

            if (group?.GroupType is GroupType.Admin)
            {
                continue;
            }

            var articleAccessRuleModel = new ArticleAccessRuleModel
            {
                RuleId = access.Id,
                PartyType = access.Party.PartyType,
                User = access.Party.PartyType is PartyType.User
                    ? _mapper.Map<UserModel>(access.Party.Users.First())
                    : null,
                Group = access.Party.PartyType is PartyType.Group
                    ? new GroupModel
                    {
                        Id = group.Id,
                        Name = group.Name,
                    }
                    : null,
                ArticleAccessRule = access.ArticleAccessRule,
            };

            resultList.Add(articleAccessRuleModel);
        }

        return new CollectionResult<ArticleAccessRuleModel>(resultList, resultList.Count);
    }
}