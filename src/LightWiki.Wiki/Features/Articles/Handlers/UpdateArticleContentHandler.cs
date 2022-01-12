using System.Threading;
using System.Threading.Tasks;
using LightWiki.ArticleEngine.Patches;
using LightWiki.Data;
using LightWiki.Features.Articles.Requests;
using LightWiki.Infrastructure.Models;
using MediatR;
using OneOf;

namespace LightWiki.Features.Articles.Handlers;

public class UpdateArticleContentHandler : IRequestHandler<UpdateArticleContent, OneOf<Success, Fail>>
{
    private readonly IPatchHelper _patchHelper;
    private readonly WikiContext _context;

    public UpdateArticleContentHandler(IPatchHelper patchHelper, WikiContext context)
    {
        _patchHelper = patchHelper;
        _context = context;
    }

    public async Task<OneOf<Success, Fail>> Handle(UpdateArticleContent request, CancellationToken cancellationToken)
    {
        var article = await _context.Articles.FindAsync(request.ArticleId);
        throw new System.NotImplementedException();
    }
}