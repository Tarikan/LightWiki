using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using LightWiki.Data;
using LightWiki.Features.Groups.Requests;
using LightWiki.Features.Users.Responses.Models;
using LightWiki.Infrastructure.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;

namespace LightWiki.Features.Groups.Handlers;

public class GetGroupMembersHandler : IRequestHandler<GetGroupMembers, OneOf<CollectionResult<UserModel>, Fail>>
{
    private readonly WikiContext _wikiContext;
    private readonly IMapper _mapper;

    public GetGroupMembersHandler(WikiContext wikiContext, IMapper mapper)
    {
        _wikiContext = wikiContext;
        _mapper = mapper;
    }

    public async Task<OneOf<CollectionResult<UserModel>, Fail>> Handle(GetGroupMembers request, CancellationToken cancellationToken)
    {
        var group = await _wikiContext.Groups
            .Include(g => g.Users)
            .SingleAsync(g => g.Id == request.GroupId, cancellationToken);

        var models = _mapper.Map<IEnumerable<UserModel>>(group.Users).ToList();

        return new CollectionResult<UserModel>(models, models.Count);
    }
}