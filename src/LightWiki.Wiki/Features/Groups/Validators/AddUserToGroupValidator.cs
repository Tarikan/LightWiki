﻿using FluentValidation;
using LightWiki.Data;
using LightWiki.Features.Groups.Requests;
using LightWiki.Infrastructure.Validators;

namespace LightWiki.Features.Groups.Validators;

public class AddUserToGroupValidator : AbstractValidator<AddUserToGroup>
{
    public AddUserToGroupValidator(WikiContext context)
    {
        RuleFor(r => r.GroupId).EntityShouldExist(context.Groups);

        RuleFor(r => r.UserId).EntityShouldExist(context.Users);
    }
}