using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Azure.Storage.Blobs;

namespace CoffeeNChill.Functions;

public class DownloadStaffDocument
{
    private readonly ILogger<DownloadStaffDocument> _logger;
    private readonly string _connectionString;

    // Injects the logger and retrieves the storage connection string from environment settings
    public DownloadStaffDocument(ILogger<DownloadStaffDocument> logger)
    {
        _logger = logger;
        _connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage")
            ?? "UseDevelopmentStorage=true";
    }

    [Function("DownloadStaffDocument")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "documents/download/{fileName}")] HttpRequest req,
        string fileName)
    {
        // Logs an informational message containing the name of the file being requested for download
        _logger.LogInformation($"Downloading file: {fileName}");

        // Connects to the "staff-docs" Blob container and gets the requested file
        var containerClient = new BlobContainerClient(_connectionString, "staff-docs");
        var blobClient = containerClient.GetBlobClient(fileName);

        // Checks if the requested file actually exists in storage; returns a 404 Not Found if it doesn't
        if (!await blobClient.ExistsAsync())
        {
            return new NotFoundObjectResult($"File '{fileName}' not found.");
        }

        // Downloads the file contents as a stream from Blob Storage
        var download = await blobClient.DownloadStreamingAsync();

        // Returns the file stream back to the client as an octet-stream download with its original file name
        return new FileStreamResult(download.Value.Content, "application/octet-stream")
        {
            FileDownloadName = fileName
        };
    }
}