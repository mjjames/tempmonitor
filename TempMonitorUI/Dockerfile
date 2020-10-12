#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim-arm32v7 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["TempMonitorUI/TempMonitorUI.csproj", "TempMonitorUI/"]
RUN dotnet restore "TempMonitorUI/TempMonitorUI.csproj"
COPY . .
WORKDIR "/src/TempMonitorUI"
RUN dotnet build "TempMonitorUI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TempMonitorUI.csproj" -c Release -o /app/publish -r linux-arm

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TempMonitorUI.dll"]