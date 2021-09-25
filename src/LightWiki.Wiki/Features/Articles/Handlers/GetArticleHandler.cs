using System.Threading;
using System.Threading.Tasks;
using LightWiki.Data;
using LightWiki.Features.Articles.Requests;
using LightWiki.Features.Articles.Responses.Models;
using LightWiki.Infrastructure.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;

namespace LightWiki.Features.Articles.Handlers
{
    public class GetArticleHandler : IRequestHandler<GetArticle, OneOf<ArticleModel, Fail>>
    {
        private readonly WikiContext _wikiContext;

        public GetArticleHandler(WikiContext wikiContext)
        {
            _wikiContext = wikiContext;
        }

        public async Task<OneOf<ArticleModel, Fail>> Handle(GetArticle request, CancellationToken cancellationToken)
        {
            var article = await _wikiContext.Articles.Include(a => a.Versions)
                .SingleOrDefaultAsync(a => a.Id == request.Id, cancellationToken);
            throw new System.NotImplementedException();
        }
    }
}