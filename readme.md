## ASP.NET Core on Containers

Building a multicontainer application with nginx at front, proxying requests to a aspnetcore backend.

* Using a linux image (alpine) with .net core sdk to build the app
* Using another linux image (alpine) with .net core runtime to run the app 
* Nginx at the front (port 3050)  (image busybox)
* the aspnetcore website (port 5000) and has one route, just /.

## Starting - how to

Since it's a multicontainer setup docker-compose is needed.
`docker-compose up --build`
You can also run the containers individually with their respective build.bat and run.bat files.

## Resources

* Udemy course docker: https://www.udemy.com/docker-and-kubernetes-the-complete-guide/
* Microsoft tutorial: https://docs.microsoft.com/en-us/dotnet/core/docker/building-net-docker-images
* To override ports in aspnetcore: https://stackoverflow.com/questions/48669548/why-does-aspnet-core-start-on-port-80-from-within-docker
* Github for config aspnetcore and docker https://github.com/dotnet/dotnet-docker/blob/master/samples/aspnetapp/aspnetcore-docker-windows.md
* Github sample aspnetapp for docker https://github.com/dotnet/dotnet-docker/tree/master/samples/aspnetapp
* Combine nginx and aspnetcore https://stackoverflow.com/questions/51264577/install-nginx-on-an-existing-asp-net-core-docker-container
* Some nginx upstream examples http://nginx.org/en/docs/http/ngx_http_upstream_module.html#example
* Nginx config beginners guide: http://nginx.org/en/docs/beginners_guide.html#conf_structure
* Nginx docker images: https://hub.docker.com/_/nginx