using FluentValidation;
using LightWiki.Data;
using LightWiki.Domain.Enums;
using LightWiki.Features.Workspaces.Requests;
using LightWiki.Infrastructure.Auth;
using LightWiki.Infrastructure.Models;
using LightWiki.Infrastructure.Validators;
using LightWiki.Shared.Validation;

namespace LightWiki.Features.Workspaces.Validators;

public class GetWorkspaceTreeValidator : AbstractValidator<GetWorkspaceTree>
{
    public GetWorkspaceTreeValidator(
        WikiContext wikiContext,
        IAuthorizedUserProvider authorizedUserProvider)
    {
        RuleFor(r => r.WorkspaceId)
            .Cascade(CascadeMode.Stop)
            .EntityShouldExist(wikiContext.Workspaces)
            .WithErrorCode(FailCode.BadRequest.ToString())
            .UserShouldHaveAccessToWorkspace(
                wikiContext.Workspaces,
                authorizedUserProvider,
                WorkspaceAccessRule.Browse)
            .WithErrorCode(FailCode.Forbidden.ToString());
    }
}