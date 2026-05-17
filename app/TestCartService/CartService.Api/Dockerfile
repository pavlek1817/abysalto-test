FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY CartService.Api/CartService.Api.csproj CartService.Api/
COPY CartService.Application/CartService.Application.csproj CartService.Application/
COPY CartService.Infrastructure/CartService.Infrastructure.csproj CartService.Infrastructure/
COPY CartService.Domain/CartService.Domain.csproj CartService.Domain/

RUN dotnet restore CartService.Api/CartService.Api.csproj

COPY CartService.Api/ CartService.Api/
COPY CartService.Application/ CartService.Application/
COPY CartService.Infrastructure/ CartService.Infrastructure/
COPY CartService.Domain/ CartService.Domain/


RUN dotnet publish CartService.Api/CartService.Api.csproj -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "CartService.Api.dll"]
