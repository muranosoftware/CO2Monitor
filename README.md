# CO2Monitor

Docker image building:
docker build -t docker-hub-user/docker-hub-repo:latest .

Get prebuild image:
docker pull alexandrzhuravlevmuranosoft/co2monitor

Run docker  prebuild image:
docker run -p 127.0.0.1:80:80 alexandrzhuravlevmuranosoft/co2monitor:latest

Open browser localhost:80/swagger