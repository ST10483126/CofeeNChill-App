# Uses the .NET 10 SDK to build the project
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

# Sets the working folder inside the build container
WORKDIR /src

# Copies the project file first and restores the required packages
COPY CoffeeNChill.csproj ./
RUN dotnet restore CoffeeNChill.csproj

# Copies the rest of the project files
COPY . ./

# Publishes the project in Release mode
RUN dotnet publish CoffeeNChill.csproj \
    --configuration Release \
    --output /home/site/wwwroot

# Uses the official Azure Functions runtime for .NET 10
FROM mcr.microsoft.com/azure-functions/dotnet-isolated:4-dotnet-isolated10.0

# Tells Azure Functions where the published files are located
# Also enables console logs so we can check the container output
ENV AzureWebJobsScriptRoot=/home/site/wwwroot \
    AzureFunctionsJobHost__Logging__Console__IsEnabled=true

# Copies the published application into the final container
COPY --from=build /home/site/wwwroot /home/site/wwwroot