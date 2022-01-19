using FluentValidation;
using LightWiki.Data;
using LightWiki.Features.Workspaces.Requests;
using Microsoft.EntityFrameworkCore;

namespace LightWiki.Features.Workspaces.Validators;

public class CreateWorkspaceValidator : AbstractValidator<CreateWorkspace>
{
    public CreateWorkspaceValidator(WikiContext context)
    {
        RuleFor(r => r.WorkspaceName)
            .CustomAsync(async (name, ctx, _) =>
            {
                if (await context.Workspaces.AnyAsync(w => w.Name == name))
                {
                    ctx.AddFailure("Workspace with suggested name already exists");
                }
            });
    }
}