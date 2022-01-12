using System.Threading;
using System.Threading.Tasks;
using LightWiki.ArticleEngine.MarkDown;
using LightWiki.ArticleEngine.Patches;
using LightWiki.Data;
using LightWiki.Data.Mongo.Enums;
using LightWiki.Data.Mongo.Models;
using LightWiki.Data.Mongo.Repositories;
using LightWiki.Domain.Models;
using LightWiki.Features.Articles.Requests;
using LightWiki.Infrastructure.Auth;
using LightWiki.Infrastructure.Models;
using MediatR;
using OneOf;

namespace LightWiki.Features.Articles.Handlers;

public class UpdateArticleContentHandler : IRequestHandler<UpdateArticleContent, OneOf<Success, Fail>>
{
    private readonly IPatchHelper _patchHelper;
    private readonly WikiContext _context;
    private readonly IAuthorizedUserProvider _authorizedUserProvider;
    private readonly IArticleMdRepository _articleMdRepository;
    private readonly IArticleHtmlRepository _articleHtmlRepository;
    private readonly IMdHelper _mdHelper;

    public UpdateArticleContentHandler(
        IPatchHelper patchHelper,
        WikiContext context,
        IAuthorizedUserProvider authorizedUserProvider,
        IArticleMdRepository articleMdRepository,
        IArticleHtmlRepository articleHtmlRepository,
#pragma warning disable SA1305
        IMdHelper mdHelper)
#pragma warning restore SA1305
    {
        _patchHelper = patchHelper;
        _context = context;
        _authorizedUserProvider = authorizedUserProvider;
        _articleMdRepository = articleMdRepository;
        _articleHtmlRepository = articleHtmlRepository;
        _mdHelper = mdHelper;
    }

    public async Task<OneOf<Success, Fail>> Handle(UpdateArticleContent request, CancellationToken cancellationToken)
    {
        var userContext = await _authorizedUserProvider.GetUser();
        await _context.Articles.FindAsync(request.ArticleId);

        var lastMdArticle = await _articleMdRepository.GetLatest(request.ArticleId);
        var lastHtmlArticle = await _articleHtmlRepository.GetLatest(request.ArticleId);

        var lastMdText = lastMdArticle == null ? string.Empty : lastMdArticle.Text;

        var patchText = _patchHelper.CreatePatch(lastMdText, request.Text);
        var version = new ArticleVersion
        {
            UserId = userContext.Id,
            Patch = patchText,
            ArticleId = request.ArticleId,
        };

        await _context.ArticleVersions.AddAsync(version, cancellationToken);

        if (lastMdArticle != null)
        {
            await _articleMdRepository.Remove(lastMdArticle.Id);
        }

        if (lastHtmlArticle != null)
        {
            await _articleHtmlRepository.Remove(lastHtmlArticle.Id);
        }

        var newMd = new ArticleMd
        {
            ArticleId = request.ArticleId,
            Text = request.Text,
            ArticleStoreType = ArticleStoreType.Latest,
            Index = lastMdArticle == null ? 0 : lastMdArticle.Index + 1,
        };

        await _articleMdRepository.Create(newMd);

        var html = _mdHelper.ConvertMdToHtml(request.Text);

        var newHtml = new ArticleHtml
        {
            ArticleId = request.ArticleId,
            Text = html,
            ArticleStoreType = ArticleStoreType.Latest,
            Index = lastHtmlArticle == null ? 0 : lastHtmlArticle.Index + 1,
        };

        await _articleHtmlRepository.Create(newHtml);

        return new Success();
    }
}