
using Azure;
using Azure.Data.Tables;
using CoffeeNChill.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace CoffeeNChill;

public class UpdateMenuItem
{
    private readonly TableClient _tableClient;
    private readonly ILogger<UpdateMenuItem> _logger;

    public UpdateMenuItem(
        TableClient tableClient,
        ILogger<UpdateMenuItem> logger)
    {
        _tableClient = tableClient;
        _logger = logger;
    }

    [Function("UpdateMenuItem")]
    public async Task<IActionResult> Run(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "put",
            Route = "menu/{category}/{id}")]
        HttpRequest req,
        string category,
        string id)
    {
        try
        {
            var existingItem = await _tableClient.GetEntityAsync<MenuItemEntity>(
                category,
                id);

            var menuItem = existingItem.Value;

            var updateData = await JsonSerializer.DeserializeAsync<MenuItemEntity>(
                req.Body,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            if (updateData == null)
            {
                return new BadRequestObjectResult(
                    "Invalid update data.");
            }

            menuItem.Price = updateData.Price;
            menuItem.IsAvailable = updateData.IsAvailable;

            await _tableClient.UpdateEntityAsync(
                menuItem,
                menuItem.ETag,
                TableUpdateMode.Replace);

            _logger.LogInformation(
                "Menu item {RowKey} was updated.",
                id);

            return new OkObjectResult(
                "Menu item updated successfully.");
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
                "Error updating menu item.");

            return new StatusCodeResult(500);
        }
    }
}


