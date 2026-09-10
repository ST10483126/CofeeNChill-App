using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Azure.Storage.Files.Shares;
using Azure.Storage.Files.Shares.Models;
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
        _connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
    }

    [Function("ListStaffDocuments")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "documents")] HttpRequest req)
    {
        // Logs an informational message when the listing function is triggered
        _logger.LogInformation("Listing all staff documents...");

        // Connects to the "staff-docs" Azure File Share using the connection string
        var shareClient = new ShareClient(_connectionString, "staff-docs");

        // Ensures the file share exists in storage before attempting to read from it
        await shareClient.CreateIfNotExistsAsync();

        // Gets a reference to the root directory and creates a list to hold file info objects
        var directoryClient = shareClient.GetRootDirectoryClient();
        var fileList = new List<object>();

        // Loop through all files and subdirectories in the share root
        await foreach (ShareFileItem item in directoryClient.GetFilesAndDirectoriesAsync())
        {
            // Checks to ensure the item is a file and not a folder
            if (!item.IsDirectory)
            {
                // Gets the client and metadata properties for the specific file
                var fileClient = directoryClient.GetFileClient(item.Name);
                var properties = await fileClient.GetPropertiesAsync();

                // Adds an anonymous object containing the file name, size, and last modified date to the list
                fileList.Add(new
                {
                    FileName = item.Name,
                    Size = properties.Value.ContentLength,
                    LastModified = properties.Value.LastModified
                });
            }
        }

        // Returns an OK response with the populated list of files 
        return new OkObjectResult(fileList);
    }
}