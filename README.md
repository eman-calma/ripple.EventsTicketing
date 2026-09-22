# ripple.EventsTicketing

# Ripple Events Ticketing

A .NET 9 Web API for creating events, defining priced ticket tiers, and selling tickets
— with built-in protection against overselling and duplicate purchases.

## Architecture at a glance

```
Ripple.EventsTicketing.Api              → ASP.NET Core Web API (controllers, DI, middleware)
Ripple.EventsTicketing.Application      → CQRS commands/queries, handlers, validators (business logic)
Ripple.EventsTicketing.Domain           → Entities & enums only, no dependencies
Ripple.EventsTicketing.Infrastructure   → EF Core DbContext, repositories, migrations
Ripple.EventsTicketing.Tests            → xUnit + Moq unit tests
```

Dependencies flow one direction: `Domain ← Infrastructure ← Application ← Api`.

- **Api** — thin controllers that translate HTTP ↔ MediatR requests. No business logic here.
- **Application** — every use case is a MediatR **command** (write, e.g. `CreateEventCommand`)
  or **query** (read, e.g. `GetEventQuery`), each with one handler. FluentValidation
  validators run automatically before every handler via a MediatR pipeline behavior.
- **Domain** — plain entities (`Event`, `PricingTier`, `TicketOrder`, `TicketOrderItem`) and
  the `OrderStatus` enum. No EF Core or business rules here.
- **Infrastructure** — the EF Core `DbContext`, entity configurations, migrations, and
  repositories (`IEventRepository`, `ITicketRepository`, `IReportsRepository`).

Each feature area (`Events`, `Tickets`, `Reports`) under Application follows the same shape:
`Commands/`, `Queries/`, `Handlers/`, `Validators/`.

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- SQL Server (LocalDB, Express, or full) — the default connection string targets
  `localhost\SQLEXPRESS`

## Setup

1. **Clone and restore**

   ```powershell
   dotnet restore
   ```

2. **Point at your SQL Server instance** (optional — a working default is already set)

   Edit the `ConnectionStrings:DefaultConnection` value in
   [Ripple.EventsTicketing.Api/appsettings.json](Ripple.EventsTicketing.Api/appsettings.json)
   or [appsettings.Development.json](Ripple.EventsTicketing.Api/appsettings.Development.json).

3. **Apply database migrations**

   ```powershell
   dotnet ef database update `
     --project Ripple.EventsTicketing.Infrastructure `
     --startup-project Ripple.EventsTicketing.Api
   ```

   (Install the tool once if you don't have it: `dotnet tool install --global dotnet-ef`)

4. **Run the API**

   ```powershell
   dotnet run --project Ripple.EventsTicketing.Api
   ```

   Swagger UI opens at the app's root URL (e.g. `http://localhost:5158/`) and doubles as
   the quickest way to try the endpoints — create an event, then purchase tickets against
   one of its pricing tiers.

5. **Run the tests**

   ```powershell
   dotnet test
   ```

## API surface

| Controller | Endpoint | Purpose |
|---|---|---|
| `EventsController` | `GET /api/events` | List all events |
| | `GET /api/events/{id}` | Get one event with pricing tiers |
| | `POST /api/events` | Create an event |
| | `PUT /api/events/{id}` | Update an event / its pricing tiers |
| | `DELETE /api/events/{id}` | Delete an event (blocked if tickets sold) |
| `TicketsController` | `POST /api/tickets/purchase` | Purchase tickets (idempotent, oversell-safe) |
| | `GET /api/tickets/availability/{eventId}` | Per-tier remaining capacity |
| `ReportsController` | `GET /api/reports/sales` | Ticket sales summary for every event |
| | `GET /api/reports/sales/{eventId}` | Ticket sales summary for one event |
