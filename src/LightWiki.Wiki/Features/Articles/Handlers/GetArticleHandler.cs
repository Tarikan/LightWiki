using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DiffMatchPatch;
using LightWiki.Data;
using LightWiki.Features.Articles.Requests;
using LightWiki.Features.Articles.Responses.Models;
using LightWiki.Infrastructure.Models;
using LightWiki.Shared.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;

namespace LightWiki.Features.Articles.Handlers
{
    public sealed class GetArticleHandler : IRequestHandler<GetArticle, OneOf<ArticleWithContentModel, Fail>>
    {
        private readonly WikiContext _wikiContext;
        private readonly IMapper _mapper;

        public GetArticleHandler(WikiContext wikiContext, IMapper mapper)
        {
            _wikiContext = wikiContext;
            _mapper = mapper;
        }

        public async Task<OneOf<ArticleWithContentModel, Fail>> Handle(
            GetArticle request,
            CancellationToken cancellationToken)
        {
            var article = await _wikiContext.Articles
                              .Include(a => a.Versions)
                              .SingleOrDefaultAsync(a => a.Id == request.Id, cancellationToken) ??
                          throw new ArticleDoesNotExistsException();

            var dmp = new diff_match_patch();
            var text = article.Versions.OrderBy(v => v.CreatedAt)
                .Select(version => dmp.patch_fromText(version.Patch))
                .Aggregate(string.Empty, (current, patches) => dmp.patch_apply(patches, current)[0] as string);

            var model = _mapper.Map<ArticleWithContentModel>(article);
            model.Content = text;

            return model;
        }
    }
}