FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8090
EXPOSE 8082

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/ImageApp.Web/ImageApp.Web.csproj", "src/ImageApp.Web/"]
COPY ["src/ImageApp.Application/ImageApp.Application.csproj", "src/ImageApp.Application/"]
COPY ["src/ImageApp.Domain/ImageApp.Domain.csproj", "src/ImageApp.Domain/"]
COPY ["src/ImageApp.Infrastructure/ImageApp.Infrastructure.csproj", "src/ImageApp.Infrastructure/"]
RUN dotnet restore "src/ImageApp.Web/ImageApp.Web.csproj"
COPY . .
WORKDIR "/src/src/ImageApp.Web/ImageApp.Web"
RUN dotnet build "ImageApp.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ImageApp.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ImageApp.Web.dll"]
