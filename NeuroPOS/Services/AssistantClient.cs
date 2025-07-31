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


        #region CorePrompt
        private const string CorePrompt =
        @"You are the AI assistant inside a point‑of‑sale (POS) desktop app.
Speak JSON only when the user intends an action; otherwise reply in plain
natural language.

Always:
• Follow the JSON schema precisely.
• Never add keys not listed in the schema.
• Never wrap JSON in markdown fences.

When the user asks “what can you do” or similar:
1. List the main capability areas (Selling, Inventory, Orders, Reports,
   Categories & Products).
2. Ask which area they’d like more details about.
When they name an area, respond with the matching help section in plain text.";
        #endregion

        #region SchemaPrompt
        private const string SchemaPrompt =
        @"JSON schema
{
  ""action"": ""sell"" | ""clear"" | ""show_cart"" |
             ""discount_only"" | ""check_stock"" | ""inventory_value"" |
             ""transactions_today"" | ""transactions_sell_today"" |
             ""transactions_buy_today"" | ""items_sold_today"" |
             ""sales_stats"" | ""cash_flow_today"" |
             ""add_category"" | ""edit_category_name"" | ""edit_category_desc"" |
             ""add_product"" | ""edit_product_price"" | ""edit_product_cost"" |
             ""edit_product_category"" | ""delete_product"" | ""buy_products"" |
             ""add_order"" | ""query_orders"" | ""confirm_order"" |
             ""delete_order"" | ""edit_order"",

  ""items"":           [ { ""name"": ""string"", ""quantity"": int } ],
  ""discount_amount"": number,                          // $ flat; omit or 0 if N/A
  ""payment"":         ""cash"" | ""on_tab"" | null,
  ""contact"":         ""string or null"",              // optional for edit_order
  ""order_id"":        int | null,
  ""response_mode"":   ""yes_no"" | ""details"",         // for query_orders
  ""new_items"":       [ { ""name"": ""string"", ""quantity"": int } ],
  ""new_discount"":    number | null,
  ""period"":          ""today"" | ""yesterday"" | ""last_week"" |
                      ""last_month"" | ""last_30_days"",

  // category/product ops
  ""category_name"": ""string or null"",
  ""description"":   ""string or null"",
  ""product_name"":  ""string or null"",
  ""price"":         number | null,
  ""cost"":          number | null,
  ""quantity"":      int    | null        // for buy_products

  /* For edit_order:
{""action"":""edit_order"",
 ""order_id"":21,
 ""new_items"":[{""name"":""Snickers"",""quantity"":5},
              {""name"":""Cola"",""quantity"":2}]}

     • Omit contact when order_id is present unless the user supplies it
     • new_items & new_discount are optional but at least one must appear */
}

If uncertain, return this stub exactly:
{""error"":""I am not sure how to respond""}";
        #endregion

        #region HomePrompt
        private const string HomePrompt =
        @"### Selling (Home page) ###
• sell            → “Sell 2 Pepsi and 1 Chips in cash”
• discount_only   → “Apply $3 discount to cart”
• on‑tab selling  → “Sell 5 Cola on tab for  Ahmad”
• clear           → “Clear cart”
• show_cart       → “What’s in cart?”
• check_stock     → “How many Pepsi left?”
Return JSON per schema.";
        #endregion

        #region TransactionsPrompt
        private const string TransactionsPrompt =
        @"### Transactions & Reports ###
• transactions_today            → all transactions today
• transactions_sell_today       → only 'sell' today
• transactions_buy_today        → only 'buy' today
• items_sold_today              → list items/qty sold today
• sales_stats last_week         → aggregated stats
• cash_flow_today               → net cash in/out today";
        #endregion

        #region InventoryPrompt
        private const string InventoryPrompt =
        @"### Inventory & Products ###

• add_category – ""Add category Drinks desc: Beverages""
• edit_category_name – ""Rename category Drinks to Bevs""
• edit_category_desc – ""Set Drinks description: Cold beverages""

• add_product – ""Add product Snickers price 3 cost 1 [category Drinks]""
                (if category is omitted or unknown, one will be assigned automatically;
                 image must be added manually)

• edit_product_price    – ""Set Pepsi price 1.4""
• edit_product_cost     – ""Set Pepsi cost 0.7""
• edit_product_category – ""Move Pepsi to Snacks""
• delete_product        – ""Delete product Pepsi""

• buy_products – ""Buy 30 Pepsi and 50 Cola""
                 ""Buy 200 Batata""   (single-item shortcut)

  JSON examples
  – Multiple items
    {""action"":""buy_products"",
     ""items"":[{""name"":""Pepsi"",""quantity"":30},
                {""name"":""Cola"",""quantity"":50}]}

  – Single-item shortcut
    {""action"":""buy_products"",
     ""product_name"":""Cola"",
     ""quantity"":200}";
        #endregion



        #region OrderPrompt
        private const string OrderPrompt =
        @"### Orders — natural‑language patterns ###

➊ **Linking to an existing contact**  
   ‑ Prefix the name with *loyal / dear / valued* (any adjective).  
     “Add order for **loyal Ahmad** …” → links to contact *Ahmad* (sets ContactId).  

➋ **Walk‑in / one‑off customer**  
   ‑ Omit the adjective.  
     “Add order for **Ahmad** …” → records *CustomerName = Ahmad*, *ContactId = 0* even if a contact exists.

────────────────────────────────────────────────────────
**Supported actions & sample phrases**

• **add_order**  
  ‑ “Add order for John with 12 Pepsi, 13 Chips discount 12”  
  ‑ “Add order for loyal Ahmad with 12 Pepsi discount 5”

• **query_orders** *(response_mode: yes_no | details)*  
  ‑ “How many pending orders for Samir?”           → *yes_no*  
  ‑ “Orders info for Bourhan (details)”            → *details*

• **confirm_order** *(pending → completed)*  
  ‑ “Confirm order 42”                              (contact optional)

• **delete_order** *(remove pending order)*  
  ‑ “Delete order 17”

• **edit_order** →

  – “Edit order 17 for Bourhan: Pepsi 10, Cola 12 **discount 5**”  
    JSON ➜ { ""action"":""edit_order"", ""order_id"":17,
             ""new_items"":[{""name"":""Pepsi"",""quantity"":10},
                          {""name"":""Cola"",""quantity"":12}],
             ""new_discount"":5 }

  – “Edit order 20: Pepsi 12 Cola 14 **discount 3**”  
    (contact omitted)  
    JSON ➜ { ""action"":""edit_order"",
             ""order_id"":20,
             ""new_items"":[{...}], ""new_discount"":3 }

  – “Edit order 21: Snickers 5 Cola 2”  (**no discount**)  
    JSON ➜ { ""action"":""edit_order"",
             ""order_id"":21,
             ""new_items"":[{""name"":""Snickers"",""quantity"":5},
                          {""name"":""Cola"",""quantity"":2}] }
– “Edit order 28: Pepsi 18”                         (**single item, no discount**)

  Works only while the order is *pending*; refuse if confirmed.  
  **`new_discount` is optional – omit the key entirely when the user did not
  mention a discount.  At least one of `new_items` or `new_discount`
  must appear.**


• confirm_order → “Confirm order 42”               (contact optional)
                  “Confirm order 42 for Ahmad”

  When *edit_order*: include **order_id** plus at least *new_items* or
  *new_discount*. Quantity 0 means “remove that line”.

────────────────────────────────────────────────────────
**Product names** — always use exact names from list  
Pepsi · Cola · Chips · …

────────────────────────────────────────────────────────
Example JSON for *add_order*:

{""action"":""add_order"",
 ""contact"":""loyal Bourhan"",
 ""items"":[{""name"":""Pepsi"",""quantity"":3}],
 ""discount_amount"":0}
{""action"":""query_orders"",
 ""contact"":""Bourhan"",
 ""response_mode"":""details""}

Return **only** valid JSON (no markdown, no commentary).  
If unsure, output the error stub exactly.";
        #endregion

        #region HelpPrompt
        private const string HelpPrompt =
        @"### What I can help you with ###
**Areas**
1. Selling / Cart
2. Inventory & Categories / Products
3. Orders (pending / confirmed)
4. Transactions & Reports
5. Cash‑flow & Stats

Ask the user: “Which area are you interested in?”

When they name an area:
• Reply with the matching help section (HomePrompt, OrderPrompt, etc.)
  using clear natural‑language examples.
• **Do NOT output raw JSON in the help text** – JSON is only for
  executing an action once the user issues a concrete command.";
        #endregion



        private static readonly InventoryVM inventoryVM = new InventoryVM();
        private static readonly OrderVM orderVM = new OrderVM();
        public async Task<string> GetRawAssistantReplyAsync(
            string userMessage,
            IEnumerable<string> productNames,
            IEnumerable<string> contactNames)
        {

            var categoryNames = inventoryVM.Categories.Select(c => c.Name).ToList();
            var customerName = orderVM.Orders.Select(c => c.CustomerName);

            string sys = CorePrompt +
              SchemaPrompt +
              HomePrompt +
              InventoryPrompt +
              TransactionsPrompt +
              OrderPrompt +
              HelpPrompt +
              "\n\nCurrent products:\n" + string.Join(", ", productNames) +
              "\nCurrent categories:\n" + string.Join(", ", categoryNames) +
              "\nCurrent contacts:\n" + string.Join(", ", contactNames) +
              "\nCurrent CustomerNames:\n" + string.Join(", ", customerName);


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