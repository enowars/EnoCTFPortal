# Build Backend Portal
FROM mcr.microsoft.com/dotnet/sdk:5.0-focal AS backend-build
WORKDIR /src
# TODO: Update build

# Fetch deps
COPY EnoLandingPage.sln EnoLandingPage.sln
COPY EnoLandingPageBackend/EnoLandingPageBackend.csproj EnoLandingPageBackend/EnoLandingPageBackend.csproj
RUN dotnet restore

# Publish
COPY EnoLandingPageBackend EnoLandingPageBackend
COPY Directory.Build.props Directory.Build.props
COPY ENOWARS.ruleset ENOWARS.ruleset
COPY stylecop.json stylecop.json
RUN dotnet publish -c Release -o /app

# Build Frontend
FROM node:12.7-alpine AS build
WORKDIR /usr/src/app
COPY package.json package-lock.json ./
RUN npm install
COPY EnoLandingPageBackend/ClientApp .
RUN npm run build


# Copy to runtime container
FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
EXPOSE 80
COPY --from=backend-build /app .
COPY --from=build /usr/src/app/dist /ClientApp
COPY EnoLandingPageBackend/appsettings.json .
ENTRYPOINT ["dotnet", "EnoLandingPageBackend.dll"]
