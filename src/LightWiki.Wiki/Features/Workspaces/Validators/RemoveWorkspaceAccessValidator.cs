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

public class RemoveWorkspaceAccessValidator : AbstractValidator<RemoveWorkspaceAccess>
{
    public RemoveWorkspaceAccessValidator(WikiContext wikiContext, IAuthorizedUserProvider authorizedUserProvider)
    {
        RuleFor(r => r.PartyId)
            .EntityShouldExist(wikiContext.Parties)
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
                if (!await wikiContext.WorkspaceAccesses
                        .AnyAsync(u => u.PartyId == r.PartyId &&
                                       u.WorkspaceId == r.WorkspaceId))
                {
                    ctx.AddFailure("Rule not found");
                }
            });
    }
}