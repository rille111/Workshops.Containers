echo off
REM -d is for detaching (run container in background)
REM -it is for attaching stdin and a terminal
REM -p is for port mapping
REM docker run --name nginxfront_1 -d nginxfront
REM docker run --name nginxfront_1 -it nginxfront
echo on
docker run -p 3050:80 --name nginxfront_name -d nginxfront