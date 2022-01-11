using FluentValidation;
using LightWiki.Infrastructure.Models;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using OneOf;

namespace LightWiki.Infrastructure.MediatR;

public class FeatureRegistrator<TRequest, TResult> where TRequest : IRequest<OneOf<TResult, Fail>>
{
    private readonly IServiceCollection _services;

    public FeatureRegistrator(IServiceCollection services)
    {
        _services = services;
    }

    public FeatureRegistrator<TRequest, TResult> WithValidation<TValidator>()
        where TValidator : class, IValidator<TRequest>
    {
        _services.AddScoped<IValidator<TRequest>, TValidator>();
        _services.AddScoped<IPipelineBehavior<TRequest, OneOf<TResult, Fail>>>(p =>
            new ValidationPipelineBehaviour<TRequest, TResult>(p.GetService<IValidator<TRequest>>()));

        return this;
    }

    public FeatureRegistrator<TRequest, TResult> AddHandler<THandler>()
        where THandler : class, IRequestHandler<TRequest, OneOf<TResult, Fail>>
    {
        _services.AddScoped<IRequestHandler<TRequest, OneOf<TResult, Fail>>, THandler>();

        return this;
    }
}