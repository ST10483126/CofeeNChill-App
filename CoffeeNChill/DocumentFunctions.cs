using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace CoffeeNChill
{
    public class DocumentFunctions
    {
        private readonly ILogger<DocumentFunctions> _logger;
        private readonly BlobContainerClient _containerClient;

        // The BlobContainerClient we registered in Program.cs is automatically injected here
        public DocumentFunctions(ILogger<DocumentFunctions> logger, BlobContainerClient containerClient)
        {
            _logger = logger;
            _containerClient = containerClient;
        }

        // 1. Uploads function (POST /api/documents/upload)
        [Function("UploadDocument")]
        public async Task<IActionResult> Upload(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "documents/upload")] HttpRequest req)
        {
            _logger.LogInformation("Processing a document upload request.");

            try
            {
                var formCollection = await req.ReadFormAsync();
                var file = formCollection.Files.GetFile("file");

                if (file == null || file.Length == 0)
                {
                    _logger.LogWarning("Upload failed: No file was provided in the request form-data.");
                    return new BadRequestObjectResult("Please provide a valid file using the key 'file' in your form-data.");
                }

                // MIME-type validation: Only allow PDF documents as requested by the rubric
                if (file.ContentType != "application/pdf" && !file.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogWarning($"Upload blocked: Invalid file type '{file.ContentType}'. Only PDFs allowed.");
                    return new BadRequestObjectResult("Security Validation Error: Only PDF documents are allowed for staff records.");
                }

                var blobClient = _containerClient.GetBlobClient(file.FileName);

                // Stream-based file transfer to handle high performance efficiently
                using (var stream = file.OpenReadStream())
                {
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

                    await blobClient.UploadAsync(stream, blobOptions);
                }

                _logger.LogInformation($"Successfully uploaded {file.FileName} to local Azurite Blob storage.");
                return new OkObjectResult(new { message = $"Successfully uploaded '{file.FileName}' to staff documentation storage." });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Critical error occurred during file upload execution: {ex.Message}");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        // 2. List function (GET /api/documents)
        [Function("ListDocuments")]
        public async Task<IActionResult> List(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "documents")] HttpRequest req)
        {
            _logger.LogInformation("Retrieving a directory listing of all stored staff records.");
            var documentList = new List<object>();

            try
            {
                // Queries all blob elements inside the local Azurite container
                var blobs = _containerClient.GetBlobsAsync(new GetBlobsOptions { Traits = BlobTraits.Metadata });

                await foreach (var blob in blobs)
                {
                    documentList.Add(new
                    {
                        Name = blob.Name,
                        Size = blob.Properties.ContentLength != null ? $"{blob.Properties.ContentLength} bytes" : "Unknown",
                        ContentType = blob.Properties.ContentType ?? "application/octet-stream",
                        UploadDate = blob.Properties.CreatedOn ?? DateTimeOffset.MinValue,
                        Metadata = blob.Metadata
                    });
                }

                return new OkObjectResult(documentList);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Critical error occurred during storage collection listing: {ex.Message}");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        // 3. Download function (GET /api/documents/download/{fileName})
        [Function("DownloadDocument")]
        public async Task<IActionResult> Download(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "documents/download/{fileName}")] HttpRequest req,
            string fileName)
        {
            _logger.LogInformation($"Processing download stream request for resource file: '{fileName}'.");

            try
            {
                var blobClient = _containerClient.GetBlobClient(fileName);

                if (!await blobClient.ExistsAsync())
                {
                    _logger.LogWarning($"Download failed: Resource target file '{fileName}' does not exist inside storage container.");
                    return new NotFoundObjectResult($"The document '{fileName}' could not be located in staff documentation storage.");
                }

                // Stream-based file extraction downloading chunk data straight to the destination stream
                var downloadInfo = await blobClient.DownloadStreamingAsync();

                // Returns the actual file stream directly with correct browser headers
                return new FileStreamResult(downloadInfo.Value.Content, "application/pdf")
                {
                    FileDownloadName = fileName
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Critical error occurred during file transmission tracking: {ex.Message}");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
