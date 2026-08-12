# Support Ticket Management System

A full-stack Support Ticket Management System built as part of the ElectroPi Technical Assessment.

The system allows organizations to create, assign, track, and manage customer support tickets through role-based access for Administrators, Support Agents, and Customers.

---

## 🚀 Technologies

### Backend

- ASP.NET Core Web API
- .NET 8+
- Entity Framework Core
- SQL Server
- JWT Authentication
- Role-Based Authorization
- Swagger / OpenAPI
- Clean Architecture principles
- Dependency Injection
- DTO-based API contracts
- EF Core Migrations

### Frontend

- Angular
- TypeScript
- RxJS
- Reactive Forms
- Bootstrap
- Angular Services
- Route Guards
- HTTP Interceptors

### Additional Technologies / Features

- SignalR
- Optimistic Concurrency
- Unit Testing
- Integration Testing
- Structured Logging

---

# 📋 Features

## Authentication & Authorization

The application uses JWT-based authentication with role-based authorization.

Supported roles:

- Admin
- Support Agent
- Customer

Protected API endpoints ensure that users can only perform actions allowed for their role.

Customer data is isolated to prevent customers from accessing tickets belonging to other customers, including attempts to manipulate ticket IDs through the API.

---

# 🎫 Ticket Management

The system supports:

- Create tickets
- View tickets
- Update tickets
- Delete tickets
- Assign tickets to Support Agents
- Unassign Support Agents
- Change ticket status
- Change ticket priority
- Ticket status transition validation
- Search tickets
- Filter tickets
- Sort tickets
- Pagination

### Ticket Statuses

- Open
- In Progress
- Resolved
- Closed

### Ticket Priorities

- Low
- Medium
- High
- Critical

---

# 💬 Comments & Activity Timeline

Users can add comments to tickets.

The system also tracks ticket-related activities such as:

- Status changes
- Priority changes
- Agent assignment
- Agent unassignment
- Other ticket-related actions

This provides a timeline of changes performed on a ticket.

---

# ⏱️ Time Tracking

Support Agents and Administrators can log time against tickets.

Each ticket can contain multiple time entries.

A time entry contains:

- Work Date
- Duration in Minutes
- Description

The system calculates the total time spent on a ticket based on its time entries.

### Example

```text
Time Entry 1: 60 minutes
Time Entry 2: 30 minutes
Time Entry 3: 45 minutes

Total: 135 minutes
