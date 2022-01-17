﻿using FluentValidation;
using LightWiki.Data;
using LightWiki.Domain.Enums;
using LightWiki.Features.Articles.Requests;
using LightWiki.Infrastructure.Auth;
using LightWiki.Infrastructure.Models;
using LightWiki.Infrastructure.Validators;
using LightWiki.Shared.Validation;

namespace LightWiki.Features.Articles.Validators;

public class RemovePersonalAccessValidator : AbstractValidator<RemovePersonalAccess>
{
    public RemovePersonalAccessValidator(WikiContext wikiContext, IAuthorizedUserProvider authorizedUserProvider)
    {
        RuleFor(r => r.ArticleId)
            .EntityShouldExist(wikiContext.Articles)
            .WithErrorCode(FailCode.BadRequest.ToString())
            .UserShouldHaveAccessToArticle(wikiContext.Articles, authorizedUserProvider, ArticleAccessRule.Modify)
            .WithErrorCode(FailCode.Forbidden.ToString());

        RuleFor(r => r.UserId)
            .EntityShouldExist(wikiContext.Users);
    }
}