using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using LightWiki.Data;
using LightWiki.Features.Users.Requests;
using LightWiki.Features.Users.Responses.Models;
using LightWiki.Infrastructure.Models;
using MediatR;
using OneOf;

namespace LightWiki.Features.Users.Handlers;

public class GetUserHandler : IRequestHandler<GetUser, OneOf<UserModel, Fail>>
{
    private readonly WikiContext _wikiContext;
    private readonly IMapper _mapper;

    public GetUserHandler(IMapper mapper, WikiContext wikiContext)
    {
        _mapper = mapper;
        _wikiContext = wikiContext;
    }

    public async Task<OneOf<UserModel, Fail>> Handle(GetUser request, CancellationToken cancellationToken)
    {
        var user = await _wikiContext.Users.FindAsync(request.UserId);

        return _mapper.Map<UserModel>(user);
    }
}