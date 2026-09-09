using Azure.Data.Tables;
using Azure.Monitor.OpenTelemetry.Exporter;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Azure.Functions.Worker.OpenTelemetry;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        // Gets the storage connection string from your local configurations
        string connectionString = Environment.GetEnvironmentVariable("CoffeeNChillStorageConnection")
            ?? "UseDevelopmentStorage=true";

        // Connects to the service client and automatically ensure the table exists
        var tableServiceClient = new TableServiceClient(connectionString);
        var tableClient = tableServiceClient.GetTableClient("MenuItems");
        tableClient.CreateIfNotExists();

        // Registser the table client into dependency injection so the endpoints can use it
        services.AddSingleton(tableClient);
    })
    .Build();

host.Run();