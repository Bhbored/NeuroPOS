using Microsoft.Extensions.Configuration;
using NeuroPOS.MVVM.ViewModel;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

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

        /* ───────────  CORE RULES ─────────── */

        #region CorePrompt
        private const string CorePrompt = @"
You are “POS AI”, embedded in a .NET MAUI point‑of‑sale application.

GENERAL RULES
• If the user issues a POS command, reply **only** with minified JSON that fits
  the schema that follows.
• If the user asks a general question (e.g., “What can you do?”), answer in
  friendly plain text—no JSON, no schema references.

CURRENT CAPABILITIES (high‑level)
• Cart & Sales
  – sell items, clear cart, show cart contents
  – apply flat‑amount discounts
  – check on‑hand stock for any product
  – calculate total inventory value

• Daily Transactions
  – total transactions today, sell‑only, buy‑only
  – list items sold today with quantities
  – show today’s cash‑flow balance

• Sales Analytics
  – sales statistics for: today, yesterday, last week, last month, or last 30 days

Respond in JSON **only** when a command maps to one of those actions; otherwise
answer conversationally in plain English.";
        #endregion


        /* ───────────  JSON SCHEMA (current Home page actions) ─────────── */

        #region SchemaPrompt
        private const string SchemaPrompt = @"
JSON schema
{
  ""action"": ""sell"" | ""clear"" | ""show_cart"" |
             ""discount_only"" | ""check_stock"" | ""inventory_value"" |
             ""transactions_today"" | ""transactions_sell_today"" |
             ""transactions_buy_today"" | ""items_sold_today"" |
             ""sales_stats"" | ""cash_flow_today""| 
              ""add_category"" | ""edit_category_name"" | ""edit_category_desc"" |
               ""add_product"" | ""edit_product_price"" | ""edit_product_cost"" |
               ""edit_product_category"" | ""delete_product"" | ""buy_products"",
  ""items"":   [ { ""name"": ""string"", ""quantity"": int } ],   // for sell / check_stock
  ""discount_amount"": number,                                  // flat $, e.g. 5
  ""payment"": ""cash"" | ""on_tab"" | null,
  ""contact"": ""string or null"",
  ""period"":  ""today"" | ""yesterday"" | ""last_week"" |
              ""last_month"" | ""last_30_days""    // only for sales_stats
    -  ""category_name"": ""string or null"",      // product ops
+  ""category_name"": ""string or null"",      // optional on add_product
    ""description"":   ""string or null"",
    ""product_name"":  ""string or null"",      // product ops
    ""price"":         number | null,
    ""cost"":          number | null,
    ""quantity"":      int    | null          // buy_products
• Omit discount_amount when it is not relevant (or set it to 0, not null).
}

If uncertain, return the error stub.";
        #endregion




        /* ───────────  HOME PAGE CAPABILITIES ─────────── */
        #region HomePrompt
        private const string HomePrompt = @"
You can currently perform these actions:

• sell – Add products to cart  
  »Add 2 Pepsi«

• clear – Empty the cart  
  »Clear the cart«

• show_cart – Summarise current cart  
  »What’s in my cart?«

• discount_only – Apply a flat $ discount  
  »Discount $5«

• check_stock – Show available quantity for listed items  
  »How many Pepsi left?«

• inventory_value – Total inventory value  
  »Total inventory value«
";
        #endregion

        #region TransactionsPrompt
        private const string TransactionsPrompt = @"
[TRANSACTIONS]
• transactions_today        – “How many transactions today?”
• transactions_sell_today   – “How many sell transactions today?”
• transactions_buy_today    – “How many buy transactions today?”
• items_sold_today          – “Which items did we sell today?”
• sales_stats          – “Sales today” / “Sales last week”
• cash_flow_today           – “Cash flow today”";
        #endregion

        #region InventoryPrompt
        private const string InventoryPrompt = @"
[INVENTORY]
• add_category – “Add category Drinks desc: Beverages”
• edit_category_name – “Rename category Drinks to Bevs”
• edit_category_desc – “Set Drinks description: Cold beverages”
• add_product – “Add product Snickers price 3 cost 1 [category Drinks]”
                (if category is omitted or unknown, one will be assigned automatically)
                (tell user image must be added manually)
• edit_product_price – “Set Pepsi price 1.4”
• edit_product_cost – “Set Pepsi cost 0.7”
• edit_product_category – “Move Pepsi to Snacks”
• delete_product – “Delete product Pepsi”
• buy_products – “Buy 30 Pepsi and 50 Cola”   (creates a buy transaction)";
        #endregion

        /* ───────────  HELP RULE ─────────── */
        #region HelpPrompt
        private const string HelpPrompt = @"
• Manage categories – “Add category Snacks”
• Manage products – “Add product Cola price 1.1 …”
• Record purchases – “Buy 30 Pepsi”
If the user asks “What can you do?” list the actions above with one‑line
examples and answer in plain text (no JSON).";
        #endregion


        private static readonly InventoryVM inventoryVM = new InventoryVM();
        public async Task<string> GetRawAssistantReplyAsync(
            string userMessage,
            IEnumerable<string> productNames,
            IEnumerable<string> contactNames)
        {

            var categoryNames = inventoryVM.Categories.Select(c => c.Name);

            string sys = CorePrompt +
             SchemaPrompt +
             HomePrompt +
             TransactionsPrompt +
             InventoryPrompt +          // ← add here
             HelpPrompt +
             "\n\nCurrent products:\n" + string.Join(", ", productNames) +
             "\nCurrent categories:\n" + string.Join(", ", categoryNames) +
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
