FROM microsoft/dotnet:2.2-sdk-alpine AS builder
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
#RUN dotnet publish -c Release -o out
RUN dotnet publish -c Release -o out frontend.values.web.csproj


# Build runtime image
FROM microsoft/dotnet:2.2.2-aspnetcore-runtime-alpine AS build-env
WORKDIR /app
COPY --from=builder /app/out .

# Entrypoint runs this by default, but also allows additional exec commands whereas CMD seems more limited
ENTRYPOINT ["dotnet", "frontend.values.web.dll"]