FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

ENV ASPNETCORE_ENVIRONMENT=Uat
ENV ASPNETCORE_URLS=https://+:443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Mandara.ConfigurationService/Mandara.ConfigurationService.csproj", "Mandara.ConfigurationService/"]

RUN dotnet restore "Mandara.ConfigurationService/Mandara.ConfigurationService.csproj"
COPY . .
WORKDIR "/src/Mandara.ConfigurationService"
RUN dotnet build "Mandara.ConfigurationService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Mandara.ConfigurationService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Mandara.ConfigurationService.dll"]