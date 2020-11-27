#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["CFSh/CFSh-backend/CFSh-backend.csproj", "CFSh/CFSh-backend/"]
RUN dotnet restore "CFSh/CFSh-backend/CFSh-backend.csproj"
COPY . .
WORKDIR "/src/CFSh/CFSh-backend"
RUN dotnet build "CFSh-backend.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CFSh-backend.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CFSh-backend.dll"]