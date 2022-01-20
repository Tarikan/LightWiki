using FluentValidation;
using LightWiki.Data;
using LightWiki.Domain.Enums;
using LightWiki.Features.Workspaces.Requests;
using LightWiki.Infrastructure.Auth;
using LightWiki.Infrastructure.Configuration;
using LightWiki.Infrastructure.Models;
using LightWiki.Infrastructure.Validators;
using LightWiki.Shared.Validation;

namespace LightWiki.Features.Workspaces.Validators;

public class GetWorkspaceInfoValidator : AbstractValidator<GetWorkspaceInfo>
{
    public GetWorkspaceInfoValidator(
        WikiContext wikiContext,
        IAuthorizedUserProvider authorizedUserProvider,
        AppConfiguration appConfiguration)
    {
        RuleFor(r => r.WorkspaceId)
            .Cascade(CascadeMode.Stop)
            .EntityShouldExist(wikiContext.Workspaces)
            .UserShouldHaveAccessToWorkspace(
                wikiContext.Workspaces,
                authorizedUserProvider,
                WorkspaceAccessRule.Browse,
                appConfiguration.AllowUnauthorizedUse)
            .WithErrorCode(FailCode.Forbidden.ToString());
    }
}