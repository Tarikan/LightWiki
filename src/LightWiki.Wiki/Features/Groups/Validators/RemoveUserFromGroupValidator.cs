using System.Linq;
using FluentValidation;
using LightWiki.Data;
using LightWiki.Domain.Enums;
using LightWiki.Features.Groups.Requests;
using LightWiki.Infrastructure.Auth;
using LightWiki.Infrastructure.Models;
using LightWiki.Infrastructure.Validators;
using LightWiki.Shared.Validation;
using Microsoft.EntityFrameworkCore;

namespace LightWiki.Features.Groups.Validators;

public class RemoveUserFromGroupValidator : AbstractValidator<RemoveUserFromGroup>
{
    public RemoveUserFromGroupValidator(WikiContext context, IAuthorizedUserProvider authorizedUserProvider)
    {
        RuleFor(r => r.GroupId)
            .Cascade(CascadeMode.Stop)
            .EntityShouldExist(context.Groups)
            .WithErrorCode(FailCode.BadRequest.ToString())
            .UserShouldHaveAccessToGroup(context.Groups, authorizedUserProvider, GroupAccessRule.RemoveMembers)
            .WithErrorCode(FailCode.Forbidden.ToString());

        RuleFor(r => r.UserId).EntityShouldExist(context.Users);

        RuleFor(r => r)
            .CustomAsync(async (request, ctx, _) =>
            {
                var group = await context.Groups
                    .Include(g => g.Users
                        .Where(u => u.Id == request.UserId))
                    .SingleOrDefaultAsync(g => g.Id == request.GroupId);

                if (group is null)
                {
                    return;
                }

                if (!group.Users.Any())
                {
                    ctx.AddFailure("User not in group");
                }
            });
    }
}