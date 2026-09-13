using System;
using System.Collections.Generic;
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

        // Connects to the "staff-docs" Blob Storage container using the connection string
        var containerClient = new BlobContainerClient(_connectionString, "staff-docs");

        // Ensures the blob container exists in storage before attempting to read from it
        await containerClient.CreateIfNotExistsAsync();

        // Creates a list to hold file info objects
        var fileList = new List<object>();

        // Loop through all blobs in the container and grab their details
        await foreach (BlobItem blobItem in containerClient.GetBlobsAsync())
        {
            // Adds an anonymous object containing the file name, size, and last modified date to the list
            fileList.Add(new
            {
                FileName = blobItem.Name,
                Size = blobItem.Properties.ContentLength,
                LastModified = blobItem.Properties.LastModified
            });
        }

        // Returns an HTTP 200 OK response with the populated list of files as JSON
        return new OkObjectResult(fileList);
    }
}