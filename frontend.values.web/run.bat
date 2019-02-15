echo off
REM -d is for detaching (run container in bg)
REM -p is for port mapping
REM docker run -it --rm -p 5000:80 dnxweb
REM docker run -g -p 5000:80 dnxweb
echo on
docker run -it -p 3000:80 frontweb