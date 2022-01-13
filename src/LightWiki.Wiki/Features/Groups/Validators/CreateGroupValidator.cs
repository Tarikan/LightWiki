using FluentValidation;
using LightWiki.Data;
using LightWiki.Features.Groups.Requests;
using Microsoft.EntityFrameworkCore;

namespace LightWiki.Features.Groups.Validators;

public class CreateGroupValidator : AbstractValidator<CreateGroup>
{
    public CreateGroupValidator(WikiContext context)
    {
        RuleFor(r => r.GroupName).CustomAsync(async (name, ctx, _) =>
        {
            if (await context.Groups.AnyAsync(g => g.Name == name))
            {
                ctx.AddFailure("Group with such name already exists");
            }
        });
    }
}