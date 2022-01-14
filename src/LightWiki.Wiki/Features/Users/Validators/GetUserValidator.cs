using FluentValidation;
using LightWiki.Data;
using LightWiki.Features.Users.Requests;
using LightWiki.Infrastructure.Validators;

namespace LightWiki.Features.Users.Validators;

public class GetUserValidator : AbstractValidator<GetUser>
{
    public GetUserValidator(WikiContext context)
    {
        RuleFor(r => r.UserId)
            .EntityShouldExist(context.Users);
    }
}