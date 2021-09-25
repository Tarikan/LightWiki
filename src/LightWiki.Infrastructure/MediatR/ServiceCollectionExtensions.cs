using LightWiki.Infrastructure.Models;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using OneOf;

namespace LightWiki.Infrastructure.MediatR
{
    public static class ServiceCollectionExtensions
    {
        public static FeatureRegistrator<TRequest, TSuccess> ForScoped<TRequest, TSuccess>(this IServiceCollection services)
            where TRequest : IRequest<OneOf<TSuccess, Fail>> =>
            new FeatureRegistrator<TRequest, TSuccess>(services);
    }
}