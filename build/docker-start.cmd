cd..

powershell docker kill $(docker ps -a -q)
powershell docker rm $(docker ps -a -q)

start /d "." docker-compose -f docker-compose.yml -f docker-compose.override.local.yml up
 
pause