powershell docker kill $(docker ps -a -q)
powershell docker rm $(docker ps -a -q)

start /d "." docker-compose up

pause