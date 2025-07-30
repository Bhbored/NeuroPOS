using System.Text.Json.Serialization;

namespace NeuroPOS.DTO
{
    public class AssistantIntent
    {
        [JsonPropertyName("action")]
        public string Action { get; set; }

        [JsonPropertyName("items")]
        public List<ItemIntent> Items { get; set; } = new();

        [JsonPropertyName("payment")]
        public string Payment { get; set; }

        [JsonPropertyName("contact")]
        public string Contact { get; set; }

        [JsonPropertyName("discount_amount")]
        public double? DiscountAmount { get; set; }

        [JsonPropertyName("period")]
        public string Period { get; set; }

        [JsonPropertyName("category_name")]
        public string? CategoryName { get; set; }
        [JsonPropertyName("description")] public string Description { get; set; }
        [JsonPropertyName("product_name")] public string ProductName { get; set; }
        [JsonPropertyName("price")] public double? Price { get; set; }
        [JsonPropertyName("cost")] public double? Cost { get; set; }
        [JsonPropertyName("quantity")] public int? Quantity { get; set; }
        [JsonPropertyName("order_id")] public int? OrderId { get; set; }
        [JsonPropertyName("response_mode")] public string ResponseMode { get; set; }
        [JsonPropertyName("new_items")] public List<ItemIntent> New_Items { get; set; } = new();
        [JsonPropertyName("new_discount")] public double? New_Discount { get; set; }




    }
}
