taskkill /IM consul.exe /F
start /d "." ./consul/consul agent -dev

powershell docker kill $(docker ps -a -q)
powershell docker rm $(docker ps -a -q)

start /d "." docker run -p 1113:1113 -p 2113:2113 -it --rm --name eventstore eventstore/eventstore:latest
start /d "." docker run -p 27017:27017 -it --rm --name mongo bitnami/mongodb:latest

dotnet publish --configuration Debug ./AzurePlayground.Authentication
dotnet publish --configuration Debug ./AzurePlayground.Gateway
dotnet publish --configuration Debug ./AzurePlayground.Trade.Generator
dotnet publish --configuration Debug ./AzurePlayground.Trade.Service
dotnet publish --configuration Debug ./AzurePlayground.Market.Service
dotnet publish --configuration Debug ./AzurePlayground.Compliance.Service
dotnet publish --configuration Debug ./AzurePlayground.Price.Service

start /d "." dotnet run --configuration Debug --no-build --project ./AzurePlayground.Authentication
timeout 2
start /d "." dotnet run --configuration Debug --no-build --project ./AzurePlayground.Gateway
timeout 2
start /d "." dotnet run --configuration Debug --no-build --project ./AzurePlayground.Price.Service
timeout 2
start /d "." dotnet run --configuration Debug --no-build --project ./AzurePlayground.Trade.Service
timeout 2	
start /d "." dotnet run --configuration Debug --no-build --project ./AzurePlayground.Market.Service
timeout 2
start /d "." dotnet run --configuration Debug --no-build --project ./AzurePlayground.Compliance.Service

timeout 10
start /d "." dotnet run --configuration Debug --no-build --project AzurePlayground.Trade.Generator

pause