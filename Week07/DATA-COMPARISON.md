# DATA-COMPARISON.md — Week 7 Data Layer Analysis

## Which approach fits which use case?

---

### 1. Stable Security / Users Table → ADO.NET

**Use case:** A `Users` table that is created once by a DBA, changes rarely (schema-frozen), and whose queries must be audited exactly as written.

**Why ADO.NET fits:**
- Every SQL statement is explicit and can be code-reviewed character-by-character.
- Stored procedures (`usp_GetStudentById`, `usp_InsertStudent`) give the security team one place to audit and lock down permissions — the application account needs only `EXECUTE` on those procs, not `SELECT`/`INSERT` on raw tables.
- Parameterised commands guarantee the injection string `'; DROP TABLE Students;--` is stored as literal text, never executed.
- No ORM overhead; latency is predictable for short, high-frequency queries (e.g., login checks).

**Trade-off:** Schema changes require manual SQL migrations and corresponding C# mapping updates. Acceptable when changes are rare.

**Observation from this week:** Writing the `AdoNetStudentRepository` made the relationship between SQL and C# completely transparent. I could see exactly when a connection opened and closed (every `using` block). The stored-procedure approach (Task 7.4) meant I could test the proc in SSMS in isolation before touching C#.

---

### 2. Evolving Domain (Students) → EF Core Code First

**Use case:** A domain model that changes sprint-to-sprint — adding `EnrolledOn` in Task 7.8 was a one-line C# change followed by two CLI commands.

**Why EF Core Code First fits:**
- The schema *is* the C# model; there is one source of truth.
- `Add-Migration AddEnrolledOn` → `Update-Database` propagated the new column without writing any SQL.
- Fluent API (unique index on `Email`, `HasData` seed) lives next to the model in code, so it is version-controlled and reviewable in PRs.
- LINQ queries are type-safe; renaming a property causes a compile error, not a silent bug.
- The `InMemory` provider lets integration tests run without a real database (see `EfRepositoryIntegrationTests`).

**Trade-off:** EF generates SQL you do not write. For complex reports or bulk updates the generated SQL can be suboptimal and may need `.FromSqlRaw()` as an escape hatch.

**Observation from this week:** Task 7.8 (adding `EnrolledOn`) was the clearest proof of this approach's strength. The whole change was one property line + two terminal commands. The same change in ADO.NET would require: ALTER TABLE SQL, update the INSERT stored proc, update the C# `Map()` method, update every `AddWithValue` call.

---

### 3. Reporting Layer over a DB You Don't Own → EF Core DB First

**Use case:** A read-heavy reporting layer against a database owned by another team. The schema is their responsibility; you only consume it.

**Why EF Core DB First fits:**
- `Scaffold-DbContext` reverse-engineers the exact schema as it exists — inferred keys, relationships, column types — so you never guess.
- When the other team alters their schema, re-running `Scaffold-DbContext --force` regenerates the entities to match.
- LINQ queries over scaffolded entities are type-safe and composable for reports (`.GroupBy`, `.Join`, projections).
- No migration history needed — you do not own the schema, so you do not manage its evolution.

**Trade-off:** The scaffolded entity classes reflect the DB naming conventions, which may differ from your domain language. The mapper in `DbFirstStudentRepository` bridges that gap, but it adds a mapping layer.

**Observation from this week:** The key insight is that the seam (`IRepository<Student>`) made the swap invisible to `StudentsController`. Changing `DataLayer` from `"EfCodeFirst"` to `"DbFirst"` in `appsettings.json` switched the entire persistence layer without touching a single controller line — exactly the point of the exercise.

---

## Summary table

| Criterion                      | ADO.NET          | EF Code First         | EF DB First               |
|-------------------------------|------------------|-----------------------|---------------------------|
| Schema ownership               | DBA / SQL team   | C# developer          | External team             |
| Schema evolution               | Manual SQL + proc updates | `Add-Migration` | Re-scaffold (`--force`)  |
| SQL visibility / auditability  | Full control     | ORM-generated         | ORM-generated             |
| Type safety                    | Runtime strings  | Compile-time LINQ     | Compile-time LINQ         |
| Best for                       | Security-critical, stable tables | Evolving domain model | Read/report against foreign DB |
| Integration testing            | Postman + injection test | EF InMemory provider | EF InMemory provider      |
| Injection safety               | Parameterised cmds + stored procs | EF parameterises automatically | EF parameterises automatically |

---

## What I can now do (Lessons Learnt)

1. **Persist users and CRUD to SQL Server** behind a single `IRepository<Student>` interface.
2. **Write safe ADO.NET** with parameterised queries and stored procedures — the injection string `'; DROP TABLE Students;--` is stored as literal text, never executed.
3. **Generate a database from code** using EF Core Code First migrations and evolve the schema (`EnrolledOn` column via second migration).
4. **Scaffold a Database-First model** from an existing schema with `Scaffold-DbContext`.
5. **Swap all three data layers by configuration** — changing one `appsettings.json` value switches the whole persistence layer without touching controllers.
6. **Unit-test the service against a mocked repository** (Moq) and **integration-test EF** with the InMemory provider.
