# Stage 1: build the application
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution file
COPY AuthTask.slnx ./

# Copy project files (for caching restore)
COPY AuthTask/AuthTask.csproj AuthTask/
COPY AuthTask.Application/AuthTask.Application.csproj AuthTask.Application/
COPY AuthTask.Domain/AuthTask.Domain.csproj AuthTask.Domain/
COPY AuthTask.Infrastructure/AuthTask.Infrastructure.csproj AuthTask.Infrastructure/

# Restore dependencies
RUN dotnet restore AuthTask/AuthTask.csproj
RUN dotnet restore AuthTask.Application/AuthTask.Application.csproj
RUN dotnet restore AuthTask.Domain/AuthTask.Domain.csproj
RUN dotnet restore AuthTask.Infrastructure/AuthTask.Infrastructure.csproj

# Copy the rest of the source code
COPY . .

# Publish the application
WORKDIR /src/AuthTask
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: create the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Development
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT [ "dotnet", "AuthTask.dll" ]