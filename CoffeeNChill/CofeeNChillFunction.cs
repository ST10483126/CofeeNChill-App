using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace CoffeeNChill;

public class CofeeNChillFunction
{
    private readonly ILogger<CofeeNChillFunction> _logger;

    public CofeeNChillFunction(ILogger<CofeeNChillFunction> logger)
    {
        _logger = logger;
    }

    [Function("CofeeNChillFunction")]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");
        return new OkObjectResult("Welcome to Azure Functions!");
    }
}