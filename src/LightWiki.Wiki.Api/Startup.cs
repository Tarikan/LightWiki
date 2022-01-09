using LightWiki.Features.Articles.Handlers;
using LightWiki.Features.Articles.Requests;
using LightWiki.Features.Articles.Responses.Models;
using LightWiki.Features.Articles.Validators;
using LightWiki.Infrastructure.MediatR;
using LightWiki.Infrastructure.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace LightWiki.Wiki.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            AddHandlers(services);

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "LightWiki.Wiki.Api", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "LightWiki.Wiki.Api v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }

        private void AddHandlers(IServiceCollection services)
        {
            #region Articles

            services.ForScoped<CreateArticle, SuccessWithId<int>>()
                .WithValidation<CreateArticleValidator>()
                .AddHandler<CreateArticleHandler>();

            services.ForScoped<GetArticleContent, ArticleContentModel>()
                .WithValidation<GetArticleContentValidator>()
                .AddHandler<GetArticleContentHandler>();

            services.ForScoped<GetArticle, ArticleModel>()
                .WithValidation<GetArticleValidator>()
                .AddHandler<GetArticleHandler>();

            services.ForScoped<GetArticles, CollectionResult<ArticleModel>>()
                .AddHandler<GetArticlesHandler>();

            services.ForScoped<UpdateArticle, Success>()
                .WithValidation<UpdateArticleValidator>()
                .AddHandler<UpdateArticleHandler>();

            #endregion

            #region Users

            

            #endregion
        }
    }
}