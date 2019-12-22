FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /src

# copy csproj and restore as distinct layers
COPY src/*.csproj ./
#COPY ui/*.csproj ./utils/
# WORKDIR /src
RUN dotnet restore

# copy and publish app and libraries
# WORKDIR /src
COPY ./src/. ./
#COPY utils/. ./utils/
#WORKDIR /src
RUN dotnet publish -c Release -o out

#FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS runtime
FROM build-env
#WORKDIR /src
COPY --from=build-env /src/out ./
ENTRYPOINT ["dotnet", "joked.dll"]