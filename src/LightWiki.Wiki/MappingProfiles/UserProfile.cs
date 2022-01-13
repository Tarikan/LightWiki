using AutoMapper;
using LightWiki.Domain.Models;
using LightWiki.Features.Users.Responses.Models;

namespace LightWiki.MappingProfiles;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserModel>();
    }
}