# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["OnlineShopping/OnlineShopping.csproj", "OnlineShopping/"]
COPY ["OnlineShopping.Core/OnlineShopping.Core.csproj", "OnlineShopping.Core/"]
COPY ["OnlineShopping.Infrastructure/OnlineShopping.Infrastructure.csproj", "OnlineShopping.Infrastructure/"]
RUN dotnet restore "OnlineShopping/OnlineShopping.csproj"

COPY . .
WORKDIR "/src/OnlineShopping"
RUN dotnet build "OnlineShopping.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OnlineShopping.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 80
EXPOSE 443

COPY --from=publish /app/publish .

HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
  CMD curl -f http://localhost/health || exit 1

ENTRYPOINT ["dotnet", "OnlineShopping.dll"]