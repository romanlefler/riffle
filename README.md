# Riffle

Riffle uses a C# ASP.NET 9.0 MVC backend.
The frontend is scripts built using Typescript and Vite with Node.JS.

## Run Debug Dev Server

```shell
dotnet run --project ./Riffle --launch-profile "https"
```

The backend is found in `Riffle/` with unit tests in `RiffleTest/`.

## Frontend Scripts

Frontend scripts are found in `RiffleFrontend/` and are
automatically built when the backend is run.

## SignalR Client/Server Interactions

The frontend scripts and the backend use SignalR to interact
with each other.

Methods used can be found in [HubMethods.md](./HubMethods.md).
