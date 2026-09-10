using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Files.Shares;
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

    // Injects the logger and reads the storage connection string from environment settings
    public UploadStaffDocument(ILogger<UploadStaffDocument> logger)
    {
        _logger = logger;
        _connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
    }

    [Function("UploadStaffDocument")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "documents/upload")] HttpRequest req)
    {
        // Logs an informational message when the function is triggered
        _logger.LogInformation("Processing staff document upload.");

        // Validates that the incoming HTTP request contains a form and at least one file
        if (!req.HasFormContentType || req.Form.Files.Count == 0)
        {
            return new BadRequestObjectResult("Please upload a file using multipart/form-data.");
        }

        // Extracts the first uploaded file from the request form data
        var file = req.Form.Files[0];

        // Initializes the ShareClient using the connection string and targets the "staff-docs" share
        var shareClient = new ShareClient(_connectionString, "staff-docs");

        // Ensures the "staff-docs" file share exists in storage (creates it if it doesn't)
        await shareClient.CreateIfNotExistsAsync();

        // Gets the root directory reference of the file share and prepares the file client using the uploaded file's name
        var directoryClient = shareClient.GetRootDirectoryClient();
        var fileClient = directoryClient.GetFileClient(file.FileName);

        // Opens a read stream on the incoming file and uploads its data directly to the storage share
        using (var stream = file.OpenReadStream())
        {
            await fileClient.CreateAsync(file.Length);
            await fileClient.UploadRangeAsync(new HttpRange(0, file.Length), stream);
        }

        // Returns a success HTTP 200 response indicating the file was saved
        return new OkObjectResult($"File '{file.FileName}' uploaded successfully!");
    }
}