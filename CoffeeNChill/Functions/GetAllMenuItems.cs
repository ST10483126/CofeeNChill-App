
using Azure.Data.Tables;
using CoffeeNChill.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace CoffeeNChill;

public class GetAllMenuItems
{
    private readonly TableClient _tableClient;
    private readonly ILogger<GetAllMenuItems> _logger;

    public GetAllMenuItems(
        TableClient tableClient,
        ILogger<GetAllMenuItems> logger)
    {
        _tableClient = tableClient;
        _logger = logger;
    }

    [Function("GetAllMenuItems")]
    public async Task<IActionResult> Run(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "get",
            Route = "menu")]
        HttpRequest req)
    {
        try
        {
            var menuItems = new List<MenuItemEntity>();

            await foreach (var item in _tableClient.QueryAsync<MenuItemEntity>())
            {
                menuItems.Add(item);
            }

            _logger.LogInformation(
                "Retrieved {Count} menu items.",
                menuItems.Count);

            return new OkObjectResult(menuItems);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error retrieving menu items.");

            return new StatusCodeResult(500);
        }
    }
}

