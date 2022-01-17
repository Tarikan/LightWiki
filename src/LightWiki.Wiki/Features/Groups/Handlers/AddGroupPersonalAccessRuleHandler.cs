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

public class AddGroupPersonalAccessRuleHandler : IRequestHandler<AddGroupPersonalAccessRule, OneOf<Success, Fail>>
{
    private readonly WikiContext _wikiContext;

    public AddGroupPersonalAccessRuleHandler(WikiContext wikiContext)
    {
        _wikiContext = wikiContext;
    }

    public async Task<OneOf<Success, Fail>> Handle(AddGroupPersonalAccessRule request, CancellationToken cancellationToken)
    {
        var rule = await _wikiContext.GroupPersonalAccessRules
            .SingleOrDefaultAsync(
                gpar => gpar.UserId == request.UserId &&
                        gpar.GroupId == request.GroupId,
                cancellationToken);

        rule ??= new GroupPersonalAccessRule
        {
            UserId = request.UserId,
            GroupId = request.GroupId,
        };

        rule.AccessRule = request.GroupAccessRule;
        _wikiContext.GroupPersonalAccessRules.Update(rule);
        await _wikiContext.SaveChangesAsync(cancellationToken);

        return new Success();
    }
}