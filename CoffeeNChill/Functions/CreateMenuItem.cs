
using Azure.Data.Tables;
using CoffeeNChill.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace CoffeeNChill;

public class CreateMenuItem
{
    private readonly TableClient _tableClient;
    private readonly ILogger<CreateMenuItem> _logger;

    public CreateMenuItem(
        TableClient tableClient,
        ILogger<CreateMenuItem> logger)
    {
        _tableClient = tableClient;
        _logger = logger;
    }

    [Function("CreateMenuItem")]
    public async Task<IActionResult> Run(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "post",
            Route = "menu")]
        HttpRequest req)
    {
        try
        {
            var menuItem = await JsonSerializer.DeserializeAsync<MenuItemEntity>(
                req.Body,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            if (menuItem == null)
            {
                return new BadRequestObjectResult(
                    "Invalid menu item data.");
            }

            if (string.IsNullOrWhiteSpace(menuItem.PartitionKey) ||
                string.IsNullOrWhiteSpace(menuItem.RowKey) ||
                string.IsNullOrWhiteSpace(menuItem.Name))
            {
                return new BadRequestObjectResult(
                    "PartitionKey, RowKey and Name are required.");
            }

            await _tableClient.AddEntityAsync(menuItem);

            _logger.LogInformation(
                "Menu item {RowKey} was created.",
                menuItem.RowKey);

            return new OkObjectResult(
                "Menu item created successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error creating menu item.");

            return new StatusCodeResult(500);
        }
    }
}

