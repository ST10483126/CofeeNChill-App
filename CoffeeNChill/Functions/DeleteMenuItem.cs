
using Azure;
using Azure.Data.Tables;
using CoffeeNChill.Models;
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
            _logger.LogInformation(
                "Attempting to delete Category={Category}, ID={Id}",
                category,
                id);

            // Check if the menu item exists first
            var response = await _tableClient.GetEntityIfExistsAsync<MenuItemEntity>(
                category,
                id);

            if (!response.HasValue)
            {
                _logger.LogWarning(
                    "Menu item {RowKey} was not found.",
                    id);

                return new NotFoundObjectResult(
                    "Menu item not found.");
            }

            // Delete the existing menu item
            await _tableClient.DeleteEntityAsync(
                category,
                id);

            _logger.LogInformation(
                "Menu item {RowKey} was deleted.",
                id);

            return new OkObjectResult(
                "Menu item deleted successfully.");
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
