using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using LightWiki.Infrastructure.Models;
using MediatR;
using OneOf;

namespace LightWiki.Infrastructure.MediatR;

public class ValidationPipelineBehaviour<TRequest, TResult> : IPipelineBehavior<TRequest, OneOf<TResult, Fail>>
    where TRequest : IRequest<OneOf<TResult, Fail>>
{
    private readonly IValidator<TRequest> _validator;

    public ValidationPipelineBehaviour(IValidator<TRequest> validator)
    {
        _validator = validator;
    }

    public async Task<OneOf<TResult, Fail>> Handle(
        TRequest request,
        CancellationToken cancellationToken,
        RequestHandlerDelegate<OneOf<TResult, Fail>> next)
    {
        var result = await _validator.ValidateAsync(request, cancellationToken);

        if (!result.IsValid)
        {
            var errors =
                result.Errors.ToLookup(
                    f => string.IsNullOrEmpty(f.PropertyName) ? "validationErrors" : f.PropertyName,
                    f => f.ErrorMessage);
            return new Fail(
                errors,
                FailCode.BadRequest);
        }

        return await next();
    }
}