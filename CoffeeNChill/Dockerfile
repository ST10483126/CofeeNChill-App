# Uses the .NET 10 SDK to build the project
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

# Sets the working folder inside the build container
WORKDIR /src

# Copies the project file and restores the packages
COPY CoffeeNChill.csproj ./
RUN dotnet restore CoffeeNChill.csproj

# Copies the rest of the project files
COPY . ./

# Publishes the project without restoring the packages again
RUN dotnet publish CoffeeNChill.csproj \
    --configuration Release \
    --output /home/site/wwwroot \
    --no-restore

# Uses the official Azure Functions runtime for .NET 10
FROM mcr.microsoft.com/azure-functions/dotnet-isolated:4-dotnet-isolated10.0

# Sets the required Azure Functions container settings
ENV AzureWebJobsScriptRoot=/home/site/wwwroot \
    AzureFunctionsJobHost__Logging__Console__IsEnabled=true \
    FUNCTIONS_WORKER_RUNTIME=dotnet-isolated

# Copies the published project into the Functions container
COPY --from=build /home/site/wwwroot /home/site/wwwroot