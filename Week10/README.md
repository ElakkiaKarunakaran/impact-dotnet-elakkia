# Week 10 — Microservices Capstone

Student/Teacher portal split into three microservices (Identity, Academics,
Reporting) fronted by a YARP API gateway, orchestrated with docker-compose.
See [ARCHITECTURE.md](./ARCHITECTURE.md) for why each service uses the
data-access strategy it does.

## Run it

```bash
docker compose -f Week10/docker-compose.yml up --build
```

| What                     | URL                              |
|--------------------------|-----------------------------------|
| React portal             | http://localhost:5173            |
| API gateway              | http://localhost:8080             |
| Identity (direct)        | http://localhost:8081/swagger    |
| Academics (direct)       | http://localhost:8082/swagger    |
| Reporting (direct)       | http://localhost:8083/swagger    |
| SQL Server               | localhost,14330 (sa / Your_strong_Password123) |

First boot takes a minute: SQL Server starts, `init-db` provisions `IdentityDb`
and `ReportingDb` (stored procs + view), then Academics applies its own EF
migrations on startup. Stop everything with `docker compose -f Week10/docker-compose.yml down -v`
(the `-v` also drops the SQL Server data volume, for a clean re-run).

## Demo script (Task 10.8)

1. Open http://localhost:5173/register — register a new Teacher account.
2. Log in with that account → lands on `/students`.
3. Add a student (Teacher-only button) → appears in the list immediately.
4. Log out, register/log in as a Student → no Add/Edit/Delete buttons.
5. Click **Reporting** in the nav → enrollment summary served by the
   Reporting microservice (EF DB First) through the gateway.
6. Log out.
7. **Negative path:** as the Student, try a direct `POST` to
   `http://localhost:8080/academics/api/students` (e.g. via curl/Postman) —
   the gateway forwards it, Academics' `[Authorize(Roles="Teacher")]` rejects
   it with `403 Forbidden`.
8. **Stretch (Task 10.10):** `docker compose -f Week10/docker-compose.yml stop reporting`,
   then show the rest of the portal (login, CRUD, logout) still works —
   only the Reporting page shows a friendly error.

## Tests

```bash
dotnet test Week10/Identity.Tests
dotnet test Week10/Academics.Tests
dotnet test Week10/Reporting.Tests
dotnet test Week10/Gateway.Tests
```

None of these need a live SQL Server or running containers — each service
swaps in EF's InMemory provider (or mocks the one ADO.NET repository) for its
own suite. See ARCHITECTURE.md for the per-service coverage numbers and what's
excluded from the ≥80% gate.

## Layout

```
Week10/
  Identity/        ADO.NET auth service (register, login, PBKDF2, JWT issuance)
  Academics/        EF Code First — Students, Courses, Teachers CRUD
  Reporting/        EF DB First — read-only enrollment views
  Gateway/           YARP reverse proxy, central JWT validation
  Identity.Tests/
  Academics.Tests/
  Reporting.Tests/
  Gateway.Tests/
  Sql/               Hand-authored init scripts for IdentityDb + ReportingDb
  web/               Dockerfile/nginx/env.sh for the React client (reused from Week08)
  docker-compose.yml
  ARCHITECTURE.md
```

The React client itself still lives in `Week08/student-portal` — Week 10 adds
a runtime `window.__ENV__` config (injected by `web/env.sh` at container start)
so the exact same build can point at either the Week07 monolith directly or
this gateway, and a new `/reporting` page. Nothing from Week 8/9 was rewritten.
