using System;
using AutoMapper;
using FluentValidation.AspNetCore;
using LightWiki.Data;
using LightWiki.Data.Mongo.Repositories;
using LightWiki.Features.Articles.Handlers;
using LightWiki.Features.Articles.Requests;
using LightWiki.Features.Articles.Responses.Models;
using LightWiki.Features.Articles.Validators;
using LightWiki.Infrastructure.Auth;
using LightWiki.Infrastructure.Configuration;
using LightWiki.Infrastructure.MediatR;
using LightWiki.Infrastructure.Models;
using LightWiki.Infrastructure.Web.Authentication;
using LightWiki.Shared.Query;
using LightWiki.Wiki.Api.Auth;
using MediatR;
using MicroElements.Swashbuckle.FluentValidation;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using Sieve.Services;

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
            var connectionStrings = Configuration.GetSection("ConnectionStrings").Get<ConnectionStrings>();
            services.AddSingleton(connectionStrings);
            var appConfiguration = Configuration.GetSection("AppConfiguration").Get<AppConfiguration>();
            services.AddSingleton(appConfiguration);

            services.AddMediatR(typeof(Startup));
            AddHandlers(services);

            services.AddJwtAuthentication();
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<IAuthorizedUserProvider, AuthorizedUserProvider>();

            services.AddDbContext<WikiContext>(opts => opts.UseNpgsql(connectionStrings.DbConnection));

            var mongoClient = new MongoClient(connectionStrings.MongoConnection);
            services.AddSingleton(mongoClient);
            services.AddScoped<IArticleHtmlRepository, ArticleHtmlRepository>();

            services.AddControllers()
                .AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "LightWiki.Wiki.Api", Version = "v1" });
            });

            services.AddCors(o => o.AddPolicy("default", corsPolicyBuilder =>
            {
                corsPolicyBuilder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            }));
            services.AddFluentValidation(options =>
            {
                options.AutomaticValidationEnabled = false;
                options.ConfigureClientsideValidation(enabled: false);
                options.ValidatorFactoryType = typeof(HttpContextServiceProviderValidatorFactory);
            });
            services.AddFluentValidationRulesToSwagger();
            services.AddHealthChecks();

            services.AddScoped<ISieveProcessor, ApplicationSieveProcessor>();

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IMapper mapper)
        {
            mapper.ConfigurationProvider.AssertConfigurationIsValid();

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