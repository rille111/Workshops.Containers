echo off
REM -d is for detaching (run container in bg)
REM -p is for port mapping
REM docker run --name nginxfront_debug -d nginxfront
REM docker run --name nginxfront_debug -it nginxfront
REM docker run -it nginxfront
echo on
docker run --name nginxfront_debug -it nginxfront