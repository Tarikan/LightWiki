using System.Threading;
using System.Threading.Tasks;
using LightWiki.Data;
using LightWiki.Domain.Models;
using LightWiki.Features.Groups.Requests;
using LightWiki.Infrastructure.Auth;
using LightWiki.Infrastructure.Models;
using MediatR;
using OneOf;

namespace LightWiki.Features.Groups.Handlers;

public class CreateGroupHandler : IRequestHandler<CreateGroup, OneOf<Success, Fail>>
{
    private readonly WikiContext _wikiContext;
    private readonly IAuthorizedUserProvider _authorizedUserProvider;

    public CreateGroupHandler(WikiContext wikiContext, IAuthorizedUserProvider authorizedUserProvider)
    {
        _wikiContext = wikiContext;
        _authorizedUserProvider = authorizedUserProvider;
    }

    public async Task<OneOf<Success, Fail>> Handle(CreateGroup request, CancellationToken cancellationToken)
    {
        var userContext = await _authorizedUserProvider.GetUser();
        var user = await _wikiContext.Users.FindAsync(userContext.Id);

        var group = new Group
        {
            Name = request.GroupName,
        };

        group.Users.Add(user);

        await _wikiContext.Groups.AddAsync(group, cancellationToken);

        await _wikiContext.SaveChangesAsync(cancellationToken);

        return new Success();
    }
}