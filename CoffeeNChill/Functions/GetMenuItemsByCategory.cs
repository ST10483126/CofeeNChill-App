
using Azure.Data.Tables;
using CoffeeNChill.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace CoffeeNChill;

public class GetMenuItemsByCategory
{
    private readonly TableClient _tableClient;
    private readonly ILogger<GetMenuItemsByCategory> _logger;

    public GetMenuItemsByCategory(
        TableClient tableClient,
        ILogger<GetMenuItemsByCategory> logger)
    {
        _tableClient = tableClient;
        _logger = logger;
    }

    [Function("GetMenuItemsByCategory")]
    public async Task<IActionResult> Run(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "get",
            Route = "menu/category/{category}")]
        HttpRequest req,
        string category)
    {
        try
        {
            var menuItems = new List<MenuItemEntity>();

            await foreach (var item in _tableClient.QueryAsync<MenuItemEntity>(
                item => item.PartitionKey == category))
            {
                menuItems.Add(item);
            }

            _logger.LogInformation(
                "Retrieved {Count} menu items for category {Category}.",
                menuItems.Count,
                category);

            return new OkObjectResult(menuItems);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error retrieving menu items by category.");

            return new StatusCodeResult(500);
        }
    }
}
