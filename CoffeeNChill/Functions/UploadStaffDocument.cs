using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading.Tasks;


namespace CoffeeNChill.Functions;

public class UploadStaffDocument
{
    private readonly ILogger<UploadStaffDocument> _logger;
    private readonly string _connectionString;

    // Gets the logger and storage connection
    public UploadStaffDocument(ILogger<UploadStaffDocument> logger)
    {
        _logger = logger;
        _connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage")
     ?? "UseDevelopmentStorage=true";
    }

    [Function("UploadStaffDocument")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "documents/upload")] HttpRequest req)
    {
        // Shows a message when the function starts
        _logger.LogInformation("Processing staff document upload.");

        // Checks if a file was uploaded
        if (!req.HasFormContentType || req.Form.Files.Count == 0)
        {
            return new BadRequestObjectResult("Please upload a file using multipart/form-data.");
        }

        // Gets the uploaded file
        var file = req.Form.Files[0];

        // Connects to the staff-docs Blob container
        var containerClient = new BlobContainerClient(_connectionString, "staff-docs");

        // Creates the container if it does not exist
        await containerClient.CreateIfNotExistsAsync();

        // Gets the Blob for the uploaded file
        var blobClient = containerClient.GetBlobClient(file.FileName);

        // Opens and uploads the file to Blob Storage
        using (var stream = file.OpenReadStream())
        {
            await blobClient.UploadAsync(stream, overwrite: true);
        }

        // Returns a success message
        return new OkObjectResult($"File '{file.FileName}' uploaded successfully!");
    }
}