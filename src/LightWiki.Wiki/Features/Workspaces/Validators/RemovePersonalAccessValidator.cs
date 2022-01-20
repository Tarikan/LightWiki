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

public class RemovePersonalAccessValidator : AbstractValidator<RemoveWorkspacePersonalAccess>
{
    public RemovePersonalAccessValidator(WikiContext wikiContext, IAuthorizedUserProvider authorizedUserProvider)
    {
        RuleFor(r => r.UserId)
            .EntityShouldExist(wikiContext.Users)
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
                if (!await wikiContext.WorkspacePersonalAccessRules
                        .AnyAsync(rule => rule.UserId == r.UserId ||
                                          rule.WorkspaceId == r.WorkspaceId))
                {
                    ctx.AddFailure("Rule not found");
                }
            });
    }
}