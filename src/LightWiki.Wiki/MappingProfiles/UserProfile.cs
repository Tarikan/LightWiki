using AutoMapper;
using LightWiki.Domain.Models;
using LightWiki.Features.Users.Requests;
using LightWiki.Features.Users.Responses.Models;

namespace LightWiki.MappingProfiles;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserModel>()
            .ForMember(dest => dest.Info, opts => opts.MapFrom(src => src.UserInfo))
            .ForMember(dest => dest.Avatar, opts => opts.Ignore());

        CreateMap<UpdateUserInfo, UserInfo>()
            .ForMember(dest => dest.Id, opts => opts.Ignore())
            .ForMember(dest => dest.User, opts => opts.Ignore())
            .ForMember(dest => dest.UserId, opts => opts.Ignore());

        CreateMap<UserInfo, UserInfoModel>();
    }
}