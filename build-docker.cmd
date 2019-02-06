docker kill mongo
docker kill authentication
docker kill app
docker kill trades

dotnet publish ./AzurePlayground.Authentication -c Release -o bin/publish
dotnet publish ./AzurePlayground.Trade.Service -c Release -o bin/publish
dotnet publish ./AzurePlayground.Web.App -c Release -o bin/publish

docker-compose build authentication
docker-compose build app
docker-compose build trades

pause