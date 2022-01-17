using System.Linq;
using FluentValidation;
using LightWiki.Data;
using LightWiki.Domain.Enums;
using LightWiki.Features.Groups.Requests;
using LightWiki.Infrastructure.Auth;
using LightWiki.Infrastructure.Models;
using LightWiki.Infrastructure.Validators;
using LightWiki.Shared.Validation;
using Microsoft.EntityFrameworkCore;

namespace LightWiki.Features.Groups.Validators;

public class RemoveGroupPersonalAccessRuleValidator : AbstractValidator<RemoveGroupPersonalAccessRule>
{
    public RemoveGroupPersonalAccessRuleValidator(
        WikiContext wikiContext,
        IAuthorizedUserProvider authorizedUserProvider)
    {
        RuleFor(r => r.GroupId)
            .Cascade(CascadeMode.Stop)
            .EntityShouldExist(wikiContext.Groups)
            .WithErrorCode(FailCode.Forbidden.ToString())
            .UserShouldHaveAccessToGroup(wikiContext.Groups, authorizedUserProvider, GroupAccessRule.ModifyGroup);

        RuleFor(r => r.UserId)
            .EntityShouldExist(wikiContext.Users);

        RuleFor(r => r)
            .CustomAsync(async (r, ctx, _) =>
            {
                var isAccessRuleExist = await wikiContext.GroupPersonalAccessRules
                    .AnyAsync(gpar => gpar.UserId == r.UserId &&
                                      gpar.GroupId == r.GroupId);

                if (!isAccessRuleExist)
                {
                    ctx.AddFailure("Access rule does not exists");
                }
            });
    }
}