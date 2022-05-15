FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/LightWiki.Wiki.Api/LightWiki.Wiki.Api.csproj", "LightWiki.Wiki.Api/"]
COPY ["src/LightWiki.ArticleEngine/LightWiki.ArticleEngine.csproj", "LightWiki.ArticleEngine/"]
COPY ["src/LightWiki.Domain/LightWiki.Domain.csproj", "LightWiki.Domain/"]
COPY ["src/LightWiki.Infrastructure.Web/LightWiki.Infrastructure.Web.csproj", "LightWiki.Infrastructure.Web/"]
COPY ["src/LightWiki.Infrastructure/LightWiki.Infrastructure.csproj", "LightWiki.Infrastructure/"]
COPY ["src/LightWiki.Infrastructure/StyleCop/*", "LightWiki.Infrastructure/StyleCop/"]
COPY ["src/LightWiki.Wiki/LightWiki.Wiki.csproj", "LightWiki.Wiki/"]
COPY ["src/LightWiki.Data.Mongo/LightWiki.Data.Mongo.csproj", "LightWiki.Data.Mongo/"]
COPY ["src/LightWiki.Data/LightWiki.Data.csproj", "LightWiki.Data/"]
COPY ["src/LightWiki.Shared/LightWiki.Shared.csproj", "LightWiki.Shared/"]

RUN dotnet restore "LightWiki.Wiki.Api/LightWiki.Wiki.Api.csproj"
RUN echo $(ls "LightWiki.Wiki.Api")
COPY src .

WORKDIR "LightWiki.Wiki.Api"
RUN echo $(ls)

RUN dotnet publish -c Release --no-restore -o /app/publish

FROM base AS final
COPY --from=build /app ./
WORKDIR /app/publish

ENTRYPOINT ["dotnet", "LightWiki.Wiki.Api.dll"]
