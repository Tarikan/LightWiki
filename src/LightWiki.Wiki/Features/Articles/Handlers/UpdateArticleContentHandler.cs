using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LightWiki.ArticleEngine.Patches;
using LightWiki.Data;
using LightWiki.Data.Mongo.Enums;
using LightWiki.Data.Mongo.Models;
using LightWiki.Data.Mongo.Repositories;
using LightWiki.Domain.Enums;
using LightWiki.Domain.Models;
using LightWiki.Features.Articles.Requests;
using LightWiki.Infrastructure.Auth;
using LightWiki.Infrastructure.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;

namespace LightWiki.Features.Articles.Handlers;

public class UpdateArticleContentHandler : IRequestHandler<UpdateArticleContent, OneOf<Success, Fail>>
{
    private readonly IPatchHelper _patchHelper;
    private readonly WikiContext _context;
    private readonly IAuthorizedUserProvider _authorizedUserProvider;
    private readonly IArticleHtmlRepository _articleHtmlRepository;

    public UpdateArticleContentHandler(
        IPatchHelper patchHelper,
        WikiContext context,
        IAuthorizedUserProvider authorizedUserProvider,
        IArticleHtmlRepository articleHtmlRepository)
    {
        _patchHelper = patchHelper;
        _context = context;
        _authorizedUserProvider = authorizedUserProvider;
        _articleHtmlRepository = articleHtmlRepository;
    }

    public async Task<OneOf<Success, Fail>> Handle(UpdateArticleContent request, CancellationToken cancellationToken)
    {
        var userContext = await _authorizedUserProvider.GetUser();

        var lastHtmlArticle = await _articleHtmlRepository.GetLatest(request.ArticleId);

        var lastHtmlText = lastHtmlArticle == null ? string.Empty : lastHtmlArticle.Text;

        var patchText = _patchHelper.CreatePatch(lastHtmlText, request.Text);
        var version = new ArticleVersion
        {
            UserId = userContext.Id,
            Patch = patchText,
            ArticleId = request.ArticleId,
        };

        await _context.ArticleVersions.AddAsync(version, cancellationToken);

        if (lastHtmlArticle != null)
        {
            await _articleHtmlRepository.Remove(lastHtmlArticle.Id);
        }

        var newHtml = new ArticleHtml
        {
            ArticleId = request.ArticleId,
            Text = request.Text,
            ArticleStoreType = ArticleStoreType.Latest,
            Index = lastHtmlArticle == null ? 0 : lastHtmlArticle.Index + 1,
        };

        await _articleHtmlRepository.Create(newHtml);

        await _context.SaveChangesAsync(cancellationToken);

        return new Success();
    }
}