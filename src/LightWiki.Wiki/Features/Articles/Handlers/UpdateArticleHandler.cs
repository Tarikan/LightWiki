using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using LightWiki.Data;
using LightWiki.Features.Articles.Requests;
using LightWiki.Infrastructure.Models;
using MediatR;
using OneOf;
using Slugify;

namespace LightWiki.Features.Articles.Handlers;

public sealed class UpdateArticleHandler : IRequestHandler<UpdateArticle, OneOf<SuccessWithId<string>, Fail>>
{
    private readonly WikiContext _wikiContext;
    private readonly ISlugHelper _slugHelper;

    public UpdateArticleHandler(WikiContext wikiContext, ISlugHelper slugHelper)
    {
        _wikiContext = wikiContext;
        _slugHelper = slugHelper;
    }

    public async Task<OneOf<SuccessWithId<string>, Fail>> Handle(UpdateArticle request, CancellationToken cancellationToken)
    {
        var article = await _wikiContext.Articles.FindAsync(request.Id);

        article.Name = request.Name;
        article.Slug = _slugHelper.GenerateSlug(request.Name);

        await _wikiContext.SaveChangesAsync(cancellationToken);

        return new SuccessWithId<string>(article.Slug);
    }
}