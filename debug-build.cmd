
docker pull eventstore/eventstore:latest
docker pull mongo bitnami/mongodb:latest

dotnet publish --configuration Debug ./AzurePlayground.Authentication
dotnet publish --configuration Debug ./AzurePlayground.Gateway
dotnet publish --configuration Debug ./AzurePlayground.Trade.Generator
dotnet publish --configuration Debug ./AzurePlayground.Trade.Service
dotnet publish --configuration Debug ./AzurePlayground.Market.Service
dotnet publish --configuration Debug ./AzurePlayground.Compliance.Service
dotnet publish --configuration Debug ./AzurePlayground.Price.Service

pause