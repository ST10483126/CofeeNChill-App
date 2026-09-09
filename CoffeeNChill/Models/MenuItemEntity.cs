using System;
using Azure;
using Azure.Data.Tables;

namespace CoffeeNChill.Models
{
    public class MenuItemEntity : ITableEntity
    {
        // Category string ("Hot Drinks", "Cold Drinks", "Teas", "Light Meals", "Bakery and Pastries")
        public string PartitionKey { get; set; } = string.Empty;

        // Unique Item SKU / ID (e.g. "COF-001", "PAS-104")
        public string RowKey { get; set; } = string.Empty;

        // Internal Azure Table properties required by the ITableEntity interface
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }

        // Item name ("Espresso", "Ham & Cheese Croissant")
        public string Name { get; set; } = string.Empty;

        // Short menu description
        public string Description { get; set; } = string.Empty;

        // Item price (Double/Decimal representation)
        public double Price { get; set; }

        // Availability status (Boolean)
        public bool IsAvailable { get; set; }
    }
}
