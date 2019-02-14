echo off
REM -d is for detaching (run container in bg)
REM -p is for port mapping
echo on
docker run -t --name nginxfront_app -d nginxfront