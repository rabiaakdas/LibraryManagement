FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["LibraryManagement.sln", "./"]
COPY ["LibraryManagement.Web/LibraryManagement.Web.csproj", "LibraryManagement.Web/"]
COPY ["LibraryManagement.Tests/LibraryManagement.Tests.csproj", "LibraryManagement.Tests/"]
RUN dotnet restore "LibraryManagement.sln"

COPY . .
RUN dotnet build "LibraryManagement.sln" -c Release --no-restore

FROM build AS publish
RUN dotnet publish "LibraryManagement.Web/LibraryManagement.Web.csproj" -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LibraryManagement.Web.dll"]
