using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace CoffeeNChill.Functions;

public class UploadStaffDocument
{
    private readonly ILogger<UploadStaffDocument> _logger;

    public UploadStaffDocument(ILogger<UploadStaffDocument> logger)
    {
        _logger = logger;
    }

    [Function("UploadStaffDocument")]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");
        return new OkObjectResult("Welcome to Azure Functions!");
    }
}