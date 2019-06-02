FROM microsoft/dotnet:2.2-runtime AS base
WORKDIR /app

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["DMX.NETCore.Server.csproj", "./"]
RUN dotnet restore "./DMX.NETCore.Server.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "DMX.NETCore.Server.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "DMX.NETCore.Server.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "DMX.NETCore.Server.dll"]
