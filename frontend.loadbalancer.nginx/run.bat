echo off
REM -d is for detaching (run container in background)
REM -it is for attaching stdin and a terminal
REM -p is for port mapping
REM docker run --name nginxfront_1 -d frontend-lb
REM docker run --name nginxfront_1 -it frontend-lb
echo on
docker run -p 8080:80 --rm -d frontendlbdev