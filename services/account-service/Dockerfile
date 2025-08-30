FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["AccountService.API/AccountService.API.csproj", "AccountService.API/"]
COPY ["AccountService.Application/AccountService.Application.csproj", "AccountService.Application/"]
COPY ["AccountService.Persistence/AccountService.Persistence.csproj", "AccountService.Persistence/"]
COPY ["AccountService.Infrastructure/AccountService.Infrastructure.csproj", "AccountService.Infrastructure/"]
COPY ["AccountService.Domain/AccountService.Domain.csproj", "AccountService.Domain/"]

RUN dotnet restore "AccountService.API/AccountService.API.csproj"
COPY . .

WORKDIR "/src/AccountService.API"
RUN dotnet build "AccountService.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AccountService.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AccountService.dll"]