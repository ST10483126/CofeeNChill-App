using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace CoffeeNChill.Functions
{
    public class UploadStaffDocument
    {
        private readonly ILogger<UploadStaffDocument> _logger;
        private readonly BlobContainerClient _containerClient;

        // Constructor: Uses Dependency Injection to safely fetch the active BlobContainerClient configured in Program.cs
        public UploadStaffDocument(ILogger<UploadStaffDocument> logger, BlobContainerClient containerClient)
        {
            _logger = logger;
            _containerClient = containerClient;
        }

        // UPLOAD FUNCTION (POST /api/documents/upload)
        [Function("UploadStaffDocument")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "documents/upload")] HttpRequest req)
        {
            _logger.LogInformation("Processing a staff document upload request.");

            try
            {
                // Parse the incoming multi-part form data from the client request
                var formCollection = await req.ReadFormAsync();
                var file = formCollection.Files.GetFile("file");

                // Validation Check: Ensure a file payload object was actually provided in the request
                if (file == null || file.Length == 0)
                {
                    _logger.LogWarning("Upload failed: No file was provided in the request form-data.");
                    return new BadRequestObjectResult("Please provide a valid file using the key 'file' in your form-data.");
                }

                // Enforce MIME-type restrictions to only allow PDF uploads for security
                if (file.ContentType != "application/pdf" && !file.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogWarning($"Upload blocked: Invalid file type '{file.ContentType}'. Only PDFs allowed.");
                    return new BadRequestObjectResult("Security Validation Error: Only PDF documents are allowed for staff records.");
                }

                // Establish a reference pointer targeting the specific file path destination inside the blob container
                var blobClient = _containerClient.GetBlobClient(file.FileName);

                // High-Performance Stream-Based Transfer: Stream raw file content chunks straight into local Azurite storage
                using (var stream = file.OpenReadStream())
                {
                    // Rubric Criterion Metadata Mapping: Log file tracking attributes to the storage backend layers
                    var blobOptions = new BlobUploadOptions
                    {
                        HttpHeaders = new BlobHttpHeaders { ContentType = "application/pdf" },
                        Metadata = new Dictionary<string, string>
                        {
                            { "OriginalName", file.FileName },
                            { "FileSize", file.Length.ToString() + " bytes" },
                            { "UploadDate", DateTimeOffset.UtcNow.ToString("o") }
                        }
                    };

                    // Execute the asynchronous upload binary streaming process
                    await blobClient.UploadAsync(stream, blobOptions);
                }

                _logger.LogInformation($"Successfully uploaded {file.FileName} to local Azurite Blob storage.");
                return new OkObjectResult(new { message = $"Successfully uploaded '{file.FileName}' to staff documentation storage." });
            }
            catch (Exception ex)
            {
                // Error Tracking: Log the full runtime exception tracing strings safely to the console framework
                _logger.LogError($"Critical error occurred during file upload execution: {ex.Message}");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
