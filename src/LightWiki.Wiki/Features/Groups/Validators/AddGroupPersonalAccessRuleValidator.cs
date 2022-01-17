using FluentValidation;
using LightWiki.Data;
using LightWiki.Domain.Enums;
using LightWiki.Features.Groups.Requests;
using LightWiki.Infrastructure.Auth;
using LightWiki.Infrastructure.Models;
using LightWiki.Infrastructure.Validators;
using LightWiki.Shared.Validation;

namespace LightWiki.Features.Groups.Validators;

public class AddGroupPersonalAccessRuleValidator : AbstractValidator<AddGroupPersonalAccessRule>
{
    public AddGroupPersonalAccessRuleValidator(WikiContext wikiContext, IAuthorizedUserProvider authorizedUserProvider)
    {
        RuleFor(r => r.GroupId)
            .Cascade(CascadeMode.Stop)
            .EntityShouldExist(wikiContext.Groups)
            .WithErrorCode(FailCode.Forbidden.ToString())
            .UserShouldHaveAccessToGroup(wikiContext.Groups, authorizedUserProvider, GroupAccessRule.ModifyGroup);

        RuleFor(r => r.UserId)
            .EntityShouldExist(wikiContext.Users);
    }
}