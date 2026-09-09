using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Data.Tables;
using CoffeeNChill.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace CoffeeNChill
{
    public class CofeeNChillFunction
    {
        private readonly ILogger<CofeeNChillFunction> _logger;
        private readonly TableClient _tableClient;

        // The TableClient registered in Program.cs is automatically injected here
        public CofeeNChillFunction(ILogger<CofeeNChillFunction> logger, TableClient tableClient)
        {
            _logger = logger;
            _tableClient = tableClient;
        }

        [Function("CofeeNChillFunction")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "menu")] HttpRequest req)
        {
            _logger.LogInformation("Processing a new menu item submission.");

            // Reads the JSON body from Postman
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonSerializer.Deserialize<MenuItemEntity>(requestBody, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (data == null)
            {
                return new BadRequestObjectResult("Please pass valid item data in the request body.");
            }

            // Saves the entity into your local Azurite Docker Table
            await _tableClient.AddEntityAsync(data);

            return new OkObjectResult(new { message = $"Successfully added {data.Name} to the menu!" });
        }
    }
}
