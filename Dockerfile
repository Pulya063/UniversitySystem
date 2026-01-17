# Build + publish the .NET 8 console app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

# Copy the entire project folder (ensures all files referenced by the csproj are present)
COPY . .

# Restore and build
RUN dotnet restore UniversitySystem.csproj
RUN dotnet build -c Release -o /app/build
RUN dotnet publish -c Release -o /app/publish --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS runtime
WORKDIR /app

# Copy published app
COPY --from=build /app/publish .

# Set entrypoint
ENTRYPOINT ["dotnet", "UniversitySystem.dll"]