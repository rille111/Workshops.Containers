echo off
REM -d is for detaching (run container in bg)
REM -p is for port mapping
REM docker run -it --rm -p 8081:8081 --name dnxweb_app dnxweb
REM docker run -it --rm --name dnxweb_app dnxweb
echo on
docker run -d --name dnxweb_debug dnxweb