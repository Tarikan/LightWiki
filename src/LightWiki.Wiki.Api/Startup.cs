using System;
using AutoMapper;
using FluentValidation.AspNetCore;
using LightWiki.ArticleEngine.Patches;
using LightWiki.Auth;
using LightWiki.Data;
using LightWiki.Data.Mongo.Repositories;
using LightWiki.Features.Articles.Handlers;
using LightWiki.Features.Articles.Requests;
using LightWiki.Features.Articles.Responses.Models;
using LightWiki.Features.Articles.Validators;
using LightWiki.Features.Groups.Handlers;
using LightWiki.Features.Groups.Requests;
using LightWiki.Features.Groups.Validators;
using LightWiki.Features.Users.Handlers;
using LightWiki.Features.Users.Requests;
using LightWiki.Features.Users.Responses.Models;
using LightWiki.Features.Users.Validators;
using LightWiki.Infrastructure.Auth;
using LightWiki.Infrastructure.Configuration;
using LightWiki.Infrastructure.MediatR;
using LightWiki.Infrastructure.Models;
using LightWiki.Infrastructure.Web.Authentication;
using LightWiki.Infrastructure.Web.Extensions;
using LightWiki.Infrastructure.Web.Swagger;
using LightWiki.Shared.Query;
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
using Newtonsoft.Json.Serialization;
using Sieve.Services;

namespace LightWiki.Wiki.Api;

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
        var oauthConfiguration = Configuration.GetSection("OAuth").Get<OAuthConfiguration>();
        services.AddSingleton(oauthConfiguration);

        services.AddMediatR(typeof(Startup));
        AddHandlers(services);

        services.AddJwtAuthentication();
        services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddTransient<IAuthorizedUserProvider, AuthorizedUserProvider>();

        services.AddDbContext<WikiContext>(opts =>
            opts.UseNpgsql(connectionStrings.DbConnection));

        var mongoClient = new MongoClient(connectionStrings.MongoConnection);
        services.AddSingleton<IMongoClient>(mongoClient);
        services.AddScoped<IArticleHtmlRepository, ArticleHtmlRepository>();

        services.AddTransient<IPatchHelper, PatchHelper>();

        services.AddControllers()
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
            });

        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "LightWiki.Wiki.Api", Version = "v1" });
            c.ConfigureJwt();
            c.ConfigureCognitoAuth(oauthConfiguration);
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

    public void Configure(
        IApplicationBuilder app,
        IWebHostEnvironment env,
        IMapper mapper,
        WikiContext context,
        OAuthConfiguration oauthConfiguration)
    {
        mapper.ConfigurationProvider.AssertConfigurationIsValid();

        context.Database.Migrate();

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "LightWiki.Wiki.Api v1");
                c.OAuthClientId(oauthConfiguration.ClientId);
                c.OAuthUsePkce();
            });
        }

        app.UseHttpsRedirection();

        app.UseExceptionInterception(env);

        app.UseRouting();

        app.UseCors("default");

        app.UseAuthentication();

        app.UseAuthorization();

        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }

    private static void AddHandlers(IServiceCollection services)
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

        services.ForScoped<UpdateArticleContent, Success>()
            .WithValidation<UpdateArticleContentValidator>()
            .AddHandler<UpdateArticleContentHandler>();

        services.ForScoped<UpdateArticleContent, Success>()
            .WithValidation<UpdateArticleContentValidator>()
            .AddHandler<UpdateArticleContentHandler>();

        services.ForScoped<AddGroupAccess, Success>()
            .WithValidation<AddGroupAccessValidator>()
            .AddHandler<AddGroupAccessHandler>();

        services.ForScoped<AddPersonalAccess, Success>()
            .WithValidation<AddPersonalAccessValidator>()
            .AddHandler<AddPersonalAccessHandler>();

        #endregion

        #region ArticleAccess

        services.ForScoped<AddPersonalAccess, Success>()
            .WithValidation<AddPersonalAccessValidator>()
            .AddHandler<AddPersonalAccessHandler>();

        services.ForScoped<AddGroupAccess, Success>()
            .WithValidation<AddGroupAccessValidator>()
            .AddHandler<AddGroupAccessHandler>();

        services.ForScoped<RemovePersonalAccess, Success>()
            .WithValidation<RemovePersonalAccessValidator>()
            .AddHandler<RemovePersonalAccessHandler>();

        services.ForScoped<RemoveGroupAccess, Success>()
            .WithValidation<RemoveGroupAccessValidator>()
            .AddHandler<RemoveGroupAccessHandler>();

        #endregion

        #region Groups

        services.ForScoped<CreateGroup, SuccessWithId<int>>()
            .WithValidation<CreateGroupValidator>()
            .AddHandler<CreateGroupHandler>();

        services.ForScoped<RemoveGroup, Success>()
            .WithValidation<RemoveGroupValidator>()
            .AddHandler<RemoveGroupHandler>();

        services.ForScoped<AddUserToGroup, Success>()
            .WithValidation<AddUserToGroupValidator>()
            .AddHandler<AddUserToGroupHandler>();

        services.ForScoped<RemoveUserFromGroup, Success>()
            .WithValidation<RemoveUserFromGroupValidator>()
            .AddHandler<RemoveUserFromGroupHandler>();

        services.ForScoped<GetGroupMembers, CollectionResult<UserModel>>()
            .WithValidation<GetGroupMembersValidator>()
            .AddHandler<GetGroupMembersHandler>();

        #endregion

        #region Users

        services.ForScoped<GetUser, UserModel>()
            .WithValidation<GetUserValidator>()
            .AddHandler<GetUserHandler>();

        #endregion
    }
}