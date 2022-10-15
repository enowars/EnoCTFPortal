FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Fetch deps
COPY EnoLandingPage.sln EnoLandingPage.sln
COPY EnoLandingPageBackend/EnoLandingPageBackend.csproj EnoLandingPageBackend/EnoLandingPageBackend.csproj
COPY EnoLandingPageCore/EnoLandingPageCore.csproj EnoLandingPageCore/EnoLandingPageCore.csproj
COPY EnoLandingPageFrontend/EnoLandingPageFrontend.csproj EnoLandingPageFrontend/EnoLandingPageFrontend.csproj
RUN dotnet restore

# Publish
COPY EnoLandingPageBackend EnoLandingPageBackend
COPY EnoLandingPageCore EnoLandingPageCore
COPY EnoLandingPageFrontend EnoLandingPageFrontend
COPY Directory.Build.props Directory.Build.props
COPY ENOWARS.ruleset ENOWARS.ruleset
COPY stylecop.json stylecop.json
RUN dotnet publish -c Release -o /app

# Copy to runtime container
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
EXPOSE 80
COPY --from=build /app .
COPY EnoLandingPageBackend/appsettings.json .
ENTRYPOINT ["dotnet", "EnoLandingPageBackend.dll"]
