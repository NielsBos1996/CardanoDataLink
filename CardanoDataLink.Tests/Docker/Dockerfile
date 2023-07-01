FROM mcr.microsoft.com/dotnet/sdk:7.0
WORKDIR /app

# Copy the main project
COPY . .

# Run the tests
ENTRYPOINT ["dotnet", "test"]
