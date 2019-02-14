FROM microsoft/dotnet:2.2-sdk-alpine AS builder
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o out

# Build runtime image
FROM microsoft/dotnet:2.2.2-aspnetcore-runtime-alpine AS build-env
WORKDIR /app
COPY --from=builder /app/out .

ENTRYPOINT ["dotnet", "DnxWebApi.OnDocker.dll"]
#CMD ["dotnet", "DnxWebApi.OnDocker.dll"]