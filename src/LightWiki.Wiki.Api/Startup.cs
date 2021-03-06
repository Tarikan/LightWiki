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
using LightWiki.Infrastructure.Aws.S3;
using LightWiki.Infrastructure.Configuration;
using LightWiki.Infrastructure.Configuration.Aws;
using LightWiki.Infrastructure.MediatR;
using LightWiki.Infrastructure.Models;
using LightWiki.Infrastructure.Web.Authentication;
using LightWiki.Infrastructure.Web.Extensions;
using LightWiki.Infrastructure.Web.Swagger;
using LightWiki.Shared.Helpers;
using LightWiki.Shared.Models;
using LightWiki.Shared.Query;
using LightWiki.Wiki.Api.Configuration;
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
        var s3Settings = Configuration.GetSection("AWS").GetSection("S3").Get<S3Configuration>();
        services.AddSingleton(s3Settings);
        var imageSizeSettings = Configuration.GetSection("ImageSizes").Get<ImageSizeConfiguration>();
        services.AddSingleton(imageSizeSettings);
        var firstStartConfiguration = Configuration.GetSection("FirstStartSetup").Get<FirstStartConfiguration>();
        services.AddSingleton(firstStartConfiguration);

        services.AddMediatR(typeof(Startup));
        AddHandlers(services);
        AddAwsServices(services);

        services.AddTransient<IFirstStartConfigurator, FirstStartConfigurator>();

        services.AddJwtAuthentication();
        services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddTransient<IAuthorizedUserProvider, AuthorizedUserProvider>();

        services.AddDbContext<WikiContext>(
            opts =>
            {
                opts.UseNpgsql(connectionStrings.DbConnection);
                opts.EnableSensitiveDataLogging()
                    .UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()));
            }, ServiceLifetime.Transient);

        var mongoClient = new MongoClient(connectionStrings.MongoConnection);
        services.AddSingleton<IMongoClient>(mongoClient);
        services.AddScoped<IArticleHtmlRepository, ArticleHtmlRepository>();
        services.AddScoped<IArticleHierarchyNodeRepository, ArticleHierarchyNodeRepository>();
        services.AddScoped<IArticlePatchRepository, ArticlePatchRepository>();

        services.AddTransient<IPatchHelper, PatchHelper>();
        services.AddScoped<IImageHelper, ImageHelper>();
        services.AddSingleton<IHashHelper, HashHelper>();

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
            c.SchemaFilter<SwaggerExcludeSchemaFilter>();
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
        sanitizer.UriAttributes.Add("https");
        sanitizer.AllowedAttributes.Add("srcset");
        services.AddSingleton<IHtmlSanitizer>(sanitizer);
    }

    public void Configure(
        IApplicationBuilder app,
        IWebHostEnvironment env,
        IMapper mapper,
        WikiContext context,
        OAuthConfiguration oauthConfiguration,
        IFirstStartConfigurator firstStartConfigurator)
    {
        mapper.ConfigurationProvider.AssertConfigurationIsValid();

        context.Database.Migrate();

        if (env.IsDevelopment()
            || env.EnvironmentName == "DockerCompose"
            || env.EnvironmentName == "DockerComposeDevelopment")
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

        // app.UseHttpsRedirection();
        app.UseExceptionInterception(env);

        app.UseRouting();

        app.UseCors("default");

        app.UseAuthentication();

        app.UseAuthorization();

        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

        firstStartConfigurator.Configure();
    }

    private static void AddAwsServices(IServiceCollection services)
    {
        services.AddAwsS3();
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

        services.ForScoped<UpdateArticle, SuccessWithId<string>>()
            .WithValidation<UpdateArticleValidator>()
            .AddHandler<UpdateArticleHandler>();

        services.ForScoped<UpdateArticleContent, Success>()
            .WithValidation<UpdateArticleContentValidator>()
            .AddHandler<UpdateArticleContentHandler>();

        services.ForScoped<UpdateArticleContent, Success>()
            .WithValidation<UpdateArticleContentValidator>()
            .AddHandler<UpdateArticleContentHandler>();

        services.ForScoped<RemoveArticle, Success>()
            .WithValidation<RemoveArticleValidator>()
            .AddHandler<RemoveArticleHandler>();

        services.ForScoped<GetArticleBySlug, ArticleModel>()
            .WithValidation<GetArticleBySlugValidator>()
            .AddHandler<GetArticleBySlugHandler>();

        services.ForScoped<GetArticleAncestors, ArticleAncestorsModel>()
            .WithValidation<GetArticleAncestorsValidator>()
            .AddHandler<GetArticleAncestorsHandler>();

        services.ForScoped<UploadArticleImage, ResponsiveImageModel>()
            .WithValidation<UploadArticleImageValidator>()
            .AddHandler<UploadArticleImageHandler>();

        #endregion

        #region ArticleAccess

        services.ForScoped<AddArticleAccess, Success>()
            .WithValidation<AddArticleAccessValidator>()
            .AddHandler<AddArticleAccessHandler>();

        services.ForScoped<RemoveArticleAccess, Success>()
            .WithValidation<RemoveArticleAccessValidator>()
            .AddHandler<RemoveArticleAccessHandler>();

        services.ForScoped<GetArticleAccessRules, CollectionResult<ArticleAccessRuleModel>>()
            .WithValidation<GetArticleAccessRulesValidator>()
            .AddHandler<GetArticleAccessRulesHandler>();

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

        services.ForScoped<UploadUserImage, ResponsiveImageModel>()
            .WithValidation<UploadUserImageValidator>()
            .AddHandler<UploadUserImageHandler>();

        services.ForScoped<UpdateUserInfo, Success>()
            .WithValidation<UpdateUserInfoValidator>()
            .AddHandler<UpdateUserInfoHandler>();

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

        services.ForScoped<AddWorkspaceAccess, Success>()
            .WithValidation<AddWorkspaceAccessValidator>()
            .AddHandler<AddWorkspaceAccessHandler>();

        services.ForScoped<RemoveWorkspaceAccess, Success>()
            .WithValidation<RemoveWorkspaceAccessValidator>()
            .AddHandler<RemoveWorkspaceAccessHandler>();

        #endregion
    }
}