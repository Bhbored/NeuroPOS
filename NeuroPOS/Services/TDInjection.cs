using NeuroPOS.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contact = NeuroPOS.MVVM.Model.Contact;

namespace NeuroPOS.Services
{
    public class TDInjection
    {
        public List<Product> Products { get; set; }
        public List<Category> Categories { get; set; }
        public List<Contact> Contacts { get; set; }


        public void DataGenerator()
        {

            Products = new List<Product>
            {
                new Product { Name = "Coca-Cola Classic Soda (6-Pack)", Price = 4.99, Cost = 2.50, Stock = 120, DateAdded = new DateTime(2023, 10, 15), CategoryName = "Beverages", ImageUrl = "https://images.unsplash.com/photo-1625772299848-391b6a87d7b3?w=500&auto=format&fit=crop" },
                new Product { Name = "Apple Granny Smith", Price = 1.29, Cost = 0.60, Stock = 85, DateAdded = new DateTime(2024, 1, 20), CategoryName = "Fruits", ImageUrl = "https://images.unsplash.com/photo-1568702846914-96b305d2aaeb?w=500&auto=format&fit=crop" },
                new Product { Name = "Lay's Classic Potato Chips", Price = 3.49, Cost = 1.75, Stock = 200, DateAdded = new DateTime(2023, 12, 5), CategoryName = "Snacks", ImageUrl = "https://images.unsplash.com/photo-1626082927389-6cd097cee6a6?w=500&auto=format&fit=crop" },
                new Product { Name = "Nescafé Instant Coffee", Price = 8.99, Cost = 5.00, Stock = 60, DateAdded = new DateTime(2023, 11, 11), CategoryName = "Beverages", ImageUrl = "https://images.unsplash.com/photo-1514432324607-a09d9b4aefdd?w=500&auto=format&fit=crop" },
                new Product { Name = "Colgate Toothpaste", Price = 2.99, Cost = 1.20, Stock = 150, DateAdded = new DateTime(2024, 2, 1), CategoryName = "Personal Care", ImageUrl = "https://images.unsplash.com/photo-1611078489935-0f0e5d3c5d9c?w=500&auto=format&fit=crop" },
                new Product { Name = "Maggi 2-Minute Noodles", Price = 1.50, Cost = 0.75, Stock = 300, DateAdded = new DateTime(2023, 9, 22), CategoryName = "Grocery", ImageUrl = "https://images.unsplash.com/photo-1603105090899-1f1d5a0b0d5a?w=500&auto=format&fit=crop" },
                new Product { Name = "Lifebuoy Handwash Soap", Price = 1.25, Cost = 0.55, Stock = 180, DateAdded = new DateTime(2024, 1, 8), CategoryName = "Personal Care", ImageUrl = "https://images.unsplash.com/photo-1584375524502-9c0b13c4d2b3?w=500&auto=format&fit=crop" },
                new Product { Name = "Samsung 55\" 4K Smart TV", Price = 499.99, Cost = 350.00, Stock = 25, DateAdded = new DateTime(2023, 8, 15), CategoryName = "Electronics", ImageUrl = "https://images.unsplash.com/photo-1593305841991-05c297ba4575?w=500&auto=format&fit=crop" },
                new Product { Name = "iPhone 15 Pro", Price = 1099.00, Cost = 850.00, Stock = 15, DateAdded = new DateTime(2023, 9, 22), CategoryName = "Electronics", ImageUrl = "https://images.unsplash.com/photo-1603302576837-37561b2e2302?w=500&auto=format&fit=crop" },
                new Product { Name = "Levi's 501 Original Jeans", Price = 69.99, Cost = 35.00, Stock = 40, DateAdded = new DateTime(2023, 10, 30), CategoryName = "Clothing", ImageUrl = "https://images.unsplash.com/photo-1541099649105-f69ad21f3246?w=500&auto=format&fit=crop" },
                new Product { Name = "Nike Air Max Sneakers", Price = 129.99, Cost = 75.00, Stock = 30, DateAdded = new DateTime(2023, 11, 15), CategoryName = "Clothing", ImageUrl = "https://images.unsplash.com/photo-1542291026-7eec264c27ff?w=500&auto=format&fit=crop" },
                new Product { Name = "Mr. Clean All Purpose Cleaner", Price = 4.49, Cost = 2.20, Stock = 90, DateAdded = new DateTime(2023, 12, 20), CategoryName = "Household", ImageUrl = "https://images.unsplash.com/photo-1584820156882-0c59a0e2d0c3?w=500&auto=format&fit=crop" },
                new Product { Name = "Barbie Dreamhouse Playset", Price = 199.99, Cost = 120.00, Stock = 20, DateAdded = new DateTime(2023, 7, 10), CategoryName = "Toys", ImageUrl = "https://images.unsplash.com/photo-1566576721346-d4a3b4eaeb55?w=500&auto=format&fit=crop" },
                new Product { Name = "LEGO Creator Expert Millennium Falcon", Price = 849.99, Cost = 650.00, Stock = 5, DateAdded = new DateTime(2023, 6, 1), CategoryName = "Toys", ImageUrl = "https://images.unsplash.com/photo-1637008336770-b95d6a8c9baa?w=500&auto=format&fit=crop" },
                new Product { Name = "Harry Potter and the Goblet of Fire", Price = 12.99, Cost = 6.50, Stock = 75, DateAdded = new DateTime(2023, 8, 25), CategoryName = "Books", ImageUrl = "https://images.unsplash.com/photo-1629992101753-56d196c8aabb?w=500&auto=format&fit=crop" },
                new Product { Name = "Purina ONE Dry Dog Food", Price = 34.99, Cost = 22.00, Stock = 45, DateAdded = new DateTime(2024, 1, 12), CategoryName = "Pet Supplies", ImageUrl = "https://images.unsplash.com/photo-1587300003388-59208cc962cb?w=500&auto=format&fit=crop" },
                new Product { Name = "Pedigree Wet Dog Food Pouches", Price = 1.29, Cost = 0.75, Stock = 120, DateAdded = new DateTime(2023, 10, 5), CategoryName = "Pet Supplies", ImageUrl = "https://images.unsplash.com/photo-1587300003388-59208cc962cb?w=500&auto=format&fit=crop" },
                new Product { Name = "Sunsilk Shampoo", Price = 4.99, Cost = 2.50, Stock = 110, DateAdded = new DateTime(2023, 11, 28), CategoryName = "Personal Care", ImageUrl = "https://images.unsplash.com/photo-1596040033229-a9821f0e50d4?w=500&auto=format&fit=crop" },
                new Product { Name = "Indomie Instant Noodles (Pack of 10)", Price = 3.99, Cost = 2.00, Stock = 250, DateAdded = new DateTime(2023, 9, 18), CategoryName = "Grocery", ImageUrl = "https://images.unsplash.com/photo-1603105090899-1f1d5a0b0d5a?w=500&auto=format&fit=crop" },
                new Product { Name = "Pepsi-Cola Soda (12-Pack)", Price = 5.99, Cost = 3.00, Stock = 100, DateAdded = new DateTime(2023, 12, 10), CategoryName = "Beverages", ImageUrl = "https://images.unsplash.com/photo-1625772452859-10d8f20d7a5c?w=500&auto=format&fit=crop" },
                new Product { Name = "Banana (1 lb)", Price = 0.69, Cost = 0.30, Stock = 95, DateAdded = new DateTime(2024, 2, 5), CategoryName = "Fruits", ImageUrl = "https://images.unsplash.com/photo-1571771894821-ce9b6c11b08e?w=500&auto=format&fit=crop" },
                new Product { Name = "Doritos Nacho Cheese Chips", Price = 4.29, Cost = 2.10, Stock = 180, DateAdded = new DateTime(2023, 11, 2), CategoryName = "Snacks", ImageUrl = "https://images.unsplash.com/photo-1613557941766-0b08cc962cb?w=500&auto=format&fit=crop" },
                new Product { Name = "Milk (1 Gallon)", Price = 3.49, Cost = 2.00, Stock = 150, DateAdded = new DateTime(2024, 1, 25), CategoryName = "Dairy", ImageUrl = "https://images.unsplash.com/photo-1563636619-e9143da7973b?w=500&auto=format&fit=crop" },
                new Product { Name = "Eggs (Dozen)", Price = 2.99, Cost = 1.50, Stock = 200, DateAdded = new DateTime(2024, 1, 30), CategoryName = "Dairy", ImageUrl = "https://images.unsplash.com/photo-1603569283847-aa0c73c2e1d7?w=500&auto=format&fit=crop" },
                new Product { Name = "Bread (Loaf)", Price = 2.49, Cost = 1.20, Stock = 130, DateAdded = new DateTime(2024, 2, 2), CategoryName = "Bakery", ImageUrl = "https://images.unsplash.com/photo-1509440159596-0249088772ff?w=500&auto=format&fit=crop" },
                new Product { Name = "Raspberry Pi 4 Model B", Price = 75.00, Cost = 50.00, Stock = 35, DateAdded = new DateTime(2023, 10, 20), CategoryName = "Electronics", ImageUrl = "https://images.unsplash.com/photo-1610010554923-9a1d70e0f1d4?w=500&auto=format&fit=crop" },
                new Product { Name = "Arduino Uno Rev3", Price = 24.95, Cost = 15.00, Stock = 60, DateAdded = new DateTime(2023, 9, 5), CategoryName = "Electronics", ImageUrl = "https://images.unsplash.com/photo-1587202372775-e229f172b9d3?w=500&auto=format&fit=crop" },
                new Product { Name = "Hanes Men's Crew T-Shirt (3-Pack)", Price = 18.99, Cost = 10.00, Stock = 80, DateAdded = new DateTime(2023, 12, 15), CategoryName = "Clothing", ImageUrl = "https://images.unsplash.com/photo-1521572163474-6864f9cf17ab?w=500&auto=format&fit=crop" },
                new Product { Name = "Oversized Hoodie", Price = 39.99, Cost = 20.00, Stock = 50, DateAdded = new DateTime(2023, 11, 10), CategoryName = "Clothing", ImageUrl = "https://images.unsplash.com/photo-1598033385743-6e9ac6ddd904?w=500&auto=format&fit=crop" },
                new Product { Name = "Lysol Disinfectant Spray", Price = 6.99, Cost = 4.00, Stock = 70, DateAdded = new DateTime(2023, 8, 30), CategoryName = "Household", ImageUrl = "https://images.unsplash.com/photo-1584820156882-0c59a0e2d0c3?w=500&auto=format&fit=crop" },
                new Product { Name = "Clorox Bleach", Price = 3.49, Cost = 1.80, Stock = 95, DateAdded = new DateTime(2023, 10, 12), CategoryName = "Household", ImageUrl = "https://images.unsplash.com/photo-1584820156882-0c59a0e2d0c3?w=500&auto=format&fit=crop" },
                new Product { Name = "Hot Wheels 5-Pack", Price = 6.99, Cost = 3.50, Stock = 110, DateAdded = new DateTime(2023, 11, 22), CategoryName = "Toys", ImageUrl = "https://images.unsplash.com/photo-1633356122102-3fe601e05bd2?w=500&auto=format&fit=crop" },
                new Product { Name = "Play-Doh Modeling Compound 10-Pack", Price = 14.99, Cost = 8.00, Stock = 65, DateAdded = new DateTime(2023, 12, 8), CategoryName = "Toys", ImageUrl = "https://images.unsplash.com/photo-1633356122102-3fe601e05bd2?w=500&auto=format&fit=crop" },
                new Product { Name = "The Subtle Art of Not Giving a F*ck (Book)", Price = 16.99, Cost = 9.00, Stock = 55, DateAdded = new DateTime(2023, 9, 14), CategoryName = "Books", ImageUrl = "https://images.unsplash.com/photo-1535732820275-9ffd998cac22?w=500&auto=format&fit=crop" },
                new Product { Name = "Atomic Habits (Book)", Price = 18.99, Cost = 10.00, Stock = 60, DateAdded = new DateTime(2023, 10, 25), CategoryName = "Books", ImageUrl = "https://images.unsplash.com/photo-1544947950-fa07a98d237f?w=500&auto=format&fit=crop" },
                new Product { Name = "Whiskas Tuna Flavour Cat Food", Price = 1.49, Cost = 0.90, Stock = 140, DateAdded = new DateTime(2023, 11, 18), CategoryName = "Pet Supplies", ImageUrl = "https://images.unsplash.com/photo-1519052537078-e6302a4968d4?w=500&auto=format&fit=crop" },
                new Product { Name = "Temptations Cat Treats", Price = 4.99, Cost = 2.75, Stock = 85, DateAdded = new DateTime(2023, 12, 28), CategoryName = "Pet Supplies", ImageUrl = "https://images.unsplash.com/photo-1519052537078-e6302a4968d4?w=500&auto=format&fit=crop" },
                new Product { Name = "Head & Shoulders Shampoo", Price = 7.49, Cost = 4.00, Stock = 75, DateAdded = new DateTime(2024, 1, 15), CategoryName = "Personal Care", ImageUrl = "https://images.unsplash.com/photo-1596040033229-a9821f0e50d4?w=500&auto=format&fit=crop" },
                new Product { Name = "Dove Beauty Bar Soap (4-Pack)", Price = 5.99, Cost = 3.20, Stock = 125, DateAdded = new DateTime(2023, 10, 8), CategoryName = "Personal Care", ImageUrl = "https://images.unsplash.com/photo-1596040033229-a9821f0e50d4?w=500&auto=format&fit=crop" },
                new Product { Name = "Ketchup (Heinz)", Price = 3.29, Cost = 1.80, Stock = 160, DateAdded = new DateTime(2023, 9, 25), CategoryName = "Condiments", ImageUrl = "https://images.unsplash.com/photo-1603302576837-37561b2e2302?w=500&auto=format&fit=crop" },
                new Product { Name = "Mayonnaise (Hellmann's)", Price = 4.49, Cost = 2.50, Stock = 115, DateAdded = new DateTime(2023, 11, 5), CategoryName = "Condiments", ImageUrl = "https://images.unsplash.com/photo-1603302576837-37561b2e2302?w=500&auto=format&fit=crop" },
                new Product { Name = "Sprite Lemon-Lime Soda (2L)", Price = 2.49, Cost = 1.20, Stock = 140, DateAdded = new DateTime(2024, 1, 5), CategoryName = "Beverages", ImageUrl = "https://images.unsplash.com/photo-1625772299848-391b6a87d7b3?w=500&auto=format&fit=crop" },
                new Product { Name = "Orange (1 lb)", Price = 1.49, Cost = 0.70, Stock = 90, DateAdded = new DateTime(2024, 1, 28), CategoryName = "Fruits", ImageUrl = "https://images.unsplash.com/photo-1563636619-e9143da7973b?w=500&auto=format&fit=crop" },
                new Product { Name = "Pringles Original Potato Crisps", Price = 4.99, Cost = 2.60, Stock = 170, DateAdded = new DateTime(2023, 12, 18), CategoryName = "Snacks", ImageUrl = "https://images.unsplash.com/photo-1626082927389-6cd097cee6a6?w=500&auto=format&fit=crop" },
                new Product { Name = "Cheez-It Baked Snack Crackers", Price = 3.79, Cost = 1.90, Stock = 135, DateAdded = new DateTime(2023, 10, 28), CategoryName = "Snacks", ImageUrl = "https://images.unsplash.com/photo-1613557941766-0b08cc962cb?w=500&auto=format&fit=crop" },
                new Product { Name = "Yogurt (Chobani, Plain, 4-Pack)", Price = 5.49, Cost = 3.00, Stock = 80, DateAdded = new DateTime(2024, 1, 22), CategoryName = "Dairy", ImageUrl = "https://images.unsplash.com/photo-1563636619-e9143da7973b?w=500&auto=format&fit=crop" },
                new Product { Name = "Butter (Kerrygold, Salted, 8oz)", Price = 4.99, Cost = 3.20, Stock = 65, DateAdded = new DateTime(2023, 11, 30), CategoryName = "Dairy", ImageUrl = "https://images.unsplash.com/photo-1563636619-e9143da7973b?w=500&auto=format&fit=crop" },
                new Product { Name = "Croissants (4-Pack)", Price = 5.99, Cost = 3.50, Stock = 45, DateAdded = new DateTime(2024, 2, 3), CategoryName = "Bakery", ImageUrl = "https://images.unsplash.com/photo-1509440159596-0249088772ff?w=500&auto=format&fit=crop" },
                new Product { Name = "Baguette", Price = 2.99, Cost = 1.50, Stock = 55, DateAdded = new DateTime(2024, 2, 4), CategoryName = "Bakery", ImageUrl = "https://images.unsplash.com/photo-1509440159596-0249088772ff?w=500&auto=format&fit=crop" },
                new Product { Name = "TCL 55\" Class 4-Series 4K UHD Smart TV", Price = 299.99, Cost = 220.00, Stock = 30, DateAdded = new DateTime(2023, 7, 20), CategoryName = "Electronics", ImageUrl = "https://images.unsplash.com/photo-1593305841991-05c297ba4575?w=500&auto=format&fit=crop" },
                new Product { Name = "iPad (9th Gen, 64GB)", Price = 329.00, Cost = 250.00, Stock = 22, DateAdded = new DateTime(2023, 9, 12), CategoryName = "Electronics", ImageUrl = "https://images.unsplash.com/photo-1541807084-5c52b6b3adef?w=500&auto=format&fit=crop" },
                new Product { Name = "Zara Woman Blazer", Price = 89.99, Cost = 45.00, Stock = 28, DateAdded = new DateTime(2023, 10, 10), CategoryName = "Clothing", ImageUrl = "https://images.unsplash.com/photo-1591047139829-d91aecb6caea?w=500&auto=format&fit=crop" },
                new Product { Name = "H&M Men's Chinos", Price = 29.99, Cost = 15.00, Stock = 68, DateAdded = new DateTime(2023, 11, 25), CategoryName = "Clothing", ImageUrl = "https://images.unsplash.com/photo-1541099649105-f69ad21f3246?w=500&auto=format&fit=crop" },
                new Product { Name = "Pine-Sol Multi-Surface Cleaner", Price = 5.49, Cost = 2.90, Stock = 78, DateAdded = new DateTime(2023, 9, 8), CategoryName = "Household", ImageUrl = "https://images.unsplash.com/photo-1584820156882-0c59a0e2d0c3?w=500&auto=format&fit=crop" },
                new Product { Name = "Swiffer WetJet Cleaner Starter Kit", Price = 19.99, Cost = 12.00, Stock = 42, DateAdded = new DateTime(2023, 12, 1), CategoryName = "Household", ImageUrl = "https://images.unsplash.com/photo-1584820156882-0c59a0e2d0c3?w=500&auto=format&fit=crop" },
                new Product { Name = "Kit Kat Chocolate Bar", Price = 1.99, Cost = 1.00, Stock = 200, DateAdded = new DateTime(2023, 8, 5), CategoryName = "Snacks", ImageUrl = "https://images.unsplash.com/photo-1603567370845-7a4a2b5d7a6c?w=500&auto=format&fit=crop" },
                new Product { Name = "Gatorade Thirst Quencher", Price = 1.79, Cost = 1.00, Stock = 150, DateAdded = new DateTime(2023, 7, 28), CategoryName = "Beverages", ImageUrl = "https://images.unsplash.com/photo-1625772299848-391b6a87d7b3?w=500&auto=format&fit=crop" },
                new Product { Name = "Listerine Mouthwash", Price = 5.99, Cost = 3.20, Stock = 90, DateAdded = new DateTime(2023, 9, 15), CategoryName = "Personal Care", ImageUrl = "https://images.unsplash.com/photo-1596040033229-a9821f0e50d4?w=500&auto=format&fit=crop" },
                new Product { Name = "Quaker Oats (Old Fashioned)", Price = 4.49, Cost = 2.50, Stock = 120, DateAdded = new DateTime(2023, 10, 22), CategoryName = "Grocery", ImageUrl = "https://images.unsplash.com/photo-1603105090899-1f1d5a0b0d5a?w=500&auto=format&fit=crop" },
                new Product { Name = "Green Bell Pepper", Price = 1.99, Cost = 0.90, Stock = 75, DateAdded = new DateTime(2024, 1, 18), CategoryName = "Vegetables", ImageUrl = "https://images.unsplash.com/photo-1582515073490-39981397c445?w=500&auto=format&fit=crop" },
                new Product { Name = "Red Onion", Price = 1.29, Cost = 0.60, Stock = 88, DateAdded = new DateTime(2024, 1, 21), CategoryName = "Vegetables", ImageUrl = "https://images.unsplash.com/photo-1582515073490-39981397c445?w=500&auto=format&fit=crop" },
                new Product { Name = "Chobani Greek Yogurt (Strawberry)", Price = 1.29, Cost = 0.75, Stock = 110, DateAdded = new DateTime(2024, 1, 29), CategoryName = "Dairy", ImageUrl = "https://images.unsplash.com/photo-1563636619-e9143da7973b?w=500&auto=format&fit=crop" },
                new Product { Name = "Wonder Bread Whole Wheat", Price = 2.79, Cost = 1.40, Stock = 95, DateAdded = new DateTime(2024, 2, 1), CategoryName = "Bakery", ImageUrl = "https://images.unsplash.com/photo-1509440159596-0249088772ff?w=500&auto=format&fit=crop" },
                new Product { Name = "Logitech Wireless Mouse", Price = 24.99, Cost = 15.00, Stock = 55, DateAdded = new DateTime(2023, 11, 8), CategoryName = "Electronics", ImageUrl = "https://images.unsplash.com/photo-1587829741301-dc798b83add3?w=500&auto=format&fit=crop" },
                new Product { Name = "Sony WH-1000XM4 Headphones", Price = 349.99, Cost = 250.00, Stock = 18, DateAdded = new DateTime(2023, 8, 20), CategoryName = "Electronics", ImageUrl = "https://images.unsplash.com/photo-1505740420928-5e560c06d30e?w=500&auto=format&fit=crop" },
                new Product { Name = "Adidas T-Shirt", Price = 22.99, Cost = 12.00, Stock = 72, DateAdded = new DateTime(2023, 12, 22), CategoryName = "Clothing", ImageUrl = "https://images.unsplash.com/photo-1521572163474-6864f9cf17ab?w=500&auto=format&fit=crop" },
                new Product { Name = "Calvin Klein Jeans", Price = 79.99, Cost = 45.00, Stock = 33, DateAdded = new DateTime(2023, 10, 5), CategoryName = "Clothing", ImageUrl = "https://images.unsplash.com/photo-1541099649105-f69ad21f3246?w=500&auto=format&fit=crop" },
                new Product { Name = "Febreze Air Freshener", Price = 3.99, Cost = 2.00, Stock = 130, DateAdded = new DateTime(2023, 11, 30), CategoryName = "Household", ImageUrl = "https://images.unsplash.com/photo-1584820156882-0c59a0e2d0c3?w=500&auto=format&fit=crop" },
                new Product { Name = "Scott Paper Towels (12 Rolls)", Price = 14.99, Cost = 9.00, Stock = 60, DateAdded = new DateTime(2023, 9, 30), CategoryName = "Household", ImageUrl = "https://images.unsplash.com/photo-1584820156882-0c59a0e2d0c3?w=500&auto=format&fit=crop" },
                new Product { Name = "Monopoly Board Game", Price = 24.99, Cost = 15.00, Stock = 48, DateAdded = new DateTime(2023, 12, 12), CategoryName = "Toys", ImageUrl = "https://images.unsplash.com/photo-1633356122102-3fe601e05bd2?w=500&auto=format&fit=crop" },
                new Product { Name = "Crayola Crayons (24-Pack)", Price = 4.49, Cost = 2.20, Stock = 145, DateAdded = new DateTime(2024, 1, 9), CategoryName = "Toys", ImageUrl = "https://images.unsplash.com/photo-1633356122102-3fe601e05bd2?w=500&auto=format&fit=crop" },
                new Product { Name = "The Hunger Games (Book)", Price = 11.99, Cost = 6.00, Stock = 68, DateAdded = new DateTime(2023, 8, 18), CategoryName = "Books", ImageUrl = "https://images.unsplash.com/photo-1544947950-fa07a98d237f?w=500&auto=format&fit=crop" },
                new Product { Name = "Where the Crawdads Sing (Book)", Price = 13.99, Cost = 7.50, Stock = 72, DateAdded = new DateTime(2023, 9, 28), CategoryName = "Books", ImageUrl = "https://images.unsplash.com/photo-1535732820275-9ffd998cac22?w=500&auto=format&fit=crop" },
                new Product { Name = "Iams Proactive Health Cat Food", Price = 24.99, Cost = 16.00, Stock = 38, DateAdded = new DateTime(2023, 10, 17), CategoryName = "Pet Supplies", ImageUrl = "https://images.unsplash.com/photo-1519052537078-e6302a4968d4?w=500&auto=format&fit=crop" },
                new Product { Name = "Greenies Dental Treats for Dogs", Price = 12.99, Cost = 7.50, Stock = 92, DateAdded = new DateTime(2023, 11, 14), CategoryName = "Pet Supplies", ImageUrl = "https://images.unsplash.com/photo-1587300003388-59208cc962cb?w=500&auto=format&fit=crop" },
                new Product { Name = "Aveeno Daily Moisturizing Lotion", Price = 12.49, Cost = 7.00, Stock = 58, DateAdded = new DateTime(2023, 12, 5), CategoryName = "Personal Care", ImageUrl = "https://images.unsplash.com/photo-1596040033229-a9821f0e50d4?w=500&auto=format&fit=crop" },
                new Product { Name = "Gillette Fusion ProGlide Razor", Price = 21.99, Cost = 14.00, Stock = 44, DateAdded = new DateTime(2024, 1, 20), CategoryName = "Personal Care", ImageUrl = "https://images.unsplash.com/photo-1596040033229-a9821f0e50d4?w=500&auto=format&fit=crop" },
                new Product { Name = "Heinz Tomato Ketchup (Large)", Price = 4.99, Cost = 2.80, Stock = 105, DateAdded = new DateTime(2023, 10, 8), CategoryName = "Condiments", ImageUrl = "https://images.unsplash.com/photo-1603302576837-37561b2e2302?w=500&auto=format&fit=crop" },
                new Product { Name = "Sriracha Hot Sauce", Price = 3.49, Cost = 1.90, Stock = 128, DateAdded = new DateTime(2023, 11, 20), CategoryName = "Condiments", ImageUrl = "https://images.unsplash.com/photo-1603302576837-37561b2e2302?w=500&auto=format&fit=crop" },
                new Product { Name = "Mountain Dew Soda (2L)", Price = 2.49, Cost = 1.20, Stock = 135, DateAdded = new DateTime(2024, 1, 10), CategoryName = "Beverages", ImageUrl = "https://images.unsplash.com/photo-1625772299848-391b6a87d7b3?w=500&auto=format&fit=crop" },
                new Product { Name = "Grapes (1 lb)", Price = 2.99, Cost = 1.50, Stock = 78, DateAdded = new DateTime(2024, 1, 31), CategoryName = "Fruits", ImageUrl = "https://images.unsplash.com/photo-1568702846914-96b305d2aaeb?w=500&auto=format&fit=crop" },
                new Product { Name = "Ruffles Original Potato Chips", Price = 3.99, Cost = 2.00, Stock = 165, DateAdded = new DateTime(2023, 12, 25), CategoryName = "Snacks", ImageUrl = "https://images.unsplash.com/photo-1626082927389-6cd097cee6a6?w=500&auto=format&fit=crop" },
                new Product { Name = "Fritos Corn Chips", Price = 3.49, Cost = 1.80, Stock = 142, DateAdded = new DateTime(2023, 10, 31), CategoryName = "Snacks", ImageUrl = "https://images.unsplash.com/photo-1613557941766-0b08cc962cb?w=500&auto=format&fit=crop" },
                new Product { Name = "Almond Milk (Silk, Vanilla, Half Gallon)", Price = 3.99, Cost = 2.20, Stock = 85, DateAdded = new DateTime(2024, 1, 26), CategoryName = "Dairy", ImageUrl = "https://images.unsplash.com/photo-1563636619-e9143da7973b?w=500&auto=format&fit=crop" },
                new Product { Name = "Sour Cream (Tillamook, 16oz)", Price = 3.29, Cost = 1.90, Stock = 70, DateAdded = new DateTime(2023, 12, 1), CategoryName = "Dairy", ImageUrl = "https://images.unsplash.com/photo-1563636619-e9143da7973b?w=500&auto=format&fit=crop" },
                new Product { Name = "Cinnamon Rolls (6-Pack)", Price = 4.99, Cost = 2.80, Stock = 52, DateAdded = new DateTime(2024, 2, 5), CategoryName = "Bakery", ImageUrl = "https://images.unsplash.com/photo-1509440159596-0249088772ff?w=500&auto=format&fit=crop" },
                new Product { Name = "Sourdough Bread Loaf", Price = 4.49, Cost = 2.20, Stock = 48, DateAdded = new DateTime(2024, 2, 6), CategoryName = "Bakery", ImageUrl = "https://images.unsplash.com/photo-1509440159596-0249088772ff?w=500&auto=format&fit=crop" },
                new Product { Name = "Vizio 65\" Class 4K UHD Smart TV", Price = 599.99, Cost = 450.00, Stock = 15, DateAdded = new DateTime(2023, 7, 5), CategoryName = "Electronics", ImageUrl = "https://images.unsplash.com/photo-1593305841991-05c297ba4575?w=500&auto=format&fit=crop" },
                new Product { Name = "MacBook Air M2", Price = 1199.00, Cost = 950.00, Stock = 12, DateAdded = new DateTime(2023, 9, 5), CategoryName = "Electronics", ImageUrl = "https://images.unsplash.com/photo-1496181133205-80b9d7e9b8a6?w=500&auto=format&fit=crop" },
                new Product { Name = "Uniqlo Men's Ultra Light Down Jacket", Price = 69.90, Cost = 35.00, Stock = 37, DateAdded = new DateTime(2023, 10, 25), CategoryName = "Clothing", ImageUrl = "https://images.unsplash.com/photo-1591047139829-d91aecb6caea?w=500&auto=format&fit=crop" },
                new Product { Name = "Gap Women's High Rise Jeans", Price = 59.99, Cost = 30.00, Stock = 41, DateAdded = new DateTime(2023, 11, 29), CategoryName = "Clothing", ImageUrl = "https://images.unsplash.com/photo-1541099649105-f69ad21f3246?w=500&auto=format&fit=crop" },
                new Product { Name = "Method All-Purpose Cleaner", Price = 4.99, Cost = 2.70, Stock = 82, DateAdded = new DateTime(2023, 9, 12), CategoryName = "Household", ImageUrl = "https://images.unsplash.com/photo-1584820156882-0c59a0e2d0c3?w=500&auto=format&fit=crop" },
                new Product { Name = "Bounty Select-A-Size Paper Towels", Price = 9.99, Cost = 6.00, Stock = 53, DateAdded = new DateTime(2023, 12, 15), CategoryName = "Household", ImageUrl = "https://images.unsplash.com/photo-1584820156882-0c59a0e2d0c3?w=500&auto=format&fit=crop" },
            };

            Categories = new List<Category>
        {
            new Category { Name = "Beverages", Description = "Drinks and sodas" },
            new Category { Name = "Fruits", Description = "Fresh fruits" },
            new Category { Name = "Snacks", Description = "Chips, crackers, and treats" },
            new Category { Name = "Personal Care", Description = "Toiletries and hygiene products" },
            new Category { Name = "Grocery", Description = "General food items and staples" },
            new Category { Name = "Electronics", Description = "Gadgets and devices" },
            new Category { Name = "Clothing", Description = "Apparel for men, women, and kids" },
            new Category { Name = "Household", Description = "Cleaning supplies and home essentials" },
            new Category { Name = "Toys", Description = "Games and toys for children" },
            new Category { Name = "Books", Description = "Fiction, non-fiction, and educational books" },
            new Category { Name = "Pet Supplies", Description = "Food and accessories for pets" },
            new Category { Name = "Dairy", Description = "Milk, cheese, yogurt, and other dairy products" },
            new Category { Name = "Bakery", Description = "Bread, pastries, and baked goods" },
            new Category { Name = "Condiments", Description = "Sauces, spreads, and seasonings" },
            new Category { Name = "Vegetables", Description = "Fresh vegetables" }
            };

            Contacts = new List<Contact>
        {
            new Contact { Name = "Acme Corporation", Email = "info@acmecorp.com", PhoneNumber = "555-1212", DateAdded = new DateTime(2023, 5, 10), Address = "123 Main St, Anytown, USA" },
            new Contact { Name = "Beta Solutions Ltd", Email = "contact@betasolutions.com", PhoneNumber = "555-3456", DateAdded = new DateTime(2023, 6, 18), Address = "456 Business Ave, Metropolis, USA" },
            new Contact { Name = "Gamma Industries", Email = "support@gammaind.com", PhoneNumber = "555-7890", DateAdded = new DateTime(2023, 7, 22), Address = "789 Industrial Way, Factville, USA" },
            new Contact { Name = "Delta Retail Group", Email = "orders@deltaretail.com", PhoneNumber = "555-2468", DateAdded = new DateTime(2023, 8, 5), Address = "101 Shop Street, Malltown, USA" },
            new Contact { Name = "Epsilon Services", Email = "hello@epsilonservices.net", PhoneNumber = "555-1357", DateAdded = new DateTime(2023, 9, 12), Address = "202 Service Lane, Helpville, USA" },
            new Contact { Name = "Zeta Tech", Email = "info@zetatech.io", PhoneNumber = "555-9876", DateAdded = new DateTime(2023, 10, 3), Address = "303 Tech Park, Silicon City, USA" },
            new Contact { Name = "Eta Logistics", Email = "dispatch@etalogistics.com", PhoneNumber = "555-4321", DateAdded = new DateTime(2023, 11, 15), Address = "404 Distribution Rd, Logistown, USA" },
            new Contact { Name = "Theta Marketing", Email = "campaigns@thetamarketing.com", PhoneNumber = "555-8642", DateAdded = new DateTime(2023, 12, 8), Address = "505 Ad Avenue, Creativetown, USA" },
            new Contact { Name = "Iota Finance", Email = "billing@iotafinance.com", PhoneNumber = "555-1590", DateAdded = new DateTime(2024, 1, 20), Address = "606 Finance Blvd, Moneyville, USA" },
            new Contact { Name = "Kappa Software", Email = "sales@kappasoftware.com", PhoneNumber = "555-3579", DateAdded = new DateTime(2024, 2, 1), Address = "707 Code Crescent, Devtown, USA" },
            new Contact { Name = "Lambda Consulting", Email = "consult@lambdaconsulting.com", PhoneNumber = "555-2580", DateAdded = new DateTime(2023, 5, 30), Address = "808 Strategy Street, Advisortown, USA" },
            new Contact { Name = "Mu Distribution", Email = "orders@mudistribution.com", PhoneNumber = "555-4671", DateAdded = new DateTime(2023, 6, 25), Address = "909 Wholesaler Way, Bulkburg, USA" },
            new Contact { Name = "Nu Electronics", Email = "support@nuelectronics.com", PhoneNumber = "555-6782", DateAdded = new DateTime(2023, 7, 11), Address = "1010 Circuit City, Gadgetland, USA" },
            new Contact { Name = "Xi Construction", Email = "projects@xiconstruction.com", PhoneNumber = "555-8893", DateAdded = new DateTime(2023, 8, 19), Address = "1111 Builder Blvd, Constructown, USA" },
            new Contact { Name = "Omicron Pharmaceuticals", Email = "info@omicronpharma.com", PhoneNumber = "555-0004", DateAdded = new DateTime(2023, 9, 27), Address = "1212 Health Hwy, Medicville, USA" },
            new Contact { Name = "Pi Educational", Email = "admissions@pieducational.org", PhoneNumber = "555-1115", DateAdded = new DateTime(2023, 10, 14), Address = "1313 Learning Lane, Scholartown, USA" },
            new Contact { Name = "Rho Automotive", Email = "parts@rhoauto.com", PhoneNumber = "555-2226", DateAdded = new DateTime(2023, 11, 21), Address = "1414 Garage Road, Cartown, USA" },
            new Contact { Name = "Sigma Foods", Email = "orders@sigmafoods.com", PhoneNumber = "555-3337", DateAdded = new DateTime(2023, 12, 13), Address = "1515 Culinary Court, Foodieburg, USA" },
            new Contact { Name = "Tau Energy", Email = "billing@tauenergy.com", PhoneNumber = "555-4448", DateAdded = new DateTime(2024, 1, 8), Address = "1616 Power Plant, Voltville, USA" },
            new Contact { Name = "Upsilon Communications", Email = "info@upsiloncomms.com", PhoneNumber = "555-5559", DateAdded = new DateTime(2024, 2, 3), Address = "1717 Signal Street, Connecttown, USA" },
            new Contact { Name = "Phi Manufacturing", Email = "orders@phimanufacturing.com", PhoneNumber = "555-6660", DateAdded = new DateTime(2023, 5, 17), Address = "1818 Factory Floor, Makerburg, USA" },
            new Contact { Name = "Chi Textiles", Email = "sales@chitextiles.com", PhoneNumber = "555-7771", DateAdded = new DateTime(2023, 6, 8), Address = "1919 Fabric Way, Weavetown, USA" },
            new Contact { Name = "Psi Real Estate", Email = "listings@psirealestate.com", PhoneNumber = "555-8882", DateAdded = new DateTime(2023, 7, 3), Address = "2020 Property Place, Hometown, USA" },
            new Contact { Name = "Omega Healthcare", Email = "appointments@omegahealthcare.com", PhoneNumber = "555-9993", DateAdded = new DateTime(2023, 8, 28), Address = "2121 Care Circle, Wellnesstown, USA" },
            new Contact { Name = "Alpha Bank", Email = "business@alphabank.com", PhoneNumber = "555-0101", DateAdded = new DateTime(2023, 9, 5), Address = "2222 Vault View, Banktown, USA" },
            new Contact { Name = "Bravo Airlines", Email = "reservations@bravoairlines.com", PhoneNumber = "555-0202", DateAdded = new DateTime(2023, 10, 22), Address = "2323 Terminal Terrace, Flyburg, USA" },
            new Contact { Name = "Charlie Shipping", Email = "tracking@charlieshipping.com", PhoneNumber = "555-0303", DateAdded = new DateTime(2023, 11, 9), Address = "2424 Port Place, Seattown, USA" },
            new Contact { Name = "Davidson Grocers", Email = "orders@davidsongrocers.com", PhoneNumber = "555-0404", DateAdded = new DateTime(2023, 12, 1), Address = "2525 Market Street, Grocerville, USA" },
            new Contact { Name = "Evans & Partners Law", Email = "info@evanslaw.com", PhoneNumber = "555-0505", DateAdded = new DateTime(2024, 1, 15), Address = "2626 Justice Junction, Courthouse City, USA" },
            new Contact { Name = "Foster Architecture", Email = "designs@fosterarch.com", PhoneNumber = "555-0606", DateAdded = new DateTime(2024, 2, 7), Address = "2727 Blueprint Blvd, Designville, USA" },
            new Contact { Name = "Garcia Imports", Email = "info@garciaimports.com", PhoneNumber = "555-0707", DateAdded = new DateTime(2023, 5, 24), Address = "2828 Import Avenue, Tradetown, USA" },
            new Contact { Name = "Hernandez Exports", Email = "sales@hernandezexports.com", PhoneNumber = "555-0808", DateAdded = new DateTime(2023, 6, 30), Address = "2929 Export Expressway, Global City, USA" },
            new Contact { Name = "Jackson IT Solutions", Email = "support@jacksonits.com", PhoneNumber = "555-0909", DateAdded = new DateTime(2023, 7, 18), Address = "3030 Server Street, Cybertown, USA" },
            new Contact { Name = "King Pharmaceuticals", Email = "info@kingpharma.com", PhoneNumber = "555-1010", DateAdded = new DateTime(2023, 8, 22), Address = "3131 Medicine Mile, Healburg, USA" },
            new Contact { Name = "Lee Electronics", Email = "sales@leeelectronics.com", PhoneNumber = "555-1111", DateAdded = new DateTime(2023, 9, 30), Address = "3232 Gadget Grove, Electronville, USA" },
            new Contact { Name = "Miller Construction Co", Email = "projects@millerconst.com", PhoneNumber = "555-1213", DateAdded = new DateTime(2023, 10, 17), Address = "3333 Build Boulevard, Constructopia, USA" },
            new Contact { Name = "Nelson & Associates", Email = "info@nelsonassoc.com", PhoneNumber = "555-1314", DateAdded = new DateTime(2023, 11, 28), Address = "3434 Professional Plaza, Suitetown, USA" },
            new Contact { Name = "Owens Retail", Email = "orders@owensretail.com", PhoneNumber = "555-1415", DateAdded = new DateTime(2023, 12, 19), Address = "3535 Shopper's Lane, Retailburg, USA" },
            new Contact { Name = "Perez Services Inc", Email = "contact@perezservices.com", PhoneNumber = "555-1516", DateAdded = new DateTime(2024, 1, 22), Address = "3636 Service Street, Helpington, USA" },
            new Contact { Name = "Quinn Tech Solutions", Email = "info@quinntech.com", PhoneNumber = "555-1617", DateAdded = new DateTime(2024, 2, 9), Address = "3737 Innovation Ave, Startuptown, USA" },
            new Contact { Name = "Roberts Manufacturing", Email = "orders@robertsmfg.com", PhoneNumber = "555-1718", DateAdded = new DateTime(2023, 5, 29), Address = "3838 Production Path, Makerfield, USA" },
            new Contact { Name = "Scott Textiles Ltd", Email = "sales@scotttextiles.com", PhoneNumber = "555-1819", DateAdded = new DateTime(2023, 6, 14), Address = "3939 Fiber Road, Weaveburg, USA" },
            new Contact { Name = "Thomas Real Estate", Email = "listings@thomasrealestate.com", PhoneNumber = "555-1920", DateAdded = new DateTime(2023, 7, 25), Address = "4040 Property Place, Homestead, USA" },
            new Contact { Name = "Underwood Healthcare", Email = "appointments@underwoodhc.com", PhoneNumber = "555-2021", DateAdded = new DateTime(2023, 8, 31), Address = "4141 Wellness Way, Healthville, USA" },
            new Contact { Name = "Valdez Financial", Email = "invest@valdezfinancial.com", PhoneNumber = "555-2122", DateAdded = new DateTime(2023, 9, 16), Address = "4242 Finance Forum, Moneyburg, USA" },
            new Contact { Name = "White & Co Accountants", Email = "billing@whiteaccountants.com", PhoneNumber = "555-2223", DateAdded = new DateTime(2023, 10, 27), Address = "4343 Ledger Lane, Auditown, USA" },
            new Contact { Name = "Xavier Logistics", Email = "dispatch@xavierlogistics.com", PhoneNumber = "555-2324", DateAdded = new DateTime(2023, 11, 11), Address = "4444 Distribution Drive, Logistown, USA" },
            new Contact { Name = "Young Marketing", Email = "campaigns@youngmarketing.com", PhoneNumber = "555-2425", DateAdded = new DateTime(2023, 12, 28), Address = "4545 Ad Alley, Creativille, USA" },
            new Contact { Name = "Zimmerman Software", Email = "sales@zimmermansoftware.com", PhoneNumber = "555-2526", DateAdded = new DateTime(2024, 1, 30), Address = "4646 Code Corner, Developolis, USA" }
                };



        }
    }
}
