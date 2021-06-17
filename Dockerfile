# Build Backend Portal
FROM mcr.microsoft.com/dotnet/sdk:5.0-focal AS backend-build
RUN curl -sL https://deb.nodesource.com/setup_14.x |  bash -
RUN apt-get install -y nodejs
WORKDIR /src
ARG NO_WEB_APP=true
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

# Build Frontend in extra storage for improved caching
FROM node:14 AS build
WORKDIR /usr/src/app
COPY EnoLandingPageBackend/ClientApp/package.json EnoLandingPageBackend/ClientApp/package-lock.json ./
RUN npm install
COPY EnoLandingPageBackend/ClientApp .
RUN npm run build


# Build runtime container
FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app

# Create data folder if not run as mapped volume
RUN mkdir ./data
EXPOSE 80

# Copy files from other stages
COPY --from=backend-build /app .
COPY --from=build /usr/src/app/dist /app/ClientApp/dist
COPY EnoLandingPageBackend/appsettings.json .
ENTRYPOINT ["dotnet", "EnoLandingPageBackend.dll"]