# 🧠 NeuroPOS - Intelligent Point of Sale System

**NeuroPOS** is a smart, general-purpose Point of Sale (POS) system developed in **.NET MAUI** with a modern offline-first architecture using **SQLite**. Designed to streamline inventory, transactions, cash management, and admin operations, the system is extensible for real-world retail, cafés, wholesale, or mobile selling use cases.

> 🔬 Built as a senior graduation project with practical deployment readiness in mind.

---

## 🛠 Features (Phase 1 - Offline Mode)

- 📦 **Inventory Management**  
  Add, edit, delete, and track products with real-time stock updates.

- 💰 **Transaction Handling**  
  Record sales and purchase transactions with automatic inventory and cash updates.

- 🧾 **Order System (Admin)**  
  Handle pending and confirmed orders with editable items, confirmation dialogs, and inventory sync on approval.

- 📊 **Statistics Dashboard**  
  Visual reports including:
  - Sales over time (last 30 days)
  - Inventory value trends (last 2 months)
  - Cash flow (last 6 months)

- 🧠 **AI Integration**  
  Embedded GPT-4-mini assistant for:
  - Smart inventory queries
  - Report generation
  - Auto-filling product details
  - Interpreting natural language for actions

- 💵 **Cash Register Module**  
  Track daily cash-in and cash-out operations with manual and transaction-linked entries.

- 📱 **Responsive UI with .NET MAUI**  
  Desktop and mobile-ready layouts using Shell navigation and MVVM architecture.

---

## 📁 Project Structure

```plaintext
NeuroPOS/
├── Models/               # Relational SQLite models
├── ViewModels/           # MVVM logic
├── Views/                # MAUI XAML pages
├── Services/
│   ├── SyncService.cs    # (Phase 2) Supabase sync logic
│   └── FirestoreService/ # AI + future sync hooks
├── Utilities/
├── AppShell.xaml         # Navigation structure
├── MainPage.xaml         # Startup logic
└── Resources/
