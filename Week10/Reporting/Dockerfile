FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY Week10/Reporting/Reporting.csproj Week10/Reporting/
RUN dotnet restore Week10/Reporting/Reporting.csproj
COPY Week10/Reporting/ Week10/Reporting/
WORKDIR /src/Week10/Reporting
RUN dotnet publish -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "Reporting.dll"]
