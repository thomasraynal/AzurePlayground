cd..

docker pull eventstore/eventstore:latest
docker pull mongo:latest
docker pull consul

dotnet publish --configuration Debug ./AzurePlayground.Authentication
dotnet publish --configuration Debug ./AzurePlayground.Gateway
dotnet publish --configuration Debug ./AzurePlayground.Trade.Generator
dotnet publish --configuration Debug ./AzurePlayground.Trade.Service
dotnet publish --configuration Debug ./AzurePlayground.Market.Service
dotnet publish --configuration Debug ./AzurePlayground.Compliance.Service
dotnet publish --configuration Debug ./AzurePlayground.Price.Service
dotnet publish --configuration Debug ./AzurePlayground.Trade.Event.Service
dotnet publish --configuration Debug ./AzurePlayground.Web.App

pause