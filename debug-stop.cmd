taskkill /IM consul.exe /F

powershell docker kill $(docker ps -a -q)
powershell docker rm $(docker ps -a -q)

powershell "Stop-Process -processname dotnet"