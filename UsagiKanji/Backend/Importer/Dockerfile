# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files and restore
COPY ["Importer/Importer.csproj", "Importer/"]
COPY ["Application/Application.csproj", "Application/"]
COPY ["Domain/Domain.csproj", "Domain/"]
COPY ["Infrastructure/Infrastructure.csproj", "Infrastructure/"]
RUN dotnet restore "Importer/Importer.csproj"

# Copy all source code
COPY . .

# Publish the app
WORKDIR "/src/Importer"
RUN dotnet publish "Importer.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copy the published app
COPY --from=build /app/publish .

# === NEW: Explicitly copy the files Importer needs ===
# Copy appsettings.json
COPY Importer/appsettings.json .

# Copy the entire Data folder with JSON files
COPY Importer/Data ./Data

# Entry point
ENTRYPOINT ["dotnet", "Importer.dll"]