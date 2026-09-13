using Azure.Data.Tables;
using Azure.Storage.Blobs;// Added for Blob Storage container initialization
using Azure.Monitor.OpenTelemetry.Exporter;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Azure.Functions.Worker.OpenTelemetry;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry;
using System;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        // Gets the storage connection string from your local configurations
        string connectionString = Environment.GetEnvironmentVariable("CoffeeNChillStorageConnection")
            ?? "UseDevelopmentStorage=true";

        // --- TABLE STORAGE SETUP ---
        // Connects to the service client and automatically ensures the table exists
        var tableServiceClient = new TableServiceClient(connectionString);
        var tableClient = tableServiceClient.GetTableClient("MenuItems");
        tableClient.CreateIfNotExists();

        // Register the table client into dependency injection so the endpoints can use it
        services.AddSingleton(tableClient);

        // --- BLOB STORAGE SETUP
        // Connects to the blob service client and automatically ensures the staff-docs container exists
        var blobServiceClient = new BlobServiceClient(connectionString);
        var blobContainerClient = blobServiceClient.GetBlobContainerClient("staff-docs");
        blobContainerClient.CreateIfNotExists();

        // Register the blob clients into dependency injection so your teammate's endpoints can use them
        services.AddSingleton(blobServiceClient);
        services.AddSingleton(blobContainerClient);
    })
    .Build();

host.Run();
