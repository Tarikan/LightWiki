using FluentValidation;
using LightWiki.Data;
using LightWiki.Domain.Enums;
using LightWiki.Features.Groups.Requests;
using LightWiki.Infrastructure.Auth;
using LightWiki.Infrastructure.Models;
using LightWiki.Infrastructure.Validators;
using LightWiki.Shared.Validation;

namespace LightWiki.Features.Groups.Validators;

public class RemoveGroupValidator : AbstractValidator<RemoveGroup>
{
    public RemoveGroupValidator(WikiContext context, IAuthorizedUserProvider authorizedUserProvider)
    {
        RuleFor(r => r.Id)
            .Cascade(CascadeMode.Stop)
            .EntityShouldExist(context.Groups)
            .WithErrorCode(FailCode.BadRequest.ToString())
            .UserShouldHaveAccessToGroup(context.Groups, authorizedUserProvider, GroupAccessRule.RemoveGroup)
            .WithErrorCode(FailCode.Forbidden.ToString());
    }
}