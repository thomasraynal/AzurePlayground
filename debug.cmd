dotnet publish --configuration Debug ./AzurePlayground.Authentication
dotnet publish --configuration Debug ./AzurePlayground.Gateway
dotnet publish --configuration Debug ./AzurePlayground.Trade.Generator
dotnet publish --configuration Debug ./AzurePlayground.Trade.Service
dotnet publish --configuration Debug ./AzurePlayground.Market.Service
dotnet publish --configuration Debug ./AzurePlayground.Compliance.Service

start /d "." dotnet run --no-build --project ./AzurePlayground.Authentication
timeout 2
start /d "." dotnet run --no-build --project ./AzurePlayground.Gateway
timeout 2
start /d "." dotnet run --no-build --project AzurePlayground.Trade.Generator
timeout 2
start /d "." dotnet run --no-build --project ./AzurePlayground.Trade.Service
timeout 2
start /d "." dotnet run --no-build --project ./AzurePlayground.Market.Service
timeout 2
start /d "." dotnet run --no-build --project ./AzurePlayground.Compliance.Service

pause