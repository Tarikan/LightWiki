using FluentValidation;
using LightWiki.Data;
using LightWiki.Features.Groups.Requests;
using LightWiki.Infrastructure.Validators;

namespace LightWiki.Features.Groups.Validators;

public class RemoveGroupValidator : AbstractValidator<RemoveGroup>
{
    public RemoveGroupValidator(WikiContext context)
    {
        RuleFor(r => r.Id).EntityShouldExist(context.Groups);
    }
}