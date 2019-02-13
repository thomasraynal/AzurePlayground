cd..

powershell docker kill $(docker ps -a -q)
powershell docker rm $(docker ps -a -q)

docker pull eventstore/eventstore:latest
docker pull mongo:latest
docker pull consul

dotnet publish ./AzurePlayground.Authentication -c Release -o bin/publish
dotnet publish ./AzurePlayground.Trade.Service -c Release -o bin/publish
dotnet publish ./AzurePlayground.Web.App -c Release -o bin/publish
dotnet publish ./AzurePlayground.Compliance.Service -c Release -o bin/publish
dotnet publish ./AzurePlayground.Gateway -c Release -o bin/publish
dotnet publish ./AzurePlayground.Market.Service -c Release -o bin/publish
dotnet publish ./AzurePlayground.Price.Service -c Release -o bin/publish
dotnet publish ./AzurePlayground.Trade.Event.Service -c Release -o bin/publish
dotnet publish ./AzurePlayground.Trade.Generator -c Release -o bin/publish

docker-compose -f docker-compose.yml -f docker-compose.override.local.yml build

pause