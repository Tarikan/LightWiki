using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using LightWiki.Data;
using LightWiki.Features.Articles.Requests;
using LightWiki.Infrastructure.Models;
using MediatR;
using OneOf;

namespace LightWiki.Features.Articles.Handlers
{
    public sealed class UpdateArticleHandler : IRequestHandler<UpdateArticle, OneOf<Success, Fail>>
    {
        private readonly IMapper _mapper;
        private readonly WikiContext _wikiContext;

        public UpdateArticleHandler(IMapper mapper, WikiContext wikiContext)
        {
            _mapper = mapper;
            _wikiContext = wikiContext;
        }

        public async Task<OneOf<Success, Fail>> Handle(UpdateArticle request, CancellationToken cancellationToken)
        {
            var article = await _wikiContext.Articles.FindAsync(request.Id);

            article = _mapper.Map(request, article);

            await _wikiContext.SaveChangesAsync(cancellationToken);

            return new Success();
        }
    }
}