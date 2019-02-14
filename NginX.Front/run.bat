echo off
REM -d is for detaching (run container in bg)
REM -p is for port mapping
REM docker run --name nginxfront_1 -d nginxfront
REM docker run --name nginxfront_1 -it nginxfront
echo on
docker run -p 8080:80 --name nginxfront_1 -d nginxfront