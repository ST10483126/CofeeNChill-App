using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Azure.Storage.Blobs;

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
            // Logs an informational message when the upload function is triggered
            _logger.LogInformation("Processing staff document upload.");

            // Validates that the incoming HTTP request contains a form and at least one file
            if (!req.HasFormContentType || req.Form.Files.Count == 0)
            {
                return new BadRequestObjectResult("Please upload a file using multipart/form-data.");
            }

            // Extracts the first uploaded file from the request form data
            var file = req.Form.Files[0];

            // Connect to Blob Storage container "staff-docs"
            var containerClient = new BlobContainerClient(_connectionString, "staff-docs");

            // Ensures the blob container exists in storage (creates it if it doesn't)
            await containerClient.CreateIfNotExistsAsync();

            // Gets a reference to the blob using the uploaded file's name
            var blobClient = containerClient.GetBlobClient(file.FileName);

            // Opens a read stream on the incoming file and uploads it to Blob Storage
            using (var stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, overwrite: true);
            }

            // Returns a success HTTP 200 response indicating the file was saved
            return new OkObjectResult($"File '{file.FileName}' uploaded successfully!");
        }
    }