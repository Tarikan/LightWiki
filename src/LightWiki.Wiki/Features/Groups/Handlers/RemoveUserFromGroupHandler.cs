using System.Threading;
using System.Threading.Tasks;
using LightWiki.Data;
using LightWiki.Features.Groups.Requests;
using LightWiki.Infrastructure.Models;
using MediatR;
using OneOf;

namespace LightWiki.Features.Groups.Handlers;

public class RemoveUserFromGroupHandler : IRequestHandler<RemoveUserFromGroup, OneOf<Success, Fail>>
{
    private readonly WikiContext _context;

    public RemoveUserFromGroupHandler(WikiContext context)
    {
        _context = context;
    }

    public async Task<OneOf<Success, Fail>> Handle(RemoveUserFromGroup request, CancellationToken cancellationToken)
    {
        var group = await _context.Groups.FindAsync(request.GroupId);
        var user = await _context.Users.FindAsync(request.UserId);

        group.Users.Remove(user);

        await _context.SaveChangesAsync(cancellationToken);

        return new Success();
    }
}