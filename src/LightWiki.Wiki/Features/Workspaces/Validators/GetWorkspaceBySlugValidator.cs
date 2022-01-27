using FluentValidation;
using LightWiki.Features.Workspaces.Requests;

namespace LightWiki.Features.Workspaces.Validators;

public class GetWorkspaceBySlugValidator : AbstractValidator<GetWorkspaceBySlug>
{
    public GetWorkspaceBySlugValidator()
    {
        RuleFor(r => r.Slug)
            .NotEmpty();
    }
}