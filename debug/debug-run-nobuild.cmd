cd..

taskkill /IM consul.exe /F
start /d "." ./Modules/consul/consul agent -dev

powershell docker kill $(docker ps -a -q)
powershell docker rm $(docker ps -a -q)

start /d "." docker run -p 1113:1113 -p 2113:2113 -it --rm --name eventstore eventstore/eventstore:latest
start /d "." docker run -p 27017:27017 -it --rm --name mongo mongo:latest

timeout 10

start /d "." dotnet run --configuration Debug --no-build --project ./AzurePlayground.Authentication
start /d "." dotnet run --configuration Debug --no-build --project ./AzurePlayground.Gateway
start /d "." dotnet run --configuration Debug --no-build --project ./AzurePlayground.Price.Service
start /d "." dotnet run --configuration Debug --no-build --project ./AzurePlayground.Trade.Event.Service
start /d "." dotnet run --configuration Debug --no-build --project ./AzurePlayground.Trade.Service	
start /d "." dotnet run --configuration Debug --no-build --project ./AzurePlayground.Market.Service
start /d "." dotnet run --configuration Debug --no-build --project ./AzurePlayground.Compliance.Service

timeout 10

start /d "." dotnet run --configuration Debug --no-build --project AzurePlayground.Trade.Generator
start /d "." dotnet run --configuration Debug --no-build --project AzurePlayground.Web.App

pause