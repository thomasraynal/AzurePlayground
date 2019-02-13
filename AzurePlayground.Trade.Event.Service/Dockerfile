FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["AzurePlayground.Trade.Event.Service/AzurePlayground.Trade.Event.Service.csproj", "AzurePlayground.Trade.Event.Service/"]
COPY ["AzurePlayground.Trade.Service.Shared/AzurePlayground.Trade.Service.Shared.csproj", "AzurePlayground.Trade.Service.Shared/"]
COPY ["Modules/Dasein.Core.Lite/Dasein.Core.Lite.Shared/Dasein.Core.Lite.Shared.csproj", "Modules/Dasein.Core.Lite/Dasein.Core.Lite.Shared/"]
COPY ["Modules/Dasein.Core.Lite/Dasein.Core.Lite/Dasein.Core.Lite.csproj", "Modules/Dasein.Core.Lite/Dasein.Core.Lite/"]
COPY ["AzurePlayground.Events.EventStore/AzurePlayground.Events.EventStore.csproj", "AzurePlayground.Events.EventStore/"]
RUN dotnet restore "AzurePlayground.Trade.Event.Service/AzurePlayground.Trade.Event.Service.csproj"
COPY . .
WORKDIR "/src/AzurePlayground.Trade.Event.Service"
RUN dotnet build "AzurePlayground.Trade.Event.Service.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "AzurePlayground.Trade.Event.Service.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .

RUN cp -a ./certs/. /usr/local/share/ca-certificates/
RUN update-ca-certificates
ENV ASPNETCORE_Kestrel__Certificates__Default__Path event.pfx

ENTRYPOINT ["dotnet", "AzurePlayground.Trade.Event.Service.dll"]