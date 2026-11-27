FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY Week10/Academics/Academics.csproj Week10/Academics/
RUN dotnet restore Week10/Academics/Academics.csproj
COPY Week10/Academics/ Week10/Academics/
WORKDIR /src/Week10/Academics
RUN dotnet publish -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "Academics.dll"]
