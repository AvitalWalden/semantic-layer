# Semantic Layer Manager

A mini system for creating and managing a **semantic layer** over a relational
database. It connects to a database, reads its structure dynamically, lets you
manage business metadata on top of it, merges an external metadata file (adding
attributes that do not exist in the database), and exposes the data to business
users through the semantic layer.

- **Backend:** ASP.NET Core (.NET 8), layered architecture, EF Core + Npgsql
- **Frontend:** Angular 21 + Angular Material
- **Database:** PostgreSQL 16
- **Demo domain:** Human Resources (HR)

> For the full design write-up (architecture, data model, sync process, decisions,
> limitations, future work) see [docs/DESIGN.md](docs/DESIGN.md).

---

## 1. What it does

A semantic layer is an abstraction between a physical database and business users.
It presents data in **business terms** (friendly names, descriptions), hides
technical/sensitive columns, and provides a simple, uniform experience.

This system demonstrates one complete, polished scenario end-to-end:

1. **Connect & introspect** – reads `information_schema` of the source `hr` schema.
2. **Manage the semantic layer** – edit business names, descriptions, visibility,
   PII flags and sensitivity per entity/field.
3. **Merge an external metadata file** – enrich fields with attributes not in the
   DB (PII, sensitivity, units, business names) and define **derived (calculated)
   fields** such as `Full Name` and `Annual Salary`.
4. **Explore data through the layer** – business users see only visible fields,
   with business names and derived values (e.g. `ssn` and technical foreign keys
   are hidden).

---

## 2. Quick start (Docker – recommended)

Prerequisites: **Docker Desktop**.

```bash
docker compose up -d --build
```

This starts three containers:

| Service   | URL                                   | Notes                                  |
|-----------|---------------------------------------|----------------------------------------|
| Frontend  | http://localhost:4200                 | Angular app (served by nginx)          |
| Backend   | http://localhost:8080/swagger         | REST API + Swagger UI                  |
| Database  | localhost:5432 (`semantic`/`semantic`)| PostgreSQL, seeded from `db/init.sql`  |

On first boot the backend automatically applies EF Core migrations (creating the
`semantic` schema) and runs an initial schema sync, so the app is usable right away.

To stop and remove everything (including the data volume):

```bash
docker compose down -v
```

---

## 3. Manual run (without Docker)

Prerequisites: **.NET 8 SDK**, **Node.js 20+**, and a **PostgreSQL** instance.

### 3.1 Database

Create a database `semantic_demo` and load the source schema + seed data:

```bash
psql -h localhost -U postgres -c "CREATE DATABASE semantic_demo;"
psql -h localhost -U postgres -d semantic_demo -f db/init.sql
```

### 3.2 Backend

Set the connection string (or edit `backend/src/SemanticLayer.Api/appsettings.json`):

```bash
cd backend
# optional: override connection via environment variable
#   ConnectionStrings__Default="Host=localhost;Port=5432;Database=semantic_demo;Username=postgres;Password=postgres"
dotnet run --project src/SemanticLayer.Api
```

The API listens on http://localhost:8080 (Swagger at `/swagger`). Migrations and
the initial schema sync run automatically on startup.

### 3.3 Frontend

```bash
cd frontend
npm install
npm start
```

The dev server runs at http://localhost:4200 and proxies `/api` to the backend on
port 8080 (see `frontend/proxy.conf.json`).

---

## 4. Demo walkthrough

1. Open the frontend. **Semantic Layer** lists the entities discovered from `hr`
   (Departments, Employees, Job Titles, Salaries).
2. Go to **Sync & Import**:
   - Click **Run schema sync** (idempotent, non-destructive).
   - Click **Choose file**, select `metadata/metadata.json`, then **Merge metadata**.
     This enriches fields (PII/sensitivity/units), hides `ssn` and technical foreign
     keys, and creates derived fields.
3. Open an entity under **Semantic Layer** (e.g. *Employees*) and edit a field or
   the entity itself. Your edits are marked as user-modified and are **protected
   from future syncs**.
4. Go to **Data Explorer**, pick *Employees*, and see the data through the semantic
   layer: business column names, `ssn` hidden, and `Full Name` / `Tenure (Years)`
   computed.

---

## 5. Deliverables map

| Requirement                         | Location                                             |
|-------------------------------------|------------------------------------------------------|
| Full source code                    | `backend/`, `frontend/`                              |
| Install & run instructions          | this file                                            |
| DB creation + sample data           | `db/init.sql`                                         |
| Sample metadata file                | `metadata/metadata.json`                             |
| Design document                     | `docs/DESIGN.md`                                      |
| Runnable end-to-end                 | `docker-compose.yml`                                 |

---

## 6. Main API endpoints

| Method | Path                                   | Purpose                                  |
|--------|----------------------------------------|------------------------------------------|
| POST   | `/api/sync/schema`                     | Non-destructive schema sync              |
| POST   | `/api/sync/metadata`                   | Merge uploaded metadata file (multipart) |
| GET    | `/api/semantic/entities`               | List entities (`?onlyVisible=true`)      |
| GET    | `/api/semantic/entities/{id}`          | Entity with fields                       |
| PUT    | `/api/semantic/entities/{id}`          | Update entity business attributes        |
| PUT    | `/api/semantic/fields/{id}`            | Update field business attributes         |
| GET    | `/api/data/{entityId}?page=&pageSize=` | Data through the semantic layer          |

---

## 7. Project structure

```
semantic-layer/
├─ backend/
│  └─ src/
│     ├─ SemanticLayer.Domain/          # entities, enums
│     ├─ SemanticLayer.Application/     # DTOs, interfaces, services (use-cases)
│     ├─ SemanticLayer.Infrastructure/  # EF Core, Npgsql introspection & queries
│     └─ SemanticLayer.Api/             # controllers, DI, startup
├─ frontend/                            # Angular app (Material)
├─ db/init.sql                          # source schema (hr) + seed data
├─ metadata/metadata.json               # sample external metadata file
├─ docs/DESIGN.md                       # design document
└─ docker-compose.yml
```
