FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["src/LightWiki.Wiki.Api/LightWiki.Wiki.Api.csproj", "LightWiki.Wiki.Api/"]
RUN dotnet restore "src/LightWiki.Wiki.Api/LightWiki.Wiki.Api.csproj"
COPY . .
WORKDIR "/src/LightWiki.Wiki.Api"
RUN dotnet build "LightWiki.Wiki.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LightWiki.Wiki.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LightWiki.Wiki.Api.dll"]
