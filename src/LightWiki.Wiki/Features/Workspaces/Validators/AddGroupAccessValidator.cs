﻿using FluentValidation;
using LightWiki.Data;
using LightWiki.Domain.Enums;
using LightWiki.Features.Workspaces.Requests;
using LightWiki.Infrastructure.Auth;
using LightWiki.Infrastructure.Configuration;
using LightWiki.Infrastructure.Models;
using LightWiki.Infrastructure.Validators;
using LightWiki.Shared.Validation;

namespace LightWiki.Features.Workspaces.Validators;

public class AddGroupAccessValidator : AbstractValidator<AddWorkspaceGroupAccess>
{
    public AddGroupAccessValidator(
        WikiContext wikiContext,
        IAuthorizedUserProvider authorizedUserProvider,
        AppConfiguration appConfiguration)
    {
        RuleFor(r => r.WorkspaceId)
            .Cascade(CascadeMode.Stop)
            .EntityShouldExist(wikiContext.Workspaces)
            .WithErrorCode(FailCode.BadRequest.ToString())
            .UserShouldHaveAccessToWorkspace(
                wikiContext.Workspaces,
                authorizedUserProvider,
                WorkspaceAccessRule.ManageWorkspace,
                appConfiguration.AllowUnauthorizedUse)
            .WithErrorCode(FailCode.Forbidden.ToString());

        RuleFor(r => r.GroupId)
            .EntityShouldExist(wikiContext.Groups)
            .WithErrorCode(FailCode.BadRequest.ToString());
    }
}