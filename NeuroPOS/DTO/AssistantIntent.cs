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
        public double DiscountAmount { get; set; }
    }
}
