# TrueStoryWebApi

A simple ASP.NET Core Web API for managing products, demonstrating CRUD operations, pagination, filtering, and robust error handling.

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- (Optional) Visual Studio 2022 or later

## Setup

1. **Clone the repository:**
git clone <your-repo-url> cd TrueStoryWebApi

2. **Restore dependencies:**
dotnet restore

3. **Build the project:**
dotnet build

## Running the API

- **From the command line:**
dotnet run --project TrueStoryWebApi

- **From Visual Studio:**
	- Open the solution file `TrueStoryWebApi.sln`
	- Set the `TrueStoryWebApi` project as the startup project
	- Press `F5` to run the project

The API will be available at `https://localhost:5001` (or as shown in the console output).

## API Documentation

Swagger UI is enabled in development mode for interactive API exploration.

- Visit: `https://localhost:5001/swagger`

### Main Endpoints

| Method | Route                | Description                        |
|--------|----------------------|------------------------------------|
| GET    | `/Product`           | List products (supports filtering, paging) |
| GET    | `/Product/{id}`      | Get a product by ID                |
| POST   | `/Product`           | Create a new product               |
| PUT    | `/Product/{id}`      | Update an existing product         |
| DELETE | `/Product/{id}`      | Delete a product                   |

#### Query Parameters for GET `/Product`

- `name` (string, optional): Filter by product name (case-insensitive, partial match)
- `page` (int, optional): Page number (default: 1)
- `pageSize` (int, optional): Page size (default: 10, max: 50)

#### Example Product Object
{ "id": 1, "name": "Sample Product", "data": { "color": "red", "price": 100 } }

## Error Handling

All errors return a consistent JSON response:
{ "status": 404, "error": "Not Found", "message": "The specified resource was not found.", "path": "/Product/123", "timestamp": "2025-05-21T12:34:56Z" }

- External API errors return a `502 Bad Gateway` with a relevant message.
- Validation and internal errors are handled with appropriate status codes and messages.
