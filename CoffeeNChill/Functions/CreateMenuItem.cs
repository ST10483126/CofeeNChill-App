
using Azure;
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
            var menuItem = await JsonSerializer.DeserializeAsync<MenuItemDto>(
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

            if (menuItem.Price < 0)
            {
                return new BadRequestObjectResult(
                    "Price cannot be negative.");
            }

            var entity = new MenuItemEntity
            {
                PartitionKey = menuItem.PartitionKey,
                RowKey = menuItem.RowKey,
                Name = menuItem.Name,
                Description = menuItem.Description,
                Price = menuItem.Price,
                IsAvailable = menuItem.IsAvailable
            };

            await _tableClient.AddEntityAsync(entity);

            _logger.LogInformation(
                "Menu item {RowKey} was created.",
                menuItem.RowKey);

            return new OkObjectResult(
                "Menu item created successfully.");
        }
        catch (RequestFailedException ex) when (ex.Status == 409)
        {
            return new ConflictObjectResult(
                "A menu item with this ID already exists.");
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