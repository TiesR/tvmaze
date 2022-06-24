#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["TvMazeWebApi/TvMazeWebApi.csproj", "TvMazeWebApi/"]
RUN dotnet restore "TvMazeWebApi/TvMazeWebApi.csproj"
COPY . .
WORKDIR "/src/TvMazeWebApi"
RUN dotnet build "TvMazeWebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TvMazeWebApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TvMazeWebApi.dll"]