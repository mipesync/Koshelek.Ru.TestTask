FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081
EXPOSE 5241

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Backend.API/Backend.API.csproj", "Backend.API/"]
COPY ["Backend.DAL/Backend.DAL.csproj", "Backend.DAL/"]
COPY ["Backend.Models/Backend.Models.csproj", "Backend.Models/"]
COPY ["Backend.Application/Backend.Application.csproj", "Backend.Application/"]
RUN dotnet restore "Backend.API/Backend.API.csproj"
COPY . .
WORKDIR "/src/Backend.API"
RUN dotnet build "Backend.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Backend.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Backend.API.dll"]
