using FluentValidation;
using LightWiki.Data;
using LightWiki.Domain.Enums;
using LightWiki.Features.Workspaces.Requests;
using LightWiki.Infrastructure.Auth;
using LightWiki.Infrastructure.Models;
using LightWiki.Infrastructure.Validators;
using LightWiki.Shared.Validation;
using Microsoft.EntityFrameworkCore;

namespace LightWiki.Features.Workspaces.Validators;

public class RemoveGroupAccessValidator : AbstractValidator<RemoveWorkspaceGroupAccess>
{
    public RemoveGroupAccessValidator(WikiContext wikiContext, IAuthorizedUserProvider authorizedUserProvider)
    {
        RuleFor(r => r.GroupId)
            .EntityShouldExist(wikiContext.Groups)
            .WithErrorCode(FailCode.BadRequest.ToString());

        RuleFor(r => r.WorkspaceId)
            .Cascade(CascadeMode.Stop)
            .EntityShouldExist(wikiContext.Workspaces)
            .WithErrorCode(FailCode.BadRequest.ToString())
            .UserShouldHaveAccessToWorkspace(
                wikiContext.Workspaces,
                authorizedUserProvider,
                WorkspaceAccessRule.ManageWorkspace);

        RuleFor(r => r)
            .CustomAsync(async (r, ctx, _) =>
            {
                if (!await wikiContext.WorkspaceGroupAccessRules
                        .AnyAsync(rule => rule.GroupId == r.GroupId ||
                                          rule.WorkspaceId == r.WorkspaceId))
                {
                    ctx.AddFailure("Rule not found");
                }
            });
    }
}