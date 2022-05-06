FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

WORKDIR /source

COPY src/ .

RUN dotnet publish Andtech.Codecast.Console/Andtech.Codecast.Console.csproj -c release -o /app

FROM mcr.microsoft.com/dotnet/runtime:6.0
WORKDIR /app
COPY --from=build /app ./

EXPOSE 8080
ENTRYPOINT ["/app/Andtech.Codecast.Console"]
