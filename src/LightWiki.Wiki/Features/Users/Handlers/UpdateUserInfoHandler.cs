using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using LightWiki.Data;
using LightWiki.Domain.Models;
using LightWiki.Features.Users.Requests;
using LightWiki.Infrastructure.Auth;
using LightWiki.Infrastructure.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;

namespace LightWiki.Features.Users.Handlers;

public class UpdateUserInfoHandler : IRequestHandler<UpdateUserInfo, OneOf<Success, Fail>>
{
    private readonly WikiContext _wikiContext;
    private readonly IMapper _mapper;
    private readonly IAuthorizedUserProvider _authorizedUserProvider;

    public UpdateUserInfoHandler(
        WikiContext wikiContext,
        IMapper mapper,
        IAuthorizedUserProvider authorizedUserProvider)
    {
        _wikiContext = wikiContext;
        _mapper = mapper;
        _authorizedUserProvider = authorizedUserProvider;
    }

    public async Task<OneOf<Success, Fail>> Handle(UpdateUserInfo request, CancellationToken cancellationToken)
    {
        var userContext = await _authorizedUserProvider.GetUser();

        var info = await _wikiContext.Users
            .Where(u => u.Id == userContext.Id)
            .Select(u => u.UserInfo)
            .SingleOrDefaultAsync(cancellationToken) ?? new UserInfo()
        {
            UserId = userContext.Id,
        };

        _mapper.Map(request, info);

        if (info.DateOfBirth.HasValue)
        {
            info.DateOfBirth = DateTime.SpecifyKind(info.DateOfBirth.Value, DateTimeKind.Utc);
        }

        _wikiContext.UserInfos.Update(info);
        await _wikiContext.SaveChangesAsync(cancellationToken);

        return new Success();
    }
}