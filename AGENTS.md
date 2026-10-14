# AGENTS.md — project conventions

Pragmatic Clean Architecture on .NET 10. This file is the project's constitution: it outranks any
generic pattern from a plugin. Details live in the project skills listed below.

## Language convention

Output language: Polish for documents and reports (specs, tickets, ADRs, reviews). Code
identifiers, file paths, JSON/YAML keys and commit messages stay English.

Fixed-shape text that a skill defines (template labels such as `**What to build:**` and `**Status:**`,
label strings) stays exactly as the skill writes it. Only prose is Polish. In `GLOSSARY.md`, terms stay
English as in code; definitions are Polish.

## Layers and dependencies

`SharedKernel` ← `Domain` ← `Application` ← `Infrastructure` ← `Web.Api`

- `Domain` references only `SharedKernel`. No EF Core, no Application types.
- `Application` references `Domain` + `SharedKernel`; data access only through `IApplicationDbContext`.
- Persistence details (keys, conversions, relationships) live in `Infrastructure` `IEntityTypeConfiguration<>` classes.
- `Web.Api` endpoints contain no business logic.
- `tests/ArchitectureTests` (NetArchTest) enforces the dependency direction: Domain must not depend on
  Application, Infrastructure or Web.Api; Application must not depend on Infrastructure or Web.Api;
  Infrastructure must not depend on Web.Api. When it fails: **fix the dependency, never the test.**
  The remaining rules (persistence in `Infrastructure` configurations, thin endpoints) are not covered
  by tests — `ca-review` checks them.

## Code conventions

- **Expected failures** return `Result` / `Result<T>` with errors from static `{Entity}Errors` catalogs (`"{Feature}.{Reason}"` codes). Never throw for control flow. `CustomResults.Problem` maps `NotFound→404, Conflict→409, Validation/Problem→400, Forbidden→403, Failure→500`.
- **Use cases** are vertical slices: one folder per use case in `src/Application/{Feature}/{UseCase}/`; custom `ICommandHandler` / `IQueryHandler` (no MediatR), `internal sealed`, primary constructors, registered by Scrutor — never by hand.
- **Validation**: a FluentValidation `{Command}Validator` per new command (`LoginUserCommand` predates this rule), executed by `ValidationDecorator`.
- **Endpoints**: one `IEndpoint` class per use case in `src/Web.Api/Endpoints/{Feature}/`, `.WithTags(Tags.X)`, and `.RequireAuthorization()` or `.HasPermission(...)` — only the auth endpoints (register, login, refresh) are anonymous. `HasPermission` admits every authenticated user today (the permission provider is a stub), so a "403 without the permission" test cannot pass.
- **Ownership**: handlers on user-owned data filter by `IUserContext.UserId` or return `UserErrors.Forbidden()` (ADR 0005). A `userId` the client supplies that is not the current user's → `UserErrors.Forbidden()`, an `ErrorType.Forbidden`, which answers **403** with the error code and description. An item read by its own id → filter by `UserId`, so another user's item answers 404 and doesn't reveal that it exists. Never plan a 401 acceptance criterion for an ownership mismatch.
- **Time**: `IDateTimeProvider`, never `DateTime.UtcNow` / `DateTime.Now` in Application.
- **Caching**: `HybridCache` with keys from a static `{Feature}CacheKeys` class (e.g. `TodoCacheKeys`); every command that mutates cached data invalidates it.
- **Entities**: `sealed`; entities that raise domain events inherit `SharedKernel.Entity` and call `Raise(...)`; public setters are this project's style.
- **Tests**: xUnit + Shouldly + NSubstitute. Handler tests `{Handler}Tests` with methods `Handle_Should_{Outcome}_When{Condition}` on `BaseHandlerTest` (in-memory `TestDbContext`, never mock the DbContext); validator tests (`TestValidate`) per rule; integration tests `{Action}_Should_{Outcome}[_When{Condition}]` over real HTTP with Testcontainers via `BaseIntegrationTest` (`RegisterAndLoginAsync`, `Authenticate`); every handler and integration test marks `// Arrange` `// Act` `// Assert` (`// Arrange` is omitted only when there is no setup).

## Project skills — use them

| Kind of work | Skill |
|---|---|
| New entity / table / aggregate | `add-entity` |
| New use case (command or query + endpoint + tests) | `add-feature` |
| Backfilling tests for an existing use case | `add-tests` |
| Reviewing changes against these conventions | `ca-review` |

## Agent skills

### Issue tracker

Local markdown under `.scratch/<feature-slug>/`. See `docs/agents/issue-tracker.md`.

### Triage labels

The five default role strings (`needs-triage`, `needs-info`, `ready-for-agent`, `ready-for-human`,
`wontfix`). See `docs/agents/triage-labels.md`.

### Domain docs

Single-context: `GLOSSARY.md` and `docs/adr/` at the repo root. See `docs/agents/domain.md`.

### Process skills in this project

- `to-tickets`: one ticket per use case, implemented with `add-feature`; a ticket that needs a new
  entity runs `add-entity` first. Changes to existing use cases that the new one needs go into its
  ticket. Every user story of the spec lands in some ticket's acceptance criteria, including a story the
  existing code already satisfies: its criterion is a regression test.
- `implement`, `tdd`: typechecking is `dotnet build CleanArchitecture.slnx` (analyzers are errors, so
  a warning fails it); a single test file is `dotnet test <test project> --filter
  "FullyQualifiedName~<TestClass>"`; the full suite is `dotnet test CleanArchitecture.slnx`.
- `code-review`: inside `/implement`, review each ticket on the Spec axis only. The Standards axis runs
  once per branch, as `ca-review` of the whole branch before the merge. A `code-review` outside `/implement`
  keeps both axes; its Standards sources are this file and the checklist in `.claude/skills/ca-review/SKILL.md`.

## Build and verification

```bash
dotnet tool restore                      # once per clone: dotnet-ef
dotnet build CleanArchitecture.slnx
dotnet test CleanArchitecture.slnx       # needs Docker: integration tests use Testcontainers (postgres:17)
```

Faster loops: `dotnet test tests/Application.UnitTests`, `dotnet test tests/ArchitectureTests`.
Migrations: `dotnet ef migrations add {PascalCase_With_Underscores} --project src/Infrastructure --startup-project src/Web.Api`.

- **Analyzers are errors** (`AnalysisMode=All`, `TreatWarningsAsErrors`, SonarAnalyzer). Never add a
  suppression: no `#pragma warning disable`, `<NoWarn>`, `[SuppressMessage]`,
  `<TreatWarningsAsErrors>false`, `<CodeAnalysisTreatWarningsAsErrors>false`, `<WarningsNotAsErrors>`,
  analyzers switched off (`<RunAnalyzers>false`, `<EnableNETAnalyzers>false`) or `.editorconfig`
  severity changes. Fix the code. The template's baseline suppressions (`NU1608` in
  `Directory.Build.props`, `EXTEXP0018` in `src/Infrastructure/DependencyInjection.cs` and
  `tests/Application.UnitTests/Abstractions/BaseHandlerTest.cs`, and the existing
  `dotnet_diagnostic.*.severity = none` block in `.editorconfig`) predate this rule — don't add to or
  extend any of them. Tool-generated files (first line
  `// <auto-generated />`, e.g. EF migrations) are exempt; keep them as generated, apart from the header
  line a new migration file needs (see `add-entity`).
- **NuGet audit is on**: a vulnerable (even transitive) package fails the build. Bump the package that
  pulls it in; never suppress the advisory.
- **Central Package Management**: a new package is a `<PackageVersion>` in `Directory.Packages.props`
  plus a `<PackageReference>` without `Version` in the csproj.
- Verification commands must terminate on their own: no `dotnet run` or `docker compose up` as a
  verification step.
