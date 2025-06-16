# 7-Eleven Smart Inventory System

## Features

### Authentication & Authorization
- JWT authentication for APIs
- Cookie-based login for web UI
- Role-based access (Store Operator, Store Manager, API Client)

### Inventory & Sales Management
- Track real-time stock per store
- Log sales with quantity, unit price, and sale date
- Automatically reduce inventory upon sales

### Smart Reorder Suggestion Algorithm
- Calculates optimal reorder quantity using:
  - 30-day average daily sales
  - Seasonality factor (weekend/weekday)
  - Safety stock + lead time
- Applies business rules:
  - Rounds to nearest 10 units
  - Respects max storage limits

### ABC Analysis
- Categorizes products by revenue contribution:
  - A: Top 80%
  - B: Next 15%
  - C: Final 5%
- Displayed with a Chart.js bar chart in dashboard UI

### API Endpoints
- /api/auth/login |  POST |  Login and receive JWT token
- /api/inventory/{storeId} | GET | Get current inventory for a store 
- /api/sales/transaction | POST | Record a sales transaction 
- /api/algorithms/reorder-recommendations/{storeId} | GET | Smart reorder suggestions 
- /api/algorithms/abc-analysis/{storeId} | GET | ABC product classification

## UI Pages

- /Account/Login | Secure Login Form
- /Home/Index | Dashboard with low stock, reorder suggestions and ABC Chart

## Technologies & Libraries Used
- ASP .NET Core 8
- Entity Framework Core
- Microsoft SQL Server
- JWT
- Chart.js
- Bootstrap

## Seeded Data (on first run)
- 3 Stores : Colpetty, Fort Jaffna
- 30 Products
- Inventory (Initial stock of 100 per product per store)
- Users : Admin (Username - admin, Password - admin123)

## How to Run
1. Clone the repository
2. Run the App
3. Database is automatically created on the first launch

## Project Structure
- Controllers
- Data (DbContext + SeedData)
- Models
- Views (MVC UI)

## Security
- API access is protected using JWT
- UI access is restricted by cookie auth and [Authorize] filters
- Passwords are hashed using the PasswordHasher

- 
