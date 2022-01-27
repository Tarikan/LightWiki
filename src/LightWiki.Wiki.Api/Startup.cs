using System;
using AutoMapper;
using FluentValidation.AspNetCore;
using Ganss.XSS;
using LightWiki.ArticleEngine.Patches;
using LightWiki.Auth;
using LightWiki.Data;
using LightWiki.Data.Mongo.Repositories;
using LightWiki.Features.Articles.Handlers;
using LightWiki.Features.Articles.Requests;
using LightWiki.Features.Articles.Responses.Models;
using LightWiki.Features.Articles.Validators;
using LightWiki.Features.ArticleVersions.Handlers;
using LightWiki.Features.ArticleVersions.Requests;
using LightWiki.Features.ArticleVersions.Responses.Models;
using LightWiki.Features.ArticleVersions.Validators;
using LightWiki.Features.Groups.Handlers;
using LightWiki.Features.Groups.Requests;
using LightWiki.Features.Groups.Validators;
using LightWiki.Features.Users.Handlers;
using LightWiki.Features.Users.Requests;
using LightWiki.Features.Users.Responses.Models;
using LightWiki.Features.Users.Validators;
using LightWiki.Features.Workspaces.Handlers;
using LightWiki.Features.Workspaces.Requests;
using LightWiki.Features.Workspaces.Responses.Models;
using LightWiki.Features.Workspaces.Validators;
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
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using Newtonsoft.Json.Serialization;
using Sieve.Services;
using Slugify;
using AddGroupAccess = LightWiki.Features.Articles.Requests.AddGroupAccess;
using AddGroupAccessHandler = LightWiki.Features.Articles.Handlers.AddGroupAccessHandler;
using AddGroupAccessValidator = LightWiki.Features.Articles.Validators.AddGroupAccessValidator;
using AddPersonalAccess = LightWiki.Features.Articles.Requests.AddPersonalAccess;
using AddPersonalAccessHandler = LightWiki.Features.Articles.Handlers.AddPersonalAccessHandler;
using AddPersonalAccessValidator = LightWiki.Features.Articles.Validators.AddPersonalAccessValidator;
using RemoveGroupAccess = LightWiki.Features.Articles.Requests.RemoveGroupAccess;
using RemoveGroupAccessHandler = LightWiki.Features.Articles.Handlers.RemoveGroupAccessHandler;
using RemoveGroupAccessValidator = LightWiki.Features.Articles.Validators.RemoveGroupAccessValidator;
using RemovePersonalAccess = LightWiki.Features.Articles.Requests.RemovePersonalAccess;
using RemovePersonalAccessHandler = LightWiki.Features.Articles.Handlers.RemovePersonalAccessHandler;
using RemovePersonalAccessValidator = LightWiki.Features.Articles.Validators.RemovePersonalAccessValidator;

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
        {
            opts.UseNpgsql(connectionStrings.DbConnection);
            opts.EnableSensitiveDataLogging()
                .UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()));
        });

        var mongoClient = new MongoClient(connectionStrings.MongoConnection);
        services.AddSingleton<IMongoClient>(mongoClient);
        services.AddScoped<IArticleHtmlRepository, ArticleHtmlRepository>();
        services.AddScoped<IArticleHierarchyNodeRepository, ArticleHierarchyNodeRepository>();

        services.AddTransient<IPatchHelper, PatchHelper>();

        services.AddControllers()
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
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

        var slugConfig = new SlugHelperConfiguration
        {
            ForceLowerCase = false,
        };
        services.AddSingleton<ISlugHelper>(new SlugHelper(slugConfig));

        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

        var sanitizer = new HtmlSanitizer();
        services.AddSingleton<IHtmlSanitizer>(sanitizer);
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

        services.ForScoped<RemoveArticle, Success>()
            .WithValidation<RemoveArticleValidator>()
            .AddHandler<RemoveArticleHandler>();

        services.ForScoped<GetArticleBySlug, ArticleModel>()
            .WithValidation<GetArticleBySlugValidator>()
            .AddHandler<GetArticleBySlugHandler>();

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

        #region GroupAccess

        services.ForScoped<AddGroupPersonalAccessRule, Success>()
            .WithValidation<AddGroupPersonalAccessRuleValidator>()
            .AddHandler<AddGroupPersonalAccessRuleHandler>();

        services.ForScoped<RemoveGroupPersonalAccessRule, Success>()
            .WithValidation<RemoveGroupPersonalAccessRuleValidator>()
            .AddHandler<RemoveGroupPersonalAccessRuleHandler>();

        #endregion

        #region Users

        services.ForScoped<GetUser, UserModel>()
            .WithValidation<GetUserValidator>()
            .AddHandler<GetUserHandler>();

        #endregion

        #region ArticleVersions

        services.ForScoped<GetArticleVersions, CollectionResult<ArticleVersionModel>>()
            .WithValidation<GetArticleVersionsValidator>()
            .AddHandler<GetArticleVersionsHandler>();

        services.ForScoped<GetArticleVersionContent, ArticleContentModel>()
            .WithValidation<GetArticleVersionContentValidator>()
            .AddHandler<GetArticleVersionContentHandler>();

        #endregion

        #region Workspaces

        services.ForScoped<GetWorkspaceTree, CollectionResult<ArticleHeaderModel>>()
            .WithValidation<GetWorkspaceTreeValidator>()
            .AddHandler<GetWorkspaceTreeHandler>();

        services.ForScoped<GetWorkspaces, CollectionResult<WorkspaceModel>>()
            .AddHandler<GetWorkspacesHandler>();

        services.ForScoped<CreateWorkspace, SuccessWithId<int>>()
            .WithValidation<CreateWorkspaceValidator>()
            .AddHandler<CreateWorkspaceHandler>();

        services.ForScoped<UpdateWorkspace, Success>()
            .WithValidation<UpdateWorkspaceValidator>()
            .AddHandler<UpdateWorkspaceHandler>();

        services.ForScoped<RemoveWorkspace, Success>()
            .WithValidation<RemoveWorkspaceValidator>()
            .AddHandler<RemoveWorkspaceHandler>();

        services.ForScoped<GetWorkspaceInfo, WorkspaceInfoModel>()
            .WithValidation<GetWorkspaceInfoValidator>()
            .AddHandler<GetWorkspaceInfoHandler>();

        services.ForScoped<GetWorkspaceBySlug, WorkspaceModel>()
            .WithValidation<GetWorkspaceBySlugValidator>()
            .AddHandler<GetWorkspaceBySlugHandler>();

        #endregion

        #region WorkspaceAccess

        services.ForScoped<AddWorkspacePersonalAccess, Success>()
            .WithValidation<LightWiki.Features.Workspaces.Validators.AddPersonalAccessValidator>()
            .AddHandler<LightWiki.Features.Workspaces.Handlers.AddPersonalAccessHandler>();

        services.ForScoped<AddWorkspaceGroupAccess, Success>()
            .WithValidation<LightWiki.Features.Workspaces.Validators.AddGroupAccessValidator>()
            .AddHandler<LightWiki.Features.Workspaces.Handlers.AddGroupAccessHandler>();

        services.ForScoped<RemoveWorkspacePersonalAccess, Success>()
            .WithValidation<LightWiki.Features.Workspaces.Validators.RemovePersonalAccessValidator>()
            .AddHandler<LightWiki.Features.Workspaces.Handlers.RemovePersonalAccessHandler>();

        services.ForScoped<RemoveWorkspaceGroupAccess, Success>()
            .WithValidation<LightWiki.Features.Workspaces.Validators.RemoveGroupAccessValidator>()
            .AddHandler<LightWiki.Features.Workspaces.Handlers.RemoveGroupAccessHandler>();

        #endregion
    }
}