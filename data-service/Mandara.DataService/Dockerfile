FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

ENV ASPNETCORE_ENVIRONMENT=Uat
ENV ASPNETCORE_URLS=https://+:443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Mandara.DataService/Mandara.DataService.csproj", "Mandara.DataService/"]

RUN dotnet restore "Mandara.DataService/Mandara.DataService.csproj"
COPY . .
WORKDIR "/src/Mandara.DataService"
RUN dotnet build "Mandara.DataService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Mandara.DataService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Mandara.DataService.dll"]