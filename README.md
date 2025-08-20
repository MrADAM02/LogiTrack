# LogiTrack

Secure and Optimized Inventory & Order Management API

[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

---

## Project Overview

LogiTrack is a RESTful API built with **ASP.NET Core** and **Entity Framework Core** for managing inventory and orders in a logistics system. It supports secure user authentication, role-based access, and performance optimization through caching.

The API allows users to:

- Manage inventory items (add, read, delete)
- Process orders with multiple inventory items
- Securely authenticate and authorize users with roles
- Improve performance with in-memory caching

---

## Features

1. **Inventory Management**: Track item name, quantity, and location.
2. **Order Processing**: Create and manage orders with one-to-many relationships to inventory items.
3. **Authentication & Authorization**: JWT-based authentication and role-based access control (e.g., “Manager” role).
4. **Performance Optimization**: In-memory caching, AsNoTracking queries, and efficient EF Core queries.
5. **DTOs for Data Transfer**: Prevents circular JSON references and ensures clean API responses.

---

## Technologies Used

- **ASP.NET Core 9**
- **Entity Framework Core**
- **SQLite**
- **ASP.NET Identity**
- **IMemoryCache** for caching
- **Swagger** for API testing
- **Microsoft Copilot** for code review and optimization

---

## Project Architecture & Decisions

LogiTrack follows a clean, layered architecture with separation of concerns:

- **Models & DTOs**: InventoryItem and Order represent database entities, while InventoryItemDto and OrderDto are used for safe data transfer and to prevent circular references.
- **Controllers**: InventoryController and OrderController handle API requests, perform validation, apply caching, and return structured responses.
- **Caching & Performance**: Frequently accessed data (inventory list and orders) is cached in memory for 30 seconds. `AsNoTracking()` is used for read-only queries to reduce EF Core overhead.
- **Security**: JWT-based authentication with ASP.NET Identity protects routes, while role-based authorization restricts sensitive operations (e.g., deleting items).
- **State Management**: Orders and inventory are persisted in a SQLite database, ensuring data survives server restarts. Caching provides temporary fast access for repeated queries.
- **Tools & Optimization**: Microsoft Copilot was used to review code, suggest query optimizations, and ensure secure and efficient implementations.

This architecture ensures security, performance, and maintainability, making the API production-ready and easy to extend.

---

## Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [SQLite](https://www.sqlite.org/download.html) (optional, if using local DB)

### Installation

```bash
# Clone the repository
git clone https://github.com/MrADAM02/LogiTrack.git
cd LogiTrack

# Restore NuGet packages
dotnet restore

# Apply EF Core migrations
dotnet ef database update

# Run the project
dotnet run

### API Usage

Authentication

- Register: POST /api/auth/register

- Login: POST /api/auth/login → Returns JWT token

- Include the JWT token in the Authorization header for protected routes:

Authorization: Bearer <your_token>

### Inventory Endpoints

GET /api/inventory → Get all items (cached for 30 seconds)

POST /api/inventory → Add new item

DELETE /api/inventory/{id} → Delete item (Manager only)

GET /api/inventory/timed → Get items and measure query + cache time

### Order Endpoints

GET /api/order → Get all orders (cached for 30 seconds)

GET /api/order/{id} → Get order by ID

POST /api/order → Create new order with multiple items

DELETE /api/order/{id} → Delete order

---

```
### Performance Optimization

- In-Memory Caching for frequently accessed endpoints
- AsNoTracking() for read-only queries
- DTOs to flatten object relationships
- Stopwatch timing to measure improvements

### Security

- JWT authentication with ASP.NET Identity

- Role-based authorization ([Authorize(Roles = "Manager")])

- Input validation through DTOs to prevent over-posting

- Secure password hashing and user management

### Testing

Use Swagger (auto-generated) or Postman to test all routes.
Ensure JWT token is included for protected endpoints.

### Contributing

Contributions are welcome! Please open an issue or submit a pull request.

```

```

```
