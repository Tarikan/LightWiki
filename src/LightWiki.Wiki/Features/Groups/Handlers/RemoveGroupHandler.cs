using System.Threading;
using System.Threading.Tasks;
using LightWiki.Data;
using LightWiki.Features.Groups.Requests;
using LightWiki.Infrastructure.Models;
using MediatR;
using OneOf;

namespace LightWiki.Features.Groups.Handlers;

public class RemoveGroupHandler : IRequestHandler<RemoveGroup, OneOf<Success, Fail>>
{
    private readonly WikiContext _wikiContext;

    public RemoveGroupHandler(WikiContext wikiContext)
    {
        _wikiContext = wikiContext;
    }

    public async Task<OneOf<Success, Fail>> Handle(RemoveGroup request, CancellationToken cancellationToken)
    {
        var group = await _wikiContext.Groups.FindAsync(request.Id);

        _wikiContext.Groups.Remove(group);

        await _wikiContext.SaveChangesAsync(cancellationToken);

        return new Success();
    }
}