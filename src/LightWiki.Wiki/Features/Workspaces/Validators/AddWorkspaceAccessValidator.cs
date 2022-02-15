using FluentValidation;
using LightWiki.Data;
using LightWiki.Domain.Enums;
using LightWiki.Features.Workspaces.Handlers;
using LightWiki.Infrastructure.Auth;
using LightWiki.Infrastructure.Models;
using LightWiki.Infrastructure.Validators;
using LightWiki.Shared.Validation;

namespace LightWiki.Features.Workspaces.Validators;

public class AddWorkspaceAccessValidator : AbstractValidator<AddWorkspaceAccess>
{
    public AddWorkspaceAccessValidator(WikiContext wikiContext, IAuthorizedUserProvider authorizedUserProvider)
    {
        RuleFor(r => r.WorkspaceId)
            .Cascade(CascadeMode.Stop)
            .EntityShouldExist(wikiContext.Workspaces)
            .WithErrorCode(FailCode.BadRequest.ToString())
            .UserShouldHaveAccessToWorkspace(
                wikiContext.Workspaces,
                authorizedUserProvider,
                WorkspaceAccessRule.ManageWorkspace)
            .WithErrorCode(FailCode.Forbidden.ToString());

        RuleFor(r => r.PartyId)
            .EntityShouldExist(wikiContext.Parties)
            .WithErrorCode(FailCode.BadRequest.ToString());
    }
}