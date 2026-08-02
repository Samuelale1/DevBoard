# DevBoard

A project management / issue-tracking API and web client — a stripped-down GitHub Issues / Linear alternative, built end-to-end with **.NET 10**, **PostgreSQL (Supabase)**, and a **React + TypeScript** frontend, with real-time updates via **SignalR**, JWT authentication, background workers, and a full CI/CD pipeline to **Render**.

Live demo: `https://devboard-<your-project>.vercel.app`
API base URL: `https://devboard-api-16ft.onrender.com`
Interactive API docs (Scalar): `https://devboard-api-16ft.onrender.com/scalar/v1`

---

## Table of Contents

- [What is DevBoard?](#what-is-devboard)
- [Architecture](#architecture)
- [Tech Stack](#tech-stack)
- [Project Structure](#project-structure)
- [Getting Started](#getting-started)
  - [Prerequisites](#prerequisites)
  - [Backend Setup](#backend-setup)
  - [Frontend Setup](#frontend-setup)
- [Environment Variables](#environment-variables)
- [Running Tests](#running-tests)
- [Docker](#docker)
- [API Reference](#api-reference)
- [Real-Time Updates (SignalR)](#real-time-updates-signalr)
- [Background Workers](#background-workers)
- [CI/CD Pipeline](#cicd-pipeline)
- [Deployment](#deployment)
- [Notable Design Decisions](#notable-design-decisions)
- [Known Limitations / Roadmap](#known-limitations--roadmap)

---

## What is DevBoard?

DevBoard lets teams create projects, raise issues, assign them, track status through a defined workflow, comment, label, and get **live updates** the moment anything changes — without a page refresh.

Core features:
- **Issue tracking** with a strict, validated state machine (`Backlog → Todo → InProgress → InReview → Done`, plus cancellation and send-back paths)
- **JWT authentication** with rotating refresh tokens
- **Real-time board updates** pushed over SignalR the instant an issue's status changes
- **Audit log streaming** — issue history streamed via `IAsyncEnumerable<T>`, not loaded into memory all at once
- **CSV bulk import** for issues, parsed with `Span<char>` slicing (zero extra string allocations per row)
- **Background workers** — a stale-issue closer, a webhook delivery queue (via `System.Threading.Channels`), and a daily digest job
- **Health checks** for uptime/monitoring
- **Full CI/CD** — every push runs the full test suite; a passing build automatically ships a Docker image to GitHub Container Registry and triggers a Render deploy

---

## Architecture

DevBoard follows **Clean Architecture**: dependencies point inward, and the core business logic has zero knowledge of the database, the web framework, or any external system.

```
┌─────────────────────────────────────────────┐
│                 DevBoard.Api                 │  ASP.NET Core Minimal APIs, JWT auth,
│         (endpoints, middleware, DI root)     │  SignalR hub mapping, Scalar docs
└───────────────────┬───────────────────────────┘
                     │ depends on
┌────────────────────▼──────────────────────────┐
│              DevBoard.Application              │  Services, business workflows,
│   (IIssueService, IAuthService, validators)     │  options, DTOs
└───────────────────┬────────────────┬───────────┘
                     │                │ depends on
┌────────────────────▼───┐  ┌─────────▼──────────┐
│  DevBoard.Infrastructure│  │   DevBoard.Shared   │  Cross-cutting generic
│  EF Core, repositories, │  │   (PagedList<T>)    │  types with zero
│  SignalR hub, workers   │  └─────────┬──────────┘  business logic
└────────────┬────────────┘            │
             │ depends on               │
┌────────────▼─────────────────────────▼──────┐
│                DevBoard.Domain                │  Pure C#. Entities, value objects,
│   (no EF Core, no HTTP, no external deps)     │  the state machine, exceptions —
└────────────────────────────────────────────────┘  zero dependencies on anything above
```

**Why this shape:** `DevBoard.Domain` never references EF Core, ASP.NET Core, or any infrastructure concern — it's pure, dependency-free C#, which is what makes the state machine and entity validation logic unit-testable in isolation, with no database or web server required.

---

## Tech Stack

### Backend
| Concern | Technology |
|---|---|
| Runtime | .NET 10 |
| Web framework | ASP.NET Core Minimal APIs |
| ORM | Entity Framework Core 10 |
| Database | PostgreSQL (via Supabase, using `Npgsql.EntityFrameworkCore.PostgreSQL`) |
| Authentication | JWT Bearer + rotating refresh tokens, `BCrypt.Net-Next` for password hashing |
| Real-time | SignalR (`Microsoft.AspNetCore.SignalR.Core`, via `FrameworkReference`) |
| Validation | FluentValidation (via a custom `IEndpointFilter`, not the deprecated auto-validation pipeline) |
| API docs | Scalar (`Scalar.AspNetCore`) — .NET 10 no longer ships Swagger UI by default |
| Background jobs | `BackgroundService`, `PeriodicTimer`, `System.Threading.Channels` |
| Testing | xUnit, Moq, `WebApplicationFactory` (integration tests against EF Core InMemory) |
| Containerization | Docker (multi-stage build) |
| CI/CD | GitHub Actions → GitHub Container Registry → Render |

### Frontend
| Concern | Technology |
|---|---|
| Framework | React 18 + TypeScript |
| Build tool | Vite |
| Styling | Tailwind CSS v4 (via `@tailwindcss/vite`) |
| Real-time client | `@microsoft/signalr` |
| Hosting | Vercel |

---

## Project Structure

```
DevBoard/
├── src/
│   ├── DevBoard.Domain/          Pure C# — entities, value objects, enums,
│   │                             exceptions, the issue state machine, domain events
│   ├── DevBoard.Shared/          Generic cross-cutting types (PagedList<T>)
│   ├── DevBoard.Infrastructure/  EF Core (DbContext, configurations, migrations),
│   │                             repositories, SignalR hub, background workers
│   ├── DevBoard.Application/     Services, FluentValidation validators' business
│   │                             rules, options classes (Jwt, Smtp, FeatureFlags)
│   └── DevBoard.Api/             Minimal API endpoints, DI composition root,
│                                 middleware, Program.cs
├── tests/
│   ├── DevBoard.Domain.Tests/       Pure unit tests — no DB, no HTTP
│   ├── DevBoard.Application.Tests/  Service tests with Moq
│   └── DevBoard.Api.Tests/          Integration tests via WebApplicationFactory
├── devboardClient/               React + TypeScript + Vite frontend
├── .github/workflows/
│   ├── ci.yml                    Build + full test suite on every push/PR
│   ├── deploy.yml                Builds & pushes Docker image, triggers Render
│   └── keep-alive.yml            Scheduled health-check ping (prevents idling)
├── Dockerfile                    Multi-stage build (SDK → slim runtime image)
└── DevBoard.sln
```

---

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js](https://nodejs.org/) (v20+) and npm
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (optional, for containerized runs)
- A [Supabase](https://supabase.com) project (or any PostgreSQL instance) for the database
- Git

### Backend Setup

```bash
git clone https://github.com/Samuelale1/DevBoard.git
cd DevBoard
dotnet restore
```

**1. Configure local secrets** (never commit real credentials — this uses .NET's user-secrets store, kept outside the repo):

```bash
dotnet user-secrets init --project src/DevBoard.Api
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=devboard;Username=postgres;Password=yourpassword" --project src/DevBoard.Api
dotnet user-secrets set "Jwt:Secret" "a-long-random-64-char-string" --project src/DevBoard.Api
dotnet user-secrets set "Jwt:Issuer" "https://localhost:5001" --project src/DevBoard.Api
dotnet user-secrets set "Jwt:Audience" "devboard-client" --project src/DevBoard.Api
```

> **Note on connection strings:** always use **keyword format** (`Host=...;Port=...;Database=...`), never the `postgresql://...` URI format some providers (including Supabase) show by default — Npgsql's parser does not accept the URI form.

**2. Apply migrations:**

```bash
dotnet ef database update --project src/DevBoard.Infrastructure --startup-project src/DevBoard.Api
```

> In deployed environments, migrations run automatically on startup (`db.Database.MigrateAsync()`, guarded by `IsRelational()` so it's skipped safely under the test suite's InMemory provider).

**3. Run the API:**

```bash
dotnet run --project src/DevBoard.Api
```

The API will start at `https://localhost:5001` (or the port shown in the console). Visit `/scalar/v1` for interactive API docs and `/health` for a liveness check.

### Frontend Setup

```bash
cd devboardClient
npm install
```

Create a `.env` file in `devboardClient/` (no surrounding quotes, no trailing slash):

```
VITE_API_URL=https://localhost:5001
```

Run the dev server:

```bash
npm run dev
```

Open `http://localhost:5173`. Register a new account (this writes directly into your Postgres/Supabase `Users` table), then log in.

---

## Environment Variables

### Backend (`DevBoard.Api`)

| Variable | Example | Notes |
|---|---|---|
| `ConnectionStrings__DefaultConnection` | `Host=...;Port=5432;Database=postgres;Username=...;Password=...;SSL Mode=Require;Trust Server Certificate=true` | Keyword format only. Use Supabase's **Session pooler** (port 5432) for reliable migration support. |
| `Jwt__Secret` | 64+ random characters | HMAC-SHA256 requires at least 256 bits — use a genuinely long, random string. |
| `Jwt__Issuer` | `https://devboard-api-16ft.onrender.com` | Must match the running app's actual public URL. |
| `Jwt__Audience` | `devboard-client` | Arbitrary identifier shared between token issuer and consumer. |
| `FRONTEND_URL` | `https://your-app.vercel.app` | Used for CORS policy configuration. |
| `ASPNETCORE_ENVIRONMENT` | `Production` | Controls Scalar docs visibility, exception detail, etc. |

Double underscores (`__`) represent nested configuration sections — `ConnectionStrings__DefaultConnection` maps to `ConnectionStrings:DefaultConnection` in `appsettings.json`/`IConfiguration`.

### Frontend (`devboardClient`)

| Variable | Example | Notes |
|---|---|---|
| `VITE_API_URL` | `https://devboard-api-16ft.onrender.com` | No trailing slash. Only variables prefixed `VITE_` are exposed to client code by Vite. |

---

## Running Tests

```bash
# All tests
dotnet test

# A specific project
dotnet test tests/DevBoard.Domain.Tests

# A specific test by name
dotnet test --filter "PATCH_ChangeStatus_InvalidTransition_Returns422"
```

Test layout:
- **`DevBoard.Domain.Tests`** — pure logic (state machine transitions, value object validation, `Result<T>`), no database, no HTTP.
- **`DevBoard.Application.Tests`** — service-layer tests with Moq-mocked repositories and a mocked `IHubContext<BoardHub>`.
- **`DevBoard.Api.Tests`** — full-stack integration tests via a custom `WebApplicationFactory`, using EF Core's InMemory provider and injected test JWT configuration — no real database or secrets required to run.

---

## Docker

Build and run the API in a container:

```bash
docker build -t devboard-api .

docker run -p 8080:8080 \
  -e ConnectionStrings__DefaultConnection="Host=host.docker.internal;Port=5432;Database=devboard;Username=postgres;Password=yourpassword" \
  -e Jwt__Secret="a-long-random-64-char-string" \
  -e Jwt__Issuer="http://localhost:8080" \
  -e Jwt__Audience="devboard-client" \
  devboard-api
```

`host.docker.internal` lets the container reach a Postgres instance running on your host machine — `localhost` inside a container refers to the container itself, not your machine.

The `Dockerfile` uses a **multi-stage build**: a full SDK image compiles and publishes the app, then only the compiled output is copied into a much smaller ASP.NET Core runtime image — the final image doesn't carry the SDK's build tooling.

---

## API Reference

Full interactive documentation, generated from the live OpenAPI spec, is available at **`/scalar/v1`** on any running instance. Below is a summary.

### Auth — `/api/auth`
| Method | Path | Auth required | Description |
|---|---|---|---|
| POST | `/register` | No | Create a new user. Body: `{ email, password, displayName, workspaceId }`. Returns access + refresh tokens. |
| POST | `/login` | No | Authenticate. Body: `{ email, password }`. Returns access + refresh tokens. |
| POST | `/refresh` | No | Exchange a valid refresh token for a new token pair. Old refresh token is revoked (rotation). |
| POST | `/revoke` | No | Revoke a refresh token (logout). |

### Projects — `/api/projects` *(requires `Authorization: Bearer <token>`)*
| Method | Path | Description |
|---|---|---|
| GET | `/` | List all projects. |
| POST | `/` | Create a project. Body: `{ name, slug, workspaceId }`. |

### Issues — `/api/issues` *(requires `Authorization: Bearer <token>`)*
| Method | Path | Description |
|---|---|---|
| GET | `/?projectId=&page=&pageSize=` | Paginated list of issues for a project. |
| GET | `/{id}` | Get an issue by ID. |
| GET | `/key/{issueKey}` | Get an issue by its human-readable key (e.g. `PROJ-42`). |
| POST | `/` | Create an issue. Body: `{ projectId, title, description, type, priority }`. Issue key is generated server-side from the project's slug + counter. |
| PATCH | `/{id}/status` | Transition an issue's status. Body: `{ newStatus }`. Returns `422` if the transition is not valid per the state machine. |
| GET | `/{id}/audit-log` | Streams the issue's audit history as a JSON array via `IAsyncEnumerable<T>` — does not load the full history into memory. |
| POST | `/import?projectId=` | Bulk-create issues from an uploaded CSV file (`multipart/form-data`, one issue per line: `Title,Type,Priority`). |

### Health
| Method | Path | Description |
|---|---|---|
| GET | `/health` | Returns `Healthy` if the app and its database connection are both up. Used by the CI keep-alive workflow and uptime monitoring. |

### Valid Issue Status Transitions

```
Backlog    → Todo, Cancelled
Todo       → InProgress, Cancelled
InProgress → InReview, Cancelled
InReview   → Done, InProgress (sent back), Cancelled
Done       → (terminal)
Cancelled  → (terminal)
```

Any transition not listed above is rejected with a `422 Unprocessable Entity`.

---

## Real-Time Updates (SignalR)

Connect to `/hubs/board` with a valid access token:

```typescript
const connection = new signalR.HubConnectionBuilder()
  .withUrl(`${API_URL}/hubs/board`, { accessTokenFactory: () => token })
  .withAutomaticReconnect()
  .build();

connection.on("IssueUpdated", (payload) => { /* { id, status } */ });
await connection.start();
await connection.invoke("JoinProject", projectId);
```

Whenever an issue's status changes via `PATCH /api/issues/{id}/status`, every client subscribed to that project's group receives an `IssueUpdated` event in real time — no polling required.

---

## Background Workers

Three `BackgroundService`s run inside the API process:

| Worker | Schedule | Purpose |
|---|---|---|
| `StaleIssueCloserWorker` | Every 6 hours | Automatically cancels issues that have sat in `InReview` for 30+ days with no activity. |
| `WebhookDeliveryWorker` | Continuous (reads from a bounded `Channel<T>`) | Delivers queued webhook payloads via `IHttpClientFactory`, with retry-safe timeout config. |
| `DailyDigestWorker` | Every 24 hours | Logs a summary of each user's open assigned issues (email delivery not yet wired up — see Roadmap). |

All three correctly resolve scoped services (`IRepository<T>`, `AppDbContext`) via `IServiceScopeFactory.CreateAsyncScope()` rather than injecting them directly — since `BackgroundService` instances are singletons and `DbContext` is scoped, injecting it directly would cause a `DbContext`-disposed/cross-request-sharing bug.

---

## CI/CD Pipeline

Three GitHub Actions workflows:

1. **`ci.yml`** — runs on every push/PR to `main`: restores, builds, and runs the **entire** test suite (unit, service-level, and full integration tests). Since `DevBoard.Api.Tests` uses EF Core's InMemory provider and injects test JWT config directly, this runs with zero external dependencies — no real database, no secrets needed in CI.
2. **`deploy.yml`** — triggered only after `ci.yml` completes successfully. Builds the Docker image, pushes it to GitHub Container Registry (`ghcr.io/samuelale1/devboard-api:latest` — note: lowercase, since Docker registry names reject uppercase), then calls Render's deploy hook to redeploy.
3. **`keep-alive.yml`** — a scheduled job (Mondays and Thursdays) plus manual trigger, pings `/health` to prevent the free-tier Render/Supabase instances from idling out.

---

## Deployment

- **API** — Docker image on **Render**, connected to Supabase via the Session pooler connection string. Environment variables configured directly in Render's dashboard (see [Environment Variables](#environment-variables)).
- **Database** — **Supabase** (managed PostgreSQL). Schema is created/updated automatically on app startup via `MigrateAsync()`.
- **Frontend** — **Vercel**, built from `devboardClient/`, pointed at the Render API URL via `VITE_API_URL`.

CORS is configured on the API to allow only the deployed frontend's origin (`FRONTEND_URL` env var), plus `localhost:5173` for local development.

---

## Notable Design Decisions

- **`PagedList<T>` lives in a separate `DevBoard.Shared` project**, rather than duplicated in both `Infrastructure` and `Application` — a deliberate "shared kernel" pattern for a generic type with zero business logic, kept explicitly out of `Domain` to avoid coupling the pure domain layer to a presentation/pagination concern.
- **`BoardHub` (SignalR) lives in `DevBoard.Infrastructure`, not `Api`** — this avoids a circular project reference, since `Application`-layer services need to push hub notifications and `Application` already depends on `Infrastructure`.
- **Validation uses a custom `IEndpointFilter`, not `FluentValidation.AspNetCore`** — that package is deprecated as of FluentValidation v12, with its auto-validation pipeline removed. A generic `ValidationFilter<T>` resolves the matching `IValidator<T>` from DI and short-circuits with a Problem Details response on failure.
- **Exception-to-status-code mapping is centralized** — every domain exception inherits `DevBoardException` and declares its own `abstract int StatusCode`, so the global exception handler needs a single `DevBoardException dbEx => (dbEx.StatusCode, dbEx.Message)` switch arm rather than enumerating every exception type individually.
- **Repositories use `Query()` returning `IQueryable<T>`**, not a pre-materialized list — this is what allows `IssueQueryExtensions` (`.ForProject().WithStatus().OrderedByPriority()`) to compose LINQ filters that translate to a single SQL query, rather than loading a full table into memory and filtering client-side.

---

## Known Limitations / Roadmap

- `DailyDigestWorker` currently logs digest summaries rather than sending real emails — `IEmailService` is defined but not yet implemented against a real SMTP provider.
- No role-based authorization enforcement yet beyond authentication (`UserRole.Admin`/`Member`/`Viewer` exist on the domain model but aren't yet wired into endpoint policies).
- Webhook registration/management endpoints are not yet exposed — the delivery worker and channel exist, but nothing currently enqueues payloads.
- Frontend is intentionally minimal (a working proof of the full stack, not a polished production UI) — issue creation, labels, comments, and attachments have backend support not yet reflected in the React client.
