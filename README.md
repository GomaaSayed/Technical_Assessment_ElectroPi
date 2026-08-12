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

------------------------
Role Responsibilities
Admin

Admins can:

View all tickets
Assign tickets
Unassign tickets
Change ticket priority
Change ticket status
Manage ticket-related operations
Access dashboard statistics
Support Agent

Support Agents can:

View assigned tickets
Update ticket status
Add comments
Log time entries
View ticket comments
View ticket time entries
Receive real-time SignalR notifications
Customer

Customers can:

Create tickets
View their own tickets
Add comments
Update allowed ticket information
Close resolved tickets

Customers cannot access tickets belonging to other customers.

📝 Assumptions & Limitations
Customers are restricted to their own tickets at the API level.
Support Agents can only access tickets assigned to them through the dedicated "My Tickets" functionality.
Multiple time entries are supported per ticket.
Total ticket time is calculated from the ticket's time entries.
SignalR real-time notifications are currently implemented for Support Agents.
Test credentials are provided through seeded data.
Sensitive credentials and secrets should not be committed to source control.
🔒 Security

The application follows security practices including:

JWT authentication
Role-based authorization
Protected API endpoints
Customer data isolation
Server-side authorization checks
Input validation
DTO-based API contracts
No EF Core entities exposed directly through API responses
Optimistic concurrency
HTTPS support
📦 Deliverables

The repository contains:

Backend source code
Angular frontend
EF Core migrations
Seed data
Automated tests
Swagger/OpenAPI documentation
README documentation
