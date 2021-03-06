FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["AzurePlayground.Price.Service/AzurePlayground.Price.Service.csproj", "AzurePlayground.Price.Service/"]
COPY ["AzurePlayground.Trade.Service.Shared/AzurePlayground.Trade.Service.Shared.csproj", "AzurePlayground.Trade.Service.Shared/"]
COPY ["Modules/Dasein.Core.Lite/Dasein.Core.Lite.Shared/Dasein.Core.Lite.Shared.csproj", "Modules/Dasein.Core.Lite/Dasein.Core.Lite.Shared/"]
COPY ["Modules/Dasein.Core.Lite/Dasein.Core.Lite/Dasein.Core.Lite.csproj", "Modules/Dasein.Core.Lite/Dasein.Core.Lite/"]
RUN dotnet restore "AzurePlayground.Price.Service/AzurePlayground.Price.Service.csproj"
COPY . .
WORKDIR "/src/AzurePlayground.Price.Service"
RUN dotnet build "AzurePlayground.Price.Service.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "AzurePlayground.Price.Service.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .

RUN cp -a ./certs/. /usr/local/share/ca-certificates/
RUN update-ca-certificates
ENV ASPNETCORE_Kestrel__Certificates__Default__Path price.pfx

ENTRYPOINT ["dotnet", "AzurePlayground.Price.Service.dll"]