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

public class UpdateWorkspaceValidator : AbstractValidator<UpdateWorkspace>
{
    public UpdateWorkspaceValidator(WikiContext wikiContext, IAuthorizedUserProvider authorizedUserProvider)
    {
        RuleFor(r => r.Id)
            .Cascade(CascadeMode.Stop)
            .EntityShouldExist(wikiContext.Workspaces)
            .UserShouldHaveAccessToWorkspace(
                wikiContext.Workspaces,
                authorizedUserProvider,
                WorkspaceAccessRule.ManageWorkspace)
            .WithErrorCode(FailCode.Forbidden.ToString());

        RuleFor(r => r.Name)
            .CustomAsync(async (name, ctx, _) =>
            {
                if (await wikiContext.Workspaces.AnyAsync(w => w.Name == name))
                {
                    ctx.AddFailure("Workspace with suggested name already exists");
                }
            });
    }
}