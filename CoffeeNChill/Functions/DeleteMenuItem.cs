
using Azure;
using Azure.Data.Tables;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace CoffeeNChill;

public class DeleteMenuItem
{
    private readonly TableClient _tableClient;
    private readonly ILogger<DeleteMenuItem> _logger;

    public DeleteMenuItem(
        TableClient tableClient,
        ILogger<DeleteMenuItem> logger)
    {
        _tableClient = tableClient;
        _logger = logger;
    }

    [Function("DeleteMenuItem")]
    public async Task<IActionResult> Run(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "delete",
            Route = "menu/{category}/{id}")]
        HttpRequest req,
        string category,
        string id)
    {
        try
        {
            await _tableClient.DeleteEntityAsync(
                category,
                id);

            _logger.LogInformation(
                "Menu item {RowKey} was deleted.",
                id);

            return new OkObjectResult(
                "Menu item deleted successfully.");
        }
        catch (RequestFailedException ex) when (ex.Status == 404)
        {
            return new NotFoundObjectResult(
                "Menu item not found.");
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error deleting menu item.");

            return new StatusCodeResult(500);
        }
    }
}


