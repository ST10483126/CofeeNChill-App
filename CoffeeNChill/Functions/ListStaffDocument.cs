using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System.Collections.Generic;

namespace CoffeeNChill.Functions;

public class ListStaffDocuments
{
    private readonly ILogger<ListStaffDocuments> _logger;
    private readonly string _connectionString;

    // Injects the logger and grabs the storage connection string from environment settings
    public ListStaffDocuments(ILogger<ListStaffDocuments> logger)
    {
        _logger = logger;
        _connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage")
            ?? "UseDevelopmentStorage=true";
    }

    [Function("ListStaffDocuments")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "documents")] HttpRequest req)
    {
        // Logs an informational message when the listing function is triggered
        _logger.LogInformation("Listing all staff documents...");

        // Connects to the "staff-docs" Blob container using the connection string
        var containerClient = new BlobContainerClient(_connectionString, "staff-docs");

        // Ensures the Blob container exists before reading from it
        await containerClient.CreateIfNotExistsAsync();

        // Creates a list to hold file information
        var fileList = new List<object>();

        // Loops through all Blobs in the container
        await foreach (BlobItem item in containerClient.GetBlobsAsync())
        {
            // Adds the file name, size and last modified date to the list
            fileList.Add(new
            {
                FileName = item.Name,
                Size = item.Properties.ContentLength,
                LastModified = item.Properties.LastModified
            });
        }

        // Returns an OK response with the populated list of files 
        return new OkObjectResult(fileList);
    }
}