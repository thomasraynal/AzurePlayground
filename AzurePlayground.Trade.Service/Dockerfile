FROM microsoft/dotnet:2.2-sdk 
COPY ./bin/publish /app
WORKDIR /app
ENTRYPOINT ["dotnet", "AzurePlayground.Trade.Service.dll"]
