FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY Week10/Gateway/Gateway.csproj Week10/Gateway/
RUN dotnet restore Week10/Gateway/Gateway.csproj
COPY Week10/Gateway/ Week10/Gateway/
WORKDIR /src/Week10/Gateway
RUN dotnet publish -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "Gateway.dll"]
