using System;
using System.Collections.Generic;
using System.Linq;
using NeuroPOS.MVVM.Model; 
using System.Diagnostics;
using NeuroPOS;
using Contact = NeuroPOS.MVVM.Model.Contact; // For Debug.WriteLine

// IMPORTANT: This class should be executed AFTER your database and repositories (App.*Repo) are initialized.
public class TestDataPopulator
{
    private readonly Random _random = new Random();
    private readonly List<string> _firstNames = new List<string> { "John", "Jane", "Michael", "Sarah", "David", "Emily", "Robert", "Lisa", "William", "Jessica", "Thomas", "Ashley", "Charles", "Amanda", "Daniel", "Melissa", "Matthew", "Nicole", "Anthony", "Stephanie" };
    private readonly List<string> _lastNames = new List<string> { "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller", "Davis", "Rodriguez", "Martinez", "Hernandez", "Lopez", "Gonzalez", "Wilson", "Anderson", "Thomas", "Taylor", "Moore", "Jackson", "Martin" };
    private readonly List<string> _companySuffixes = new List<string> { "Inc.", "LLC", "Corp.", "Ltd.", "Group", "Solutions", "Enterprises", "Associates" };
    private readonly List<string> _streetTypes = new List<string> { "St", "Ave", "Blvd", "Rd", "Ln", "Dr", "Ct", "Pl" };

    // --- Product & Category Data (from previous generations) ---
    private readonly List<(string Name, double Price, double Cost, int Stock, string CategoryName, string ImageUrl)> _productData = new List<(string, double, double, int, string, string)>
    {
        ("Coca-Cola Classic Soda (6-Pack)", 4.99, 2.50, 120, "Beverages", "https://images.unsplash.com/photo-1625772299848-391b6a87d7b3?w=500&auto=format&fit=crop"),
        ("Apple Granny Smith", 1.29, 0.60, 85, "Fruits", "https://images.unsplash.com/photo-1568702846914-96b305d2aaeb?w=500&auto=format&fit=crop"),
        ("Lay's Classic Potato Chips", 3.49, 1.75, 200, "Snacks", "https://images.unsplash.com/photo-1626082927389-6cd097cee6a6?w=500&auto=format&fit=crop"),
        ("Nescafé Instant Coffee", 8.99, 5.00, 60, "Beverages", "https://images.unsplash.com/photo-1514432324607-a09d9b4aefdd?w=500&auto=format&fit=crop"),
        ("Colgate Toothpaste", 2.99, 1.20, 150, "Personal Care", "https://images.unsplash.com/photo-1611078489935-0f0e5d3c5d9c?w=500&auto=format&fit=crop"),
        ("Maggi 2-Minute Noodles", 1.50, 0.75, 300, "Grocery", "https://images.unsplash.com/photo-1603105090899-1f1d5a0b0d5a?w=500&auto=format&fit=crop"),
        ("Lifebuoy Handwash Soap", 1.25, 0.55, 180, "Personal Care", "https://images.unsplash.com/photo-1584375524502-9c0b13c4d2b3?w=500&auto=format&fit=crop"),
        ("Samsung 55\" 4K Smart TV", 499.99, 350.00, 25, "Electronics", "https://images.unsplash.com/photo-1593305841991-05c297ba4575?w=500&auto=format&fit=crop"),
        ("iPhone 15 Pro", 1099.00, 850.00, 15, "Electronics", "https://images.unsplash.com/photo-1603302576837-37561b2e2302?w=500&auto=format&fit=crop"),
        ("Levi's 501 Original Jeans", 69.99, 35.00, 40, "Clothing", "https://images.unsplash.com/photo-1541099649105-f69ad21f3246?w=500&auto=format&fit=crop"),
        ("Nike Air Max Sneakers", 129.99, 75.00, 30, "Clothing", "https://images.unsplash.com/photo-1542291026-7eec264c27ff?w=500&auto=format&fit=crop"),
        ("Mr. Clean All Purpose Cleaner", 4.49, 2.20, 90, "Household", "https://images.unsplash.com/photo-1584820156882-0c59a0e2d0c3?w=500&auto=format&fit=crop"),
        ("Barbie Dreamhouse Playset", 199.99, 120.00, 20, "Toys", "https://images.unsplash.com/photo-1566576721346-d4a3b4eaeb55?w=500&auto=format&fit=crop"),
        ("LEGO Creator Expert Millennium Falcon", 849.99, 650.00, 5, "Toys", "https://images.unsplash.com/photo-1637008336770-b95d6a8c9baa?w=500&auto=format&fit=crop"),
        ("Harry Potter and the Goblet of Fire", 12.99, 6.50, 75, "Books", "https://images.unsplash.com/photo-1629992101753-56d196c8aabb?w=500&auto=format&fit=crop"),
        ("Purina ONE Dry Dog Food", 34.99, 22.00, 45, "Pet Supplies", "https://images.unsplash.com/photo-1587300003388-59208cc962cb?w=500&auto=format&fit=crop"),
        ("Pedigree Wet Dog Food Pouches", 1.29, 0.75, 120, "Pet Supplies", "https://images.unsplash.com/photo-1587300003388-59208cc962cb?w=500&auto=format&fit=crop"),
        ("Sunsilk Shampoo", 4.99, 2.50, 110, "Personal Care", "https://images.unsplash.com/photo-1596040033229-a9821f0e50d4?w=500&auto=format&fit=crop"),
        ("Indomie Instant Noodles (Pack of 10)", 3.99, 2.00, 250, "Grocery", "https://images.unsplash.com/photo-1603105090899-1f1d5a0b0d5a?w=500&auto=format&fit=crop"),
        ("Pepsi-Cola Soda (12-Pack)", 5.99, 3.00, 100, "Beverages", "https://images.unsplash.com/photo-1625772452859-10d8f20d7a5c?w=500&auto=format&fit=crop"),
        ("Banana (1 lb)", 0.69, 0.30, 95, "Fruits", "https://images.unsplash.com/photo-1571771894821-ce9b6c11b08e?w=500&auto=format&fit=crop"),
        ("Doritos Nacho Cheese Chips", 4.29, 2.10, 180, "Snacks", "https://images.unsplash.com/photo-1613557941766-0b90c9d3f0c3?w=500&auto=format&fit=crop"),
        ("Milk (1 Gallon)", 3.49, 2.00, 150, "Dairy", "https://images.unsplash.com/photo-1563636619-e9143da7973b?w=500&auto=format&fit=crop"),
        ("Eggs (Dozen)", 2.99, 1.50, 200, "Dairy", "https://images.unsplash.com/photo-1603569283847-aa0c73c2e1d7?w=500&auto=format&fit=crop"),
        ("Bread (Loaf)", 2.49, 1.20, 130, "Bakery", "https://images.unsplash.com/photo-1509440159596-0249088772ff?w=500&auto=format&fit=crop"),
        ("Raspberry Pi 4 Model B", 75.00, 50.00, 35, "Electronics", "https://images.unsplash.com/photo-1610010554923-9a1d70e0f1d4?w=500&auto=format&fit=crop"),
        ("Arduino Uno Rev3", 24.95, 15.00, 60, "Electronics", "https://images.unsplash.com/photo-1587202372775-e229f172b9d3?w=500&auto=format&fit=crop"),
        ("Hanes Men's Crew T-Shirt (3-Pack)", 18.99, 10.00, 80, "Clothing", "https://images.unsplash.com/photo-1521572163474-6864f9cf17ab?w=500&auto=format&fit=crop"),
        ("Oversized Hoodie", 39.99, 20.00, 50, "Clothing", "https://images.unsplash.com/photo-1598033385743-6e9ac6ddd904?w=500&auto=format&fit=crop"),
        ("Lysol Disinfectant Spray", 6.99, 4.00, 70, "Household", "https://images.unsplash.com/photo-1584820156882-0c59a0e2d0c3?w=500&auto=format&fit=crop"),
        ("Clorox Bleach", 3.49, 1.80, 95, "Household", "https://images.unsplash.com/photo-1584820156882-0c59a0e2d0c3?w=500&auto=format&fit=crop"),
        ("Hot Wheels 5-Pack", 6.99, 3.50, 110, "Toys", "https://images.unsplash.com/photo-1633356122102-3fe601e05bd2?w=500&auto=format&fit=crop"),
        ("Play-Doh Modeling Compound 10-Pack", 14.99, 8.00, 65, "Toys", "https://images.unsplash.com/photo-1633356122102-3fe601e05bd2?w=500&auto=format&fit=crop"),
        ("The Subtle Art of Not Giving a F*ck (Book)", 16.99, 9.00, 55, "Books", "https://images.unsplash.com/photo-1535732820275-9ffd998cac22?w=500&auto=format&fit=crop"),
        ("Atomic Habits (Book)", 18.99, 10.00, 60, "Books", "https://images.unsplash.com/photo-1544947950-fa07a98d237f?w=500&auto=format&fit=crop"),
        ("Whiskas Tuna Flavour Cat Food", 1.49, 0.90, 140, "Pet Supplies", "https://images.unsplash.com/photo-1519052537078-e6302a4968d4?w=500&auto=format&fit=crop"),
        ("Temptations Cat Treats", 4.99, 2.75, 85, "Pet Supplies", "https://images.unsplash.com/photo-1519052537078-e6302a4968d4?w=500&auto=format&fit=crop"),
        ("Head & Shoulders Shampoo", 7.49, 4.00, 75, "Personal Care", "https://images.unsplash.com/photo-1596040033229-a9821f0e50d4?w=500&auto=format&fit=crop"),
        ("Dove Beauty Bar Soap (4-Pack)", 5.99, 3.20, 125, "Personal Care", "https://images.unsplash.com/photo-1596040033229-a9821f0e50d4?w=500&auto=format&fit=crop"),
        ("Ketchup (Heinz)", 3.29, 1.80, 160, "Condiments", "https://images.unsplash.com/photo-1603302576837-37561b2e2302?w=500&auto=format&fit=crop"),
        ("Mayonnaise (Hellmann's)", 4.49, 2.50, 115, "Condiments", "https://images.unsplash.com/photo-1603302576837-37561b2e2302?w=500&auto=format&fit=crop"),
        ("Sprite Lemon-Lime Soda (2L)", 2.49, 1.20, 140, "Beverages", "https://images.unsplash.com/photo-1625772299848-391b6a87d7b3?w=500&auto=format&fit=crop"),
        ("Orange (1 lb)", 1.49, 0.70, 90, "Fruits", "https://images.unsplash.com/photo-1563636619-e9143da7973b?w=500&auto=format&fit=crop"),
        ("Pringles Original Potato Crisps", 4.99, 2.60, 170, "Snacks", "https://images.unsplash.com/photo-1626082927389-6cd097cee6a6?w=500&auto=format&fit=crop"),
        ("Cheez-It Baked Snack Crackers", 3.79, 1.90, 135, "Snacks", "https://images.unsplash.com/photo-1613557941766-0b08cc962cb?w=500&auto=format&fit=crop"),
        ("Yogurt (Chobani, Plain, 4-Pack)", 5.49, 3.00, 80, "Dairy", "https://images.unsplash.com/photo-1563636619-e9143da7973b?w=500&auto=format&fit=crop"),
        ("Butter (Kerrygold, Salted, 8oz)", 4.99, 3.20, 65, "Dairy", "https://images.unsplash.com/photo-1563636619-e9143da7973b?w=500&auto=format&fit=crop"),
        ("Croissants (4-Pack)", 5.99, 3.50, 45, "Bakery", "https://images.unsplash.com/photo-1509440159596-0249088772ff?w=500&auto=format&fit=crop"),
        ("Baguette", 2.99, 1.50, 55, "Bakery", "https://images.unsplash.com/photo-1509440159596-0249088772ff?w=500&auto=format&fit=crop"),
        ("TCL 55\" Class 4-Series 4K UHD Smart TV", 299.99, 220.00, 30, "Electronics", "https://images.unsplash.com/photo-1593305841991-05c297ba4575?w=500&auto=format&fit=crop"),
        ("iPad (9th Gen, 64GB)", 329.00, 250.00, 22, "Electronics", "https://images.unsplash.com/photo-1541807084-5c52b6b3adef?w=500&auto=format&fit=crop"),
        ("Zara Woman Blazer", 89.99, 45.00, 28, "Clothing", "https://images.unsplash.com/photo-1591047139829-d91aecb6caea?w=500&auto=format&fit=crop"),
        ("H&M Men's Chinos", 29.99, 15.00, 68, "Clothing", "https://images.unsplash.com/photo-1541099649105-f69ad21f3246?w=500&auto=format&fit=crop"),
        ("Pine-Sol Multi-Surface Cleaner", 5.49, 2.90, 78, "Household", "https://images.unsplash.com/photo-1584820156882-0c59a0e2d0c3?w=500&auto=format&fit=crop"),
        ("Swiffer WetJet Cleaner Starter Kit", 19.99, 12.00, 42, "Household", "https://images.unsplash.com/photo-1584820156882-0c59a0e2d0c3?w=500&auto=format&fit=crop"),
        ("Kit Kat Chocolate Bar", 1.99, 1.00, 200, "Snacks", "https://images.unsplash.com/photo-1603567370845-7a4a2b5d7a6c?w=500&auto=format&fit=crop"),
        ("Gatorade Thirst Quencher", 1.79, 1.00, 150, "Beverages", "https://images.unsplash.com/photo-1625772299848-391b6a87d7b3?w=500&auto=format&fit=crop"),
        ("Listerine Mouthwash", 5.99, 3.20, 90, "Personal Care", "https://images.unsplash.com/photo-1596040033229-a9821f0e50d4?w=500&auto=format&fit=crop"),
        ("Quaker Oats (Old Fashioned)", 4.49, 2.50, 120, "Grocery", "https://images.unsplash.com/photo-1603105090899-1f1d5a0b0d5a?w=500&auto=format&fit=crop"),
        ("Green Bell Pepper", 1.99, 0.90, 75, "Vegetables", "https://images.unsplash.com/photo-1582515073490-39981397c445?w=500&auto=format&fit=crop"),
        ("Red Onion", 1.29, 0.60, 88, "Vegetables", "https://images.unsplash.com/photo-1582515073490-39981397c445?w=500&auto=format&fit=crop"),
        ("Chobani Greek Yogurt (Strawberry)", 1.29, 0.75, 110, "Dairy", "https://images.unsplash.com/photo-1563636619-e9143da7973b?w=500&auto=format&fit=crop"),
        ("Wonder Bread Whole Wheat", 2.79, 1.40, 95, "Bakery", "https://images.unsplash.com/photo-1509440159596-0249088772ff?w=500&auto=format&fit=crop"),
        ("Logitech Wireless Mouse", 24.99, 15.00, 55, "Electronics", "https://images.unsplash.com/photo-1587829741301-dc798b83add3?w=500&auto=format&fit=crop"),
        ("Sony WH-1000XM4 Headphones", 349.99, 250.00, 18, "Electronics", "https://images.unsplash.com/photo-1505740420928-5e560c06d30e?w=500&auto=format&fit=crop"),
        ("Adidas T-Shirt", 22.99, 12.00, 72, "Clothing", "https://images.unsplash.com/photo-1521572163474-6864f9cf17ab?w=500&auto=format&fit=crop"),
        ("Calvin Klein Jeans", 79.99, 45.00, 33, "Clothing", "https://images.unsplash.com/photo-1541099649105-f69ad21f3246?w=500&auto=format&fit=crop"),
        ("Febreze Air Freshener", 3.99, 2.00, 130, "Household", "https://images.unsplash.com/photo-1584820156882-0c59a0e2d0c3?w=500&auto=format&fit=crop"),
        ("Scott Paper Towels (12 Rolls)", 14.99, 9.00, 60, "Household", "https://images.unsplash.com/photo-1584820156882-0c59a0e2d0c3?w=500&auto=format&fit=crop"),
        ("Monopoly Board Game", 24.99, 15.00, 48, "Toys", "https://images.unsplash.com/photo-1633356122102-3fe601e05bd2?w=500&auto=format&fit=crop"),
        ("Crayola Crayons (24-Pack)", 4.49, 2.20, 145, "Toys", "https://images.unsplash.com/photo-1633356122102-3fe601e05bd2?w=500&auto=format&fit=crop"),
        ("The Hunger Games (Book)", 11.99, 6.00, 68, "Books", "https://images.unsplash.com/photo-1544947950-fa07a98d237f?w=500&auto=format&fit=crop"),
        ("Where the Crawdads Sing (Book)", 13.99, 7.50, 72, "Books", "https://images.unsplash.com/photo-1535732820275-9ffd998cac22?w=500&auto=format&fit=crop"),
        ("Iams Proactive Health Cat Food", 24.99, 16.00, 38, "Pet Supplies", "https://images.unsplash.com/photo-1519052537078-e6302a4968d4?w=500&auto=format&fit=crop"),
        ("Greenies Dental Treats for Dogs", 12.99, 7.50, 92, "Pet Supplies", "https://images.unsplash.com/photo-1587300003388-59208cc962cb?w=500&auto=format&fit=crop"),
        ("Aveeno Daily Moisturizing Lotion", 12.49, 7.00, 58, "Personal Care", "https://images.unsplash.com/photo-1596040033229-a9821f0e50d4?w=500&auto=format&fit=crop"),
        ("Gillette Fusion ProGlide Razor", 21.99, 14.00, 44, "Personal Care", "https://images.unsplash.com/photo-1596040033229-a9821f0e50d4?w=500&auto=format&fit=crop"),
        ("Heinz Tomato Ketchup (Large)", 4.99, 2.80, 105, "Condiments", "https://images.unsplash.com/photo-1603302576837-37561b2e2302?w=500&auto=format&fit=crop"),
        ("Sriracha Hot Sauce", 3.49, 1.90, 128, "Condiments", "https://images.unsplash.com/photo-1603302576837-37561b2e2302?w=500&auto=format&fit=crop"),
        ("Mountain Dew Soda (2L)", 2.49, 1.20, 135, "Beverages", "https://images.unsplash.com/photo-1625772299848-391b6a87d7b3?w=500&auto=format&fit=crop"),
        ("Grapes (1 lb)", 2.99, 1.50, 78, "Fruits", "https://images.unsplash.com/photo-1568702846914-96b305d2aaeb?w=500&auto=format&fit=crop"),
        ("Ruffles Original Potato Chips", 3.99, 2.00, 165, "Snacks", "https://images.unsplash.com/photo-1626082927389-6cd097cee6a6?w=500&auto=format&fit=crop"),
        ("Fritos Corn Chips", 3.49, 1.80, 142, "Snacks", "https://images.unsplash.com/photo-1613557941766-0b08cc962cb?w=500&auto=format&fit=crop"),
        ("Almond Milk (Silk, Vanilla, Half Gallon)", 3.99, 2.20, 85, "Dairy", "https://images.unsplash.com/photo-1563636619-e9143da7973b?w=500&auto=format&fit=crop"),
        ("Sour Cream (Tillamook, 16oz)", 3.29, 1.90, 70, "Dairy", "https://images.unsplash.com/photo-1563636619-e9143da7973b?w=500&auto=format&fit=crop"),
        ("Cinnamon Rolls (6-Pack)", 4.99, 2.80, 52, "Bakery", "https://images.unsplash.com/photo-1509440159596-0249088772ff?w=500&auto=format&fit=crop"),
        ("Sourdough Bread Loaf", 4.49, 2.20, 48, "Bakery", "https://images.unsplash.com/photo-1509440159596-0249088772ff?w=500&auto=format&fit=crop"),
        ("Vizio 65\" Class 4K UHD Smart TV", 599.99, 450.00, 15, "Electronics", "https://images.unsplash.com/photo-1593305841991-05c297ba4575?w=500&auto=format&fit=crop"),
        ("MacBook Air M2", 1199.00, 950.00, 12, "Electronics", "https://images.unsplash.com/photo-1496181133205-80b9d7e9b8a6?w=500&auto=format&fit=crop"),
        ("Uniqlo Men's Ultra Light Down Jacket", 69.90, 35.00, 37, "Clothing", "https://images.unsplash.com/photo-1591047139829-d91aecb6caea?w=500&auto=format&fit=crop"),
        ("Gap Women's High Rise Jeans", 59.99, 30.00, 41, "Clothing", "https://images.unsplash.com/photo-1541099649105-f69ad21f3246?w=500&auto=format&fit=crop"),
        ("Method All-Purpose Cleaner", 4.99, 2.70, 82, "Household", "https://images.unsplash.com/photo-1584820156882-0c59a0e2d0c3?w=500&auto=format&fit=crop"),
        ("Bounty Select-A-Size Paper Towels", 9.99, 6.00, 53, "Household", "https://images.unsplash.com/photo-1584820156882-0c59a0e2d0c3?w=500&auto=format&fit=crop")
    };

    private readonly List<(string Name, string Description)> _categoryData = new List<(string, string)>
    {
        ("Beverages", "Drinks and sodas"),
        ("Fruits", "Fresh fruits"),
        ("Snacks", "Chips, crackers, and treats"),
        ("Personal Care", "Toiletries and hygiene products"),
        ("Grocery", "General food items and staples"),
        ("Electronics", "Gadgets and devices"),
        ("Clothing", "Apparel for men, women, and kids"),
        ("Household", "Cleaning supplies and home essentials"),
        ("Toys", "Games and toys for children"),
        ("Books", "Fiction, non-fiction, and educational books"),
        ("Pet Supplies", "Food and accessories for pets"),
        ("Dairy", "Milk, cheese, yogurt, and other dairy products"),
        ("Bakery", "Bread, pastries, and baked goods"),
        ("Condiments", "Sauces, spreads, and seasonings"),
        ("Vegetables", "Fresh vegetables")
    };
    // --- End Product & Category Data ---

    public void PopulateAllTestData()
    {
        Debug.WriteLine("Starting Test Data Population...");

        // --- 1. Populate Categories ---
        Debug.WriteLine("Populating Categories...");
        foreach (var catData in _categoryData)
        {
            var category = new Category { Name = catData.Name, Description = catData.Description };
            App.CategoryRepo.InsertItem(category); // Use your repo method
        }
        Debug.WriteLine($"Inserted {_categoryData.Count} Categories.");

        // --- 2. Populate Products ---
        Debug.WriteLine("Populating Products...");
        var insertedProducts = new List<Product>(); // Keep track for later use
        foreach (var pData in _productData)
        {
            // Find CategoryId by Name (assuming CategoryRepo can query by name or you fetch all categories)
            // For simplicity here, we assume the name match is sufficient for the Product.CategoryName property.
            // If you need the actual Category.Id, you'd fetch the inserted categories first.
            var product = new Product
            {
                Name = pData.Name,
                Price = pData.Price,
                Cost = pData.Cost,
                Stock = pData.Stock,
                CategoryName = pData.CategoryName, // This is the string name
                ImageUrl = pData.ImageUrl,
                DateAdded = GenerateRandomDate(new DateTime(2023, 1, 1), DateTime.Now.AddDays(-30)) // Add some variance
            };
            App.ProductRepo.InsertItem(product); // Use your repo method
            insertedProducts.Add(product); // Store for potential later linking if needed by ID
        }
        Debug.WriteLine($"Inserted {_productData.Count} Products.");


        // --- 3. Populate Contacts ---
        Debug.WriteLine("Populating Contacts...");
        var insertedContacts = new List<Contact>();
        for (int i = 1; i <= 50; i++)
        {
            var contact = new Contact
            {
                Name = GenerateRandomBusinessName(),
                Email = $"contact{i}@{GenerateRandomDomain()}.com",
                PhoneNumber = GenerateRandomPhoneNumber(),
                DateAdded = GenerateRandomDate(new DateTime(2023, 1, 1), DateTime.Now.AddDays(-10)),
                Address = GenerateRandomAddress()
            };
            App.ContactRepo.InsertItem(contact); // Use your repo method
            insertedContacts.Add(contact);
        }
        Debug.WriteLine($"Inserted 50 Contacts.");


        // --- 4. Populate Orders ---
        Debug.WriteLine("Populating Orders...");
        var insertedOrders = new List<Order>();
        var orderDates = GenerateRandomDates(new DateTime(2023, 6, 1), DateTime.Now.AddDays(-5), 50);
        for (int i = 1; i <= 50; i++)
        {
            var contact = insertedContacts[_random.Next(insertedContacts.Count)];
            var orderDate = orderDates[i - 1];

            var order = new Order
            {
                CustomerName = contact.Name,
                Date = orderDate,
                IsConfirmed = _random.NextDouble() > 0.2, // 80% chance confirmed
                ContactId = contact.Id, // Link to contact
                Tax = Math.Round(_random.Next(5, 16) * 1.0, 2), // 5-15%
                Discount = Math.Round(_random.NextDouble() * 20, 2), // Up to $20
                Lines = new List<TransactionLine>() // Initialize lines list
            };

            // Add random lines to the order
            int numLines = _random.Next(1, 6); // 1-5 lines
            var selectedProductsForOrder = insertedProducts.OrderBy(x => _random.Next()).Take(numLines).ToList();

            foreach (var product in selectedProductsForOrder)
            {
                var line = new TransactionLine
                {
                    Name = product.Name,
                    Price = product.Price,
                    Cost = product.Cost,
                    Stock = _random.Next(1, 11), // Quantity 1-10
                    DateAdded = orderDate.AddDays(-_random.Next(1, 5)), // Date added before order date
                    CategoryName = product.CategoryName,
                    ImageUrl = product.ImageUrl
                    // Note: OrderId will be set by SQLite when inserting the order with children
                    // ProductId could be set if needed, but often not required for display
                };
                order.Lines.Add(line);
            }

            // Calculate order totals after lines are added
            order.ItemCount = order.Lines.Count;
            // SubTotalAmount is calculated by the Order class
            // CalculatedTaxAmount is calculated by the Order class
            // CalculatedTotalAmount is calculated by the Order class, but we can also set TotalAmount explicitly if needed
            // Let's use the calculated one for consistency
            // order.TotalAmount = order.CalculatedTotalAmount; // Implicit from SubTotal, Tax, Discount

            App.OrderRepo.InsertItemWithChildren(order); // Use your repo method for orders with lines
            insertedOrders.Add(order);
        }
        Debug.WriteLine($"Inserted 50 Orders with TransactionLines.");


        // --- 5. Populate Transactions ---
        Debug.WriteLine("Populating Transactions...");
        var transactionDates = GenerateRandomDates(new DateTime(2023, 6, 1), DateTime.Now, 50);
        // Assuming CashRegister IDs 1 and 2 exist, or use null
        var cashRegisterIds = new List<int?> { 1, 2, null };

        for (int i = 1; i <= 50; i++)
        {
            var contact = insertedContacts[_random.Next(insertedContacts.Count)];
            var transactionDate = transactionDates[i - 1];
            string type = _random.NextDouble() > 0.8 ? "buy" : "sell"; // 80% sell, 20% buy

            var transaction = new Transaction
            {
                Date = transactionDate,
                TransactionType = type,
                IsPaid = _random.NextDouble() > 0.1, // 90% chance paid
                ContactId = contact.Id, // Link to contact
                CashRegisterId = cashRegisterIds[_random.Next(cashRegisterIds.Count)], // Assign CashRegister
                Lines = new List<TransactionLine>() // Initialize lines list
            };

            // Decide if this transaction uses lines from an existing order or creates new ones
            bool useOrderLines = _random.NextDouble() > 0.5 && insertedOrders.Any();
            List<Product> selectedProductsForTransaction = insertedProducts.OrderBy(x => _random.Next()).Take(_random.Next(1, 8)).ToList(); // 1-7 lines

            if (useOrderLines)
            {
                var orderPool = insertedOrders.Where(o => o.Lines != null && o.Lines.Any()).ToList();
                if (orderPool.Any())
                {
                    var sourceOrder = orderPool[_random.Next(orderPool.Count)];
                    var orderLinesToUse = sourceOrder.Lines.OrderBy(x => _random.Next()).Take(_random.Next(1, Math.Min(sourceOrder.Lines.Count + 1, selectedProductsForTransaction.Count + 1))).ToList();

                    foreach (var orderLine in orderLinesToUse)
                    {
                        // Create a TransactionLine based on the OrderLine
                        var line = new TransactionLine
                        {
                            Name = orderLine.Name,
                            Price = orderLine.Price,
                            Cost = orderLine.Cost,
                            // Potentially sell/buy less than ordered
                            Stock = Math.Min(orderLine.Stock, _random.Next(1, orderLine.Stock + 2)),
                            DateAdded = transactionDate,
                            CategoryName = orderLine.CategoryName,
                            ImageUrl = orderLine.ImageUrl,
                            OrderId = sourceOrder.Id // Link to the source order
                            // ProductId = orderLine.ProductId if available
                        };
                        transaction.Lines.Add(line);
                    }
                }
                else
                {
                    // Fallback if no suitable order found
                    CreateTransactionLines(transaction, selectedProductsForTransaction, transactionDate, type);
                }
            }
            else
            {
                // Create new standalone lines
                CreateTransactionLines(transaction, selectedProductsForTransaction, transactionDate, type);
            }

            // Calculate transaction totals after lines are added
            transaction.ItemCount = transaction.Lines.Count;
            // SubTotalAMount is calculated by the Transaction class based on Type (buy/sell)
            double subTotal = transaction.SubTotalAMount;
            transaction.Tax = Math.Round(subTotal * (_random.Next(5, 16) / 100.0), 2); // 5-15% tax
            transaction.Discount = Math.Round(_random.NextDouble() * Math.Min(30, subTotal * 0.2), 2); // Up to 20% or $30 discount
            transaction.TotalAmount = subTotal - transaction.Discount + transaction.Tax;

            // --- IMPORTANT: Use your specific method to insert transaction with children AND link to contact ---
            // App.TransactionRepo.InsertItemWithChildren(transaction); // Inserts transaction and lines
            // App.ContactRepo.AddNewChildToParentRecursively(contact, transaction, (c, ts) => c.Transactions = ts.ToList()); // Links transaction to contact

            // Combine both actions using your recursive method which handles both:
            App.ContactRepo.AddNewChildToParentRecursively(
                contact,
                transaction,
                (c, ts) => c.Transactions = ts.ToList()
            );
            // This method should internally call InsertWithChildren on the transaction and UpdateWithChildren on the contact.
        }
        Debug.WriteLine($"Inserted 50 Transactions with TransactionLines and linked to Contacts.");

        Debug.WriteLine("Test Data Population Complete!");
    }

    // --- Helper Methods for Generating Random Data ---
    private void CreateTransactionLines(Transaction transaction, List<Product> products, DateTime transactionDate, string transactionType)
    {
        foreach (var product in products)
        {
            var line = new TransactionLine
            {
                Name = product.Name,
                Price = product.Price,
                Cost = product.Cost,
                Stock = _random.Next(1, 11), // Quantity 1-10
                DateAdded = transactionDate,
                CategoryName = product.CategoryName,
                ImageUrl = product.ImageUrl
                // OrderId = 0 or not set for standalone lines
                // ProductId = product.Id if needed
            };
            transaction.Lines.Add(line);
        }
    }

    private DateTime GenerateRandomDate(DateTime start, DateTime end)
    {
        var range = (end - start).Days;
        return start.AddDays(_random.Next(range)).Date.AddHours(_random.Next(8, 19)).AddMinutes(_random.Next(0, 60)); // 8 AM - 6 PM
    }

    private List<DateTime> GenerateRandomDates(DateTime start, DateTime end, int count)
    {
        var dates = new List<DateTime>();
        for (int i = 0; i < count; i++)
        {
            dates.Add(GenerateRandomDate(start, end));
        }
        return dates.OrderBy(d => d).ToList();
    }

    private string GenerateRandomBusinessName()
    {
        string prefix = _firstNames[_random.Next(_firstNames.Count)];
        string suffix = _companySuffixes[_random.Next(_companySuffixes.Count)];
        return $"{prefix} {suffix}";
    }

    private string GenerateRandomDomain()
    {
        var parts = new List<string>();
        for (int i = 0; i < _random.Next(1, 3); i++) // 1 or 2 parts
        {
            parts.Add(_firstNames[_random.Next(_firstNames.Count)].ToLower());
        }
        return string.Join("", parts);
    }

    private string GenerateRandomPhoneNumber()
    {
        return $"555-{_random.Next(100, 999)}-{_random.Next(1000, 9999)}";
    }

    private string GenerateRandomAddress()
    {
        int number = _random.Next(100, 9999);
        string street = _firstNames[_random.Next(_firstNames.Count)];
        string type = _streetTypes[_random.Next(_streetTypes.Count)];
        string city = _lastNames[_random.Next(_lastNames.Count)];
        string state = _firstNames[_random.Next(_firstNames.Count)].Substring(0, 2).ToUpper();
        int zip = _random.Next(10000, 99999);
        return $"{number} {street} {type}, {city}, {state} {zip}";
    }
}