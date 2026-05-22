FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ProjectTaskManagement.sln ./
COPY src/ProjectTaskManagement.Domain/ProjectTaskManagement.Domain.csproj src/ProjectTaskManagement.Domain/
COPY src/ProjectTaskManagement.Application/ProjectTaskManagement.Application.csproj src/ProjectTaskManagement.Application/
COPY src/ProjectTaskManagement.Infrastructure/ProjectTaskManagement.Infrastructure.csproj src/ProjectTaskManagement.Infrastructure/
COPY src/ProjectTaskManagement.API/ProjectTaskManagement.API.csproj src/ProjectTaskManagement.API/

RUN dotnet restore src/ProjectTaskManagement.API/ProjectTaskManagement.API.csproj

COPY . .
RUN dotnet publish src/ProjectTaskManagement.API/ProjectTaskManagement.API.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Docker

EXPOSE 8080
ENTRYPOINT ["dotnet", "ProjectTaskManagement.API.dll"]
