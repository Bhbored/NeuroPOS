using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace NeuroPOS.Services
{
    public sealed class AssistantClient
    {
        // chat‑capable GPT‑4o‑mini ID visible in your /v1/models list
        private const string ModelId = "gpt-4o-mini-2024-07-18";

        private readonly HttpClient _http;

        public AssistantClient(IConfiguration cfg)
        {
            var apiKey = cfg["OpenAI:ApiKey"]?.Trim();
            if (string.IsNullOrWhiteSpace(apiKey))
                throw new InvalidOperationException("OpenAI API key is missing.");

            _http = new HttpClient { BaseAddress = new Uri("https://api.openai.com") };
            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", apiKey);

            // Uncomment if your account requires a project header:
            // _http.DefaultRequestHeaders.Add("OpenAI-Project", "proj_XXXXXXXXXXXX");
        }

        #region System prompt
        private const string SystemPrompt = @"
You are “POS AI”, integrated into a .NET MAUI point‑of‑sale app.

● When the user makes a POS request (sell items, clear cart, discounts, stock, inventory value):
    → Respond **ONLY** with minified JSON matching this schema:
      {
        ""action"":            ""sell"" | ""clear"" | ""show_cart"" | ""discount_only"" | ""check_stock"" | ""inventory_value"",
        ""items"":             [ { ""name"": ""string"", ""quantity"": int } ],
        ""payment"":           ""cash"" | ""on_tab"" | null,
        ""contact"":           ""string or null"",
        ""discount_amount"":   number        // flat amount, e.g. 5 means −$5
      }

Examples
• “Add 3 Pepsi and 1 Chips in cash”  → action=sell, items=[…], payment=""cash"", discount_amount=0
• “Apply a $10 discount only”        → action=discount_only, discount_amount=10
• “Clear the cart”                   → action=clear
• “Check stock of Coke and Water”    → action=check_stock, items=[…]
• “How much is my current inventory?”→ action=inventory_value

Rules
1. Product names must match exactly the names provided in the product list.
2. If quantity is omitted, assume 1.
3. If uncertain, respond with the JSON error stub:
   {""action"":""error"",""items"":[],""payment"":null,""contact"":null,""discount_amount"":0}";

        #endregion

        public async Task<string> GetRawAssistantReplyAsync(
            string userMessage,
            IEnumerable<string> productNames,
            IEnumerable<string> contactNames)
        {
            // Append live names so the model never invents entities
            string sys = SystemPrompt +
                         "\n\nCurrent products:\n" + string.Join(", ", productNames) +
                         "\nCurrent contacts:\n" + string.Join(", ", contactNames);

            var payload = new
            {
                model = ModelId,
                messages = new[]
                {
                    new { role = "system", content = sys },
                    new { role = "user",   content = userMessage }
                },
                temperature = 0.2
            };

            var res = await _http.PostAsync("/v1/chat/completions",
                                             JsonContent.Create(payload));
            res.EnsureSuccessStatusCode();

            string body = await res.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(body);

            return doc.RootElement
                      .GetProperty("choices")[0]
                      .GetProperty("message")
                      .GetProperty("content")
                      .GetString();
        }
    }
}
