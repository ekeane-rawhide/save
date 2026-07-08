# Build context must be the repo root (relative ProjectReferences need sibling folders):
#   docker build -f EMK.Save.API/Dockerfile -t save-api .

# ── Build ────────────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Restore first, on just the .csproj files, so dependency layers cache across code changes
COPY EMK.Save.API/EMK.Save.API.csproj EMK.Save.API/
COPY EMK.Save.BL/EMK.Save.BL.csproj EMK.Save.BL/
COPY EMK.Save.BL.Models/EMK.Save.BL.Models.csproj EMK.Save.BL.Models/
COPY EMK.Save.PL/EMK.Save.PL.csproj EMK.Save.PL/
RUN dotnet restore EMK.Save.API/EMK.Save.API.csproj

COPY EMK.Save.API/ EMK.Save.API/
COPY EMK.Save.BL/ EMK.Save.BL/
COPY EMK.Save.BL.Models/ EMK.Save.BL.Models/
COPY EMK.Save.PL/ EMK.Save.PL/

RUN dotnet publish EMK.Save.API/EMK.Save.API.csproj -c Release -o /app/publish --no-restore

# ── Runtime ──────────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# curl is needed for the HEALTHCHECK below — the base runtime image doesn't include it
RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/* \
    && useradd -m -u 1000 appuser

COPY --from=build /app/publish .
RUN chown -R appuser /app
USER appuser

ENV ASPNETCORE_URLS=http://+:8080 \
    ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080

HEALTHCHECK --interval=30s --timeout=5s --start-period=15s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "EMK.Save.API.dll"]
