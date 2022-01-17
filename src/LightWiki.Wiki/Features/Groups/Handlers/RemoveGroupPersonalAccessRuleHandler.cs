using System.Threading;
using System.Threading.Tasks;
using LightWiki.Data;
using LightWiki.Domain.Enums;
using LightWiki.Domain.Models;
using LightWiki.Features.Groups.Requests;
using LightWiki.Infrastructure.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;

namespace LightWiki.Features.Groups.Handlers;

public class RemoveGroupPersonalAccessRuleHandler : IRequestHandler<RemoveGroupPersonalAccessRule, OneOf<Success, Fail>>
{
    private readonly WikiContext _wikiContext;

    public RemoveGroupPersonalAccessRuleHandler(WikiContext wikiContext)
    {
        _wikiContext = wikiContext;
    }

    public async Task<OneOf<Success, Fail>> Handle(
        RemoveGroupPersonalAccessRule request,
        CancellationToken cancellationToken)
    {
        var rule = await _wikiContext.GroupPersonalAccessRules
            .SingleAsync(
                gpar => gpar.UserId == request.UserId &&
                        gpar.GroupId == request.GroupId,
                cancellationToken);

        _wikiContext.GroupPersonalAccessRules.Remove(rule);
        await _wikiContext.SaveChangesAsync(cancellationToken);

        return new Success();
    }
}