echo off
REM -d is for detaching (run container in bg)
REM -p is for port mapping
echo on
docker run -it --rm -p 8080:80 --name dnxweb_app dnxweb