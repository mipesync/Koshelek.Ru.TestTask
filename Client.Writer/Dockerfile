FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Client.Writer/Client.Writer.csproj", "Client.Writer/"]
RUN dotnet restore "Client.Writer/Client.Writer.csproj"
COPY . .
WORKDIR "/src/Client.Writer"
RUN dotnet build "Client.Writer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Client.Writer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Client.Writer.dll"]
