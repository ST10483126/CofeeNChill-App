

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace CoffeeNChill;

public class CoffeeNChillFunction
{
    private readonly ILogger<CoffeeNChillFunction> _logger;

    public CoffeeNChillFunction(ILogger<CoffeeNChillFunction> logger)
    {
        _logger = logger;
    }

    [Function("CoffeeNChillFunction")]
    public async Task<IActionResult> Run(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "get",
            "post")]
        HttpRequest req)
    {
        _logger.LogInformation(
            "C# HTTP trigger function processed a request.");

        await Task.CompletedTask;

        return new OkObjectResult(
            "Welcome to Azure Functions!");
    }
}

