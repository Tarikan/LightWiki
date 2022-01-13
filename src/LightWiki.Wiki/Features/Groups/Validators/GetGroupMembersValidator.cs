using FluentValidation;
using LightWiki.Data;
using LightWiki.Features.Groups.Requests;
using LightWiki.Infrastructure.Validators;

namespace LightWiki.Features.Groups.Validators;

public class GetGroupMembersValidator : AbstractValidator<GetGroupMembers>
{
    public GetGroupMembersValidator(WikiContext context)
    {
        RuleFor(r => r.GroupId).EntityShouldExist(context.Groups);
    }
}