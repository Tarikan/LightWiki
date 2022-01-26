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

public sealed class UpdateArticleHandler : IRequestHandler<UpdateArticle, OneOf<Success, Fail>>
{
    private readonly IMapper _mapper;
    private readonly WikiContext _wikiContext;
    private readonly ISlugHelper _slugHelper;

    public UpdateArticleHandler(IMapper mapper, WikiContext wikiContext, ISlugHelper slugHelper)
    {
        _mapper = mapper;
        _wikiContext = wikiContext;
        _slugHelper = slugHelper;
    }

    public async Task<OneOf<Success, Fail>> Handle(UpdateArticle request, CancellationToken cancellationToken)
    {
        var article = await _wikiContext.Articles.FindAsync(request.Id);

        article = _mapper.Map(request, article);
        article.Slug = _slugHelper.GenerateSlug(request.Name);

        await _wikiContext.SaveChangesAsync(cancellationToken);

        return new Success();
    }
}