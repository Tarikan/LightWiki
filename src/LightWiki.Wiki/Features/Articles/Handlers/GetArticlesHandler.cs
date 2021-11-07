using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using LightWiki.Data;
using LightWiki.Domain.Enums;
using LightWiki.Features.Articles.Requests;
using LightWiki.Features.Articles.Responses.Models;
using LightWiki.Infrastructure.Auth;
using LightWiki.Infrastructure.Extensions;
using LightWiki.Infrastructure.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;
using Sieve.Services;

namespace LightWiki.Features.Articles.Handlers
{
    public class GetArticlesHandler : IRequestHandler<GetArticles, OneOf<CollectionResult<ArticleModel>, Fail>>
    {
        private readonly WikiContext _wikiContext;
        private readonly IMapper _mapper;
        private readonly IAuthorizedUserProvider _authorizedUserProvider;
        private readonly ISieveProcessor _sieveProcessor;

        public GetArticlesHandler(
            WikiContext wikiContext,
            IMapper mapper,
            IAuthorizedUserProvider authorizedUserProvider,
            ISieveProcessor sieveProcessor)
        {
            _wikiContext = wikiContext;
            _mapper = mapper;
            _authorizedUserProvider = authorizedUserProvider;
            _sieveProcessor = sieveProcessor;
        }

        public async Task<OneOf<CollectionResult<ArticleModel>, Fail>>
            Handle(GetArticles request, CancellationToken cancellationToken)
        {
            var userContext = _authorizedUserProvider.GetUserOrDefault();

            var query =
                _wikiContext.Articles.Where(a => a.GlobalAccessRule >= ArticleAccessRule.Read ||
                                                 a.GroupAccessRules.Any(gar =>
                                                     gar.ArticleAccessRule >= ArticleAccessRule.Read &&
                                                     gar.Group.Users.Any(u => u.Id == userContext.Id)) ||
                                                 a.PersonalAccessRules.Any(par =>
                                                     par.UserId == userContext.Id &&
                                                     par.ArticleAccessRule >= ArticleAccessRule.Read))
                    .AsNoTracking();
            var total = await query.CountAsync(cancellationToken);

            var result = await _sieveProcessor.Apply(request, query).ToCollectionResult(total, cancellationToken);

            return _mapper.Map<CollectionResult<ArticleModel>>(result);
        }
    }
}