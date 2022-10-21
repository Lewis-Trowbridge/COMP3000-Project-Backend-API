FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /source

COPY . .

RUN [ "dotnet", "restore", "/source/COMP3000-Project-Backend-API/COMP3000-Project-Backend-API" ]

WORKDIR /source/COMP3000-Project-Backend-API
RUN [ "dotnet", "publish", "-c", "Release", "-o", "/app", "--no-restore" ]

FROM mcr.microsoft.com/dotnet/aspnet:6.0

WORKDIR /app
COPY --from=build /app .

ENV ASPNETCORE_URLS http://+

ENTRYPOINT [ "/app/COMP3000-Project-Backend-API" ]
