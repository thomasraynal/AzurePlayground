docker kill mongo
docker kill authentication
docker kill trades
docker kill app

start /d "." docker run --expose=27017 --network="host" -it --rm --name mongo bitnami/mongodb:latest

dotnet publish ./AzurePlayground.Authentication -c Release -o bin/publish
dotnet publish ./AzurePlayground.Trade.Service -c Release -o bin/publish
dotnet publish ./AzurePlayground.Web.App -c Release -o bin/publish

docker build -t azureplayground.authentication ./AzurePlayground.Authentication
docker build -t azureplayground.trade.service ./AzurePlayground.Trade.Service
docker build -t azureplayground.web.app ./AzurePlayground.Web.App

start /d "." docker run --expose=5000 -e ASPNETCORE_URLS=http://*:5000 --network="host" -it --rm --name trades azureplayground.trade.service
start /d "." docker run --expose=5001 -e ASPNETCORE_URLS=http://*:5001 --network="host" -it --rm --name authentication azureplayground.authentication
start /d "." docker run --expose=5002 -e ASPNETCORE_URLS=http://*:5002 --network="host" -it --rm --name app azureplayground.web.app

pause