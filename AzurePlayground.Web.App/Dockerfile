FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["AzurePlayground.Web.App/AzurePlayground.Web.App.csproj", "AzurePlayground.Web.App/"]
COPY ["AzurePlayground.Trade.Service.Shared/AzurePlayground.Trade.Service.Shared.csproj", "AzurePlayground.Trade.Service.Shared/"]
COPY ["Modules/Dasein.Core.Lite/Dasein.Core.Lite.Shared/Dasein.Core.Lite.Shared.csproj", "Modules/Dasein.Core.Lite/Dasein.Core.Lite.Shared/"]
COPY ["Modules/Dasein.Core.Lite/Dasein.Core.Lite/Dasein.Core.Lite.csproj", "Modules/Dasein.Core.Lite/Dasein.Core.Lite/"]
RUN dotnet restore "AzurePlayground.Web.App/AzurePlayground.Web.App.csproj"
COPY . .
WORKDIR "/src/AzurePlayground.Web.App"
RUN dotnet build "AzurePlayground.Web.App.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "AzurePlayground.Web.App.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .

RUN cp -a ./certs/. /usr/local/share/ca-certificates/
RUN update-ca-certificates
ENV ASPNETCORE_Kestrel__Certificates__Default__Path app.pfx

ENTRYPOINT ["dotnet", "AzurePlayground.Web.App.dll"]