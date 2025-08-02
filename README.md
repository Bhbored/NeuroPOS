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

<img width="1918" height="1017" alt="image" src="https://github.com/user-attachments/assets/fb7e8155-a020-48e4-a853-25046fb5b3ad" />
<img width="1912" height="986" alt="image" src="https://github.com/user-attachments/assets/6103902f-e94f-41aa-9489-7a4b583183a4" />
<img width="1918" height="1016" alt="image" src="https://github.com/user-attachments/assets/d395ae6a-593f-4712-9d26-55e8b082d5a5" />
<img width="1918" height="1050" alt="image" src="https://github.com/user-attachments/assets/417ed8b1-ba9d-4eb2-82ce-2f88376adc5b" />
<img width="1900" height="963" alt="image" src="https://github.com/user-attachments/assets/127f7bb4-66b2-4599-a041-185e0886992e" />
<img width="1918" height="1021" alt="image" src="https://github.com/user-attachments/assets/3554fe51-0680-434c-ad50-276212dd94c1" />
<img width="1918" height="1015" alt="image" src="https://github.com/user-attachments/assets/39b2c853-96a5-4a0a-ba6b-89e49352bc23" />
<img width="1910" height="972" alt="image" src="https://github.com/user-attachments/assets/2fab6b38-565c-4ed9-ab2f-3d685a626c43" />
<img width="1918" height="996" alt="image" src="https://github.com/user-attachments/assets/dc465a0a-4b13-404c-ab44-0b6f700ec05a" />
<img width="1917" height="981" alt="image" src="https://github.com/user-attachments/assets/0b5d7891-8d4b-4466-b0bc-6aa85392ba01" />
<img width="1918" height="1012" alt="image" src="https://github.com/user-attachments/assets/338273db-106f-4787-b31e-06df15325da1" />
<img width="1917" height="971" alt="image" src="https://github.com/user-attachments/assets/9efb6c85-5b8b-41bc-92cb-2a81674a93e4" />
<img width="1918" height="988" alt="image" src="https://github.com/user-attachments/assets/5bd08f3e-fe34-4f6f-a47d-6f7dbd172c26" />

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

🚀 Phase 2
🔐 Supabase Auth Integration

Sync local SQLite with cloud

Auto-clear on logout, auto-import on login

📄 PDF Invoice Generator


📈 AI-Powered Insights
Forecasting, recommendations, low-stock alerts, fraud detection, etc.

🧑‍💻 Tech Stack
Area	Tech / Tool
UI Framework	.NET MAUI
Language	C#
Database	SQLite (local)
Charts	Syncfusion + MAUI Charts
AI Assistant	OpenAI GPT-4-mini API
Architecture	MVVM + Shell Navigation
Future Sync	Supabase (REST & Auth)

🧠 Highlights
No boilerplate generators – fully hand-coded for maximum understanding

Designed for both coursework and real-world use

Extensible architecture to support future cloud and AI features

📸 Screenshots

<img width="1913" height="965" alt="image" src="https://github.com/user-attachments/assets/db5f664b-560b-4b8e-8dd7-e1b5771073f2" />
<img width="806" height="827" alt="image" src="https://github.com/user-attachments/assets/79548d80-ad76-40fd-b6f1-ef55475bc8e2" />
<img width="713" height="802" alt="image" src="https://github.com/user-attachments/assets/a25dcfe7-7f36-4e2d-a505-397d1f49b714" />
<img width="591" height="700" alt="image" src="https://github.com/user-attachments/assets/5b4a391a-3e18-498f-9afd-3d7687380465" />

📝 License
MIT License
© 2025 NeuroPOS Team


