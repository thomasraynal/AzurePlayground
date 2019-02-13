FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["AzurePlayground.Gateway/AzurePlayground.Gateway.csproj", "AzurePlayground.Gateway/"]
RUN dotnet restore "AzurePlayground.Gateway/AzurePlayground.Gateway.csproj"
COPY . .
WORKDIR "/src/AzurePlayground.Gateway"
RUN dotnet build "AzurePlayground.Gateway.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "AzurePlayground.Gateway.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .

RUN cp -a ./certs/. /usr/local/share/ca-certificates/
RUN update-ca-certificates
ENV ASPNETCORE_Kestrel__Certificates__Default__Path gateway.pfx

ENTRYPOINT ["dotnet", "AzurePlayground.Gateway.dll"]