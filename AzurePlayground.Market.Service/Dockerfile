FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["AzurePlayground.Market.Service/AzurePlayground.Market.Service.csproj", "AzurePlayground.Market.Service/"]
COPY ["AzurePlayground.Trade.Service.Shared/AzurePlayground.Trade.Service.Shared.csproj", "AzurePlayground.Trade.Service.Shared/"]
COPY ["Modules/Dasein.Core.Lite/Dasein.Core.Lite.Shared/Dasein.Core.Lite.Shared.csproj", "Modules/Dasein.Core.Lite/Dasein.Core.Lite.Shared/"]
COPY ["Modules/Dasein.Core.Lite/Dasein.Core.Lite/Dasein.Core.Lite.csproj", "Modules/Dasein.Core.Lite/Dasein.Core.Lite/"]
COPY ["AzurePlayground.Events.EventStore/AzurePlayground.Events.EventStore.csproj", "AzurePlayground.Events.EventStore/"]
RUN dotnet restore "AzurePlayground.Market.Service/AzurePlayground.Market.Service.csproj"
COPY . .
WORKDIR "/src/AzurePlayground.Market.Service"
RUN dotnet build "AzurePlayground.Market.Service.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "AzurePlayground.Market.Service.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .

RUN cp -a ./certs/. /usr/local/share/ca-certificates/
RUN update-ca-certificates
ENV ASPNETCORE_Kestrel__Certificates__Default__Path market.pfx

ENTRYPOINT ["dotnet", "AzurePlayground.Market.Service.dll"]