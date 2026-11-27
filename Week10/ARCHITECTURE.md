# Week 10 — Architecture Note

## Why each service uses the data-access approach it does

**Identity — ADO.NET.** This is the login path. It's security-critical (password
hashes, JWT issuance) and its schema barely ever changes — a `Users` table with
a username, a hash, a salt, a role. There's no benefit to an ORM's change-tracking
or LINQ translation here, and a real cost to the extra abstraction layer between
the code and the exact SQL that runs. ADO.NET with parameterized stored procedures
means the SQL is explicit, auditable, and reviewable line-by-line — nothing is
generated at runtime that a security reviewer can't read directly.

**Academics — EF Core Code First.** This is the domain Psiog/the student team
actually owns and keeps extending — Students, Courses, Teachers, and whatever
comes next. Code First is the right fit *because* the schema will keep evolving:
every change is a migration, which gives a reviewable, revertible history of the
schema instead of hand-written ALTER TABLE scripts scattered across tickets.
Rich relationships (a Student enrolled in many Courses, a Teacher owning many
Courses) and seed data are also much less code with EF's fluent API than with
hand-rolled ADO.NET mapping.

**Reporting — EF Core DB First.** This models integrating with a database *owned
by someone else* — the classic "reporting sits on top of an existing warehouse"
situation. The database is authored first (`Sql/reporting-init.sql`, standing in
for a schema a data/reporting team would hand over), and the C# classes in
`Reporting/Data/ReportingContext.cs` describe what EF would produce by running
`Scaffold-DbContext` against it. Reporting never runs migrations — the DB is the
source of truth, and Reporting only reads.

## Honest framing: would a real product do it this way?

No — a real product would standardise on **one** data-access strategy across its
services, almost always EF Core Code First, because mixing three raises the
onboarding cost (new engineers must learn ADO.NET conventions *and* two different
EF workflows) and triples the tooling surface (three different ways to apply a
schema change, three different testing patterns). The one place a second strategy
earns its keep in the real world is Reporting-style integration with a database
your team doesn't own — that's genuinely DB First's reason to exist, not a
preference. Identity could just as easily be EF Code First in practice; it's
ADO.NET here specifically so this capstone demonstrates *why* each pattern fits
where it does, not because three strategies in production is good design.

## Each service owns its database

`IdentityDb`, `AcademicsDb`, and `ReportingDb` are three separate SQL Server
databases (one `sqlserver` container, three databases on it for this capstone's
resource footprint — in production each would likely be its own instance). No
service's code ever opens a connection to another service's database. If
Academics needs enrollment data, it calls Reporting's HTTP API through the
gateway — it does not query `ReportingDb` directly. This is what makes the
services independently deployable and testable: Academics' tests never need
Reporting's schema to exist, and vice versa.

## The gateway's role

React talks to exactly one address: the YARP gateway (`http://localhost:8080`).
The gateway validates the JWT's signature, issuer, audience, and expiry **once**,
centrally, before any request is forwarded — `Gateway.Tests/GatewayAuthTests.cs`
proves a signature produced with the wrong key, or a single bit-flipped
signature, is rejected with 401 at the gateway itself, without ever reaching a
downstream service. Each service *also* re-validates the token and re-checks the
role (`[Authorize(Roles = "Teacher")]`) — this is deliberate defense in depth: if
the gateway were ever bypassed (an internal network misconfiguration, a
forgotten firewall rule), Academics and Reporting still refuse unauthorized
writes on their own.

## Graceful degradation (Task 10.10)

Stopping the Reporting container does not take down the rest of the portal.
`ReportingPage.jsx` fetches its own data and renders its own caught-error state
("Failed to load the enrollment summary.") completely independently of
`StudentsPage.jsx` — Teacher CRUD, Student read-only access, and login/logout
all route through Identity and Academics only, which never call Reporting.
Demonstrate this live with:

```
docker compose -f Week10/docker-compose.yml stop reporting
# portal still works: register, login, Teacher CRUD, Student read-only, logout
# the /reporting page alone shows a friendly error
```

## Keeping the ≥80% gate alive across the split

Each service keeps its *own* xUnit suite, run against EF's InMemory provider
(swapped in via a `UseInMemoryDb` config flag read in each `Program.cs` — see
the comment there for why this needed to be a real process environment variable
rather than a `WithWebHostBuilder` configuration hook) so no suite needs a live
SQL Server to pass:

| Service    | Tests | Coverage | What's excluded from the gate, and why |
|------------|------:|---------:|------------------------------------------|
| Identity   |    13 |     100% | `UserRepository` (ADO.NET) — proven live against real `IdentityDb` in the demo |
| Academics  |    15 |     ~84% | EF `Migrations/` folder — scaffold-generated, not hand-written logic |
| Reporting  |     5 |     ~90% | `vw_EnrollmentSummary` view-backed endpoint — SQL Server-only, proven live |
| Gateway    |     5 |     100% | — |

The Gateway suite adds the one test this week's testing focus calls out
specifically: a tampered JWT (wrong signing key, or a single bit flipped in a
valid signature) is rejected with 401 **at the gateway**, before any attempt to
reach a downstream service.
