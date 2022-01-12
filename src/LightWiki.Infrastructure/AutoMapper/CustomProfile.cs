using AutoMapper;
using LightWiki.Infrastructure.Models;

namespace LightWiki.Infrastructure.AutoMapper;

public class CustomProfile : Profile
{
    public CustomProfile()
    {
        CreateMap(typeof(CollectionResult<>), typeof(CollectionResult<>))
            .ConvertUsing(typeof(CollectionResultConverter<,>));
    }
}