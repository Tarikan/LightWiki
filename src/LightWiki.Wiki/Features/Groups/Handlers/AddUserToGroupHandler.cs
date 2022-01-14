using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LightWiki.Data;
using LightWiki.Domain.Models;
using LightWiki.Features.Groups.Requests;
using LightWiki.Infrastructure.Models;
using MediatR;
using OneOf;

namespace LightWiki.Features.Groups.Handlers;

public class AddUserToGroupHandler : IRequestHandler<AddUserToGroup, OneOf<Success, Fail>>
{
    private readonly WikiContext _context;

    public AddUserToGroupHandler(WikiContext context)
    {
        _context = context;
    }

    public async Task<OneOf<Success, Fail>> Handle(AddUserToGroup request, CancellationToken cancellationToken)
    {
        var group = await _context.Groups.FindAsync(request.GroupId);
        var user = await _context.Users.FindAsync(request.UserId);

        group.Users ??= new List<User>();

        group.Users.Add(user);

        await _context.SaveChangesAsync(cancellationToken);

        return new Success();
    }
}