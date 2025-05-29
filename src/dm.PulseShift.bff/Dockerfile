# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
# Atualizar certificados (pode variar dependendo da distribuição Linux da imagem base)
RUN apt-get update && apt-get install -y --no-install-recommends ca-certificates
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/dm.PulseShift.bff/dm.PulseShift.bff.csproj", "src/dm.PulseShift.bff/"]
COPY ["src/dm.PulseShift.Application/dm.PulseShift.Application.csproj", "src/dm.PulseShift.Application/"]
COPY ["src/dm.PulseShift.Infra.CrossCutting.IoC/dm.PulseShift.Infra.CrossCutting.IoC.csproj", "src/dm.PulseShift.Infra.CrossCutting.IoC/"]
COPY ["src/dm.PulseShift.Infra.CrossCutting.Shared/dm.PulseShift.Infra.CrossCutting.Shared.csproj", "src/dm.PulseShift.Infra.CrossCutting.Shared/"]
RUN dotnet restore "./src/dm.PulseShift.bff/dm.PulseShift.bff.csproj"
COPY . .
WORKDIR "/src/src/dm.PulseShift.bff"
RUN dotnet build "./dm.PulseShift.bff.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./dm.PulseShift.bff.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "dm.PulseShift.bff.dll"]