# ---- Build stage ----
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY SemanticLayer.sln ./
COPY src/ ./src/

RUN dotnet restore SemanticLayer.sln
RUN dotnet publish src/SemanticLayer.Api/SemanticLayer.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

# ---- Runtime stage ----
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "SemanticLayer.Api.dll"]
