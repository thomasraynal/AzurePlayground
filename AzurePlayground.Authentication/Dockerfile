FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["AzurePlayground.Authentication/AzurePlayground.Authentication.csproj", "AzurePlayground.Authentication/"]
COPY ["AzurePlayground.Authentication.Shared/AzurePlayground.Authentication.Shared.csproj", "AzurePlayground.Authentication.Shared/"]
COPY ["AzurePlayground.Trade.Service.Shared/AzurePlayground.Trade.Service.Shared.csproj", "AzurePlayground.Trade.Service.Shared/"]
COPY ["Modules/Dasein.Core.Lite/Dasein.Core.Lite.Shared/Dasein.Core.Lite.Shared.csproj", "Modules/Dasein.Core.Lite/Dasein.Core.Lite.Shared/"]
COPY ["AzurePlayground.Persistence.Mongo/AzurePlayground.Persistence.Mongo.csproj", "AzurePlayground.Persistence.Mongo/"]
COPY ["AzurePlayground.Persistence/AzurePlayground.Persistence.csproj", "AzurePlayground.Persistence/"]
COPY ["Modules/Dasein.Core.Lite/Dasein.Core.Lite/Dasein.Core.Lite.csproj", "Modules/Dasein.Core.Lite/Dasein.Core.Lite/"]
RUN dotnet restore "AzurePlayground.Authentication/AzurePlayground.Authentication.csproj"
COPY . .
WORKDIR "/src/AzurePlayground.Authentication"
RUN dotnet build "AzurePlayground.Authentication.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "AzurePlayground.Authentication.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .

RUN cp -a ./certs/. /usr/local/share/ca-certificates/
RUN update-ca-certificates

ENV ASPNETCORE_Kestrel__Certificates__Default__Path authentication.pfx

ENTRYPOINT ["dotnet", "AzurePlayground.Authentication.dll"]