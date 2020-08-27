# Hent fuldt .NET SDK
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /app

# Kopier .csproj-fil og genskab afhængigheder
COPY *.csproj ./
RUN dotnet restore

# Kopier projektfiler og byg release
COPY . ./
RUN dotnet publish -c Release -o out

# Generer runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
EXPOSE 80
COPY --from=build-env /app/out .
ENTRYPOINT [ "dotnet", "AuthServer.dll" ]