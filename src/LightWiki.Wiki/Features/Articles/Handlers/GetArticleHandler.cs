using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using LightWiki.Data;
using LightWiki.Data.Mongo.Repositories;
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
        private readonly ArticleHtmlRepository _articleHtmlRepository;

        public GetArticleHandler(WikiContext wikiContext, IMapper mapper, ArticleHtmlRepository articleHtmlRepository)
        {
            _wikiContext = wikiContext;
            _mapper = mapper;
            _articleHtmlRepository = articleHtmlRepository;
        }

        public async Task<OneOf<ArticleWithContentModel, Fail>> Handle(
            GetArticle request,
            CancellationToken cancellationToken)
        {
            var article = await _wikiContext.Articles
                              .Include(a => a.Versions)
                              .SingleOrDefaultAsync(a => a.Id == request.Id, cancellationToken) ??
                          throw new ArticleDoesNotExistsException();

            var text = (await _articleHtmlRepository.GetLatest(article.Id)).Text;

            var model = _mapper.Map<ArticleWithContentModel>(article);
            model.Content = text;

            return model;
        }
    }
}