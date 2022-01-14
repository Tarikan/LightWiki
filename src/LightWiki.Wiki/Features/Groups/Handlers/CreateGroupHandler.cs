using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LightWiki.Data;
using LightWiki.Domain.Enums;
using LightWiki.Domain.Models;
using LightWiki.Features.Groups.Requests;
using LightWiki.Infrastructure.Auth;
using LightWiki.Infrastructure.Models;
using MediatR;
using OneOf;

namespace LightWiki.Features.Groups.Handlers;

public class CreateGroupHandler : IRequestHandler<CreateGroup, OneOf<SuccessWithId<int>, Fail>>
{
    private readonly WikiContext _wikiContext;
    private readonly IAuthorizedUserProvider _authorizedUserProvider;

    public CreateGroupHandler(WikiContext wikiContext, IAuthorizedUserProvider authorizedUserProvider)
    {
        _wikiContext = wikiContext;
        _authorizedUserProvider = authorizedUserProvider;
    }

    public async Task<OneOf<SuccessWithId<int>, Fail>> Handle(CreateGroup request, CancellationToken cancellationToken)
    {
        var userContext = await _authorizedUserProvider.GetUser();
        var user = await _wikiContext.Users.FindAsync(userContext.Id);

        var group = new Group
        {
            Name = request.GroupName,
            Users = new List<User>
            {
                user,
            },
            GroupPersonalAccessRules = new List<GroupPersonalAccessRule>
            {
                new GroupPersonalAccessRule
                {
                    User = user,
                    AccessRule = GroupAccessRule.All,
                },
            },
        };

        await _wikiContext.Groups.AddAsync(group, cancellationToken);

        await _wikiContext.SaveChangesAsync(cancellationToken);

        return new SuccessWithId<int>(group.Id);
    }
}