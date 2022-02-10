using FluentValidation;
using LightWiki.Data;
using LightWiki.Features.Articles.Requests;
using LightWiki.Infrastructure.Validators;

namespace LightWiki.Features.Articles.Validators;

public class UploadArticleImageValidator : AbstractValidator<UploadArticleImage>
{
    public UploadArticleImageValidator(WikiContext wikiContext)
    {
        RuleFor(r => r.ContentType)
            .Must(ct => UploadArticleImage.AcceptedFileMimeTypes.Contains(ct));

        RuleFor(r => r.ArticleId)
            .EntityShouldExist(wikiContext.Articles);
    }
}