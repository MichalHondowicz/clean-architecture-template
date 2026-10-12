---
name: add-tests
description: Backfill missing tests for existing use cases in the Clean Architecture template — handler unit tests, FluentValidation validator tests, and HTTP integration tests. Use when the user asks to add, improve, or backfill test coverage.
argument-hint: <use case or feature to cover, e.g. "CopyTodoCommand" or "the Users feature">
---

# Add Tests for an Existing Use Case

Backfill the three test types this template expects for every slice. Read the target handler/validator/endpoint first, then mirror the structure of the closest existing test class.

## Workflow

1. **Locate the slice.** Find the command/query, handler, validator, and endpoint for the target use case. List every distinct outcome: each guard clause (`return Result.Failure(...)`) and the happy path.
2. **Check what already exists** in `tests/Application.UnitTests/{Feature}/` and `tests/IntegrationTests/{Feature}/` — extend existing classes, don't duplicate.
3. **Write handler unit tests** — one test per failure path plus one happy path asserting persisted state and raised domain events.
4. **Write validator tests** (commands only) — one failing test per rule plus one fully-valid command.
5. **Write integration tests** — happy path over real HTTP with state asserted via a follow-up GET; error translation (404/409/400) where the handler has failure paths; for ownership, prefer the `UserId` filter (→ 404) — `UserErrors.Unauthorized()` answers 500, so never write a test or AC expecting 401/403 for an ownership mismatch; 401 without a token if the route family is new.
6. **Run** `dotnet test CleanArchitecture.slnx` (Docker must be running for the integration tests) and fix failures before finishing.
7. **Red proof.** For each new test that defends a failure path, break that path once (comment out the guard clause) and watch the test fail for that reason. Before breaking, record `sha256sum <file>` (PowerShell `Get-FileHash`). The break must still compile — analyzers are errors. Deleting a clause can leave an unread parameter (CS9113) or a null dereference (CS8602); keep the symbol in use (`_ = userContext;`, `!`). A build error is not a red proof. Never add `#pragma` or `<NoWarn>`, even temporarily. Restore by re-editing the exact original text — never `git checkout`, `git restore` or `git stash` (they discard the rest of the uncommitted work) — and confirm the hash matches. `git diff` alone proves nothing: it omits untracked new files. The hash check above is what proves the restore; `git diff` may legitimately show other uncommitted work. Leave no `.bak` copies. A backfilled test that never failed is decoration.

## Conventions

- **Frameworks:** xUnit + Shouldly + NSubstitute; `FluentValidation.TestHelper` for validators. Global usings already cover `Xunit`, `NSubstitute`, `Shouldly`, `SharedKernel`.
- **Unit test base:** inherit `BaseHandlerTest`; use `CreateDbContext()` for a fresh in-memory `TestDbContext` and `CreateCache()` for a real `HybridCache`. Substitute only interfaces (`IUserContext`, `IDateTimeProvider`, `IPasswordHasher`, `ITokenProvider`) — never mock the DbContext.
- **Integration test base:** inherit `BaseIntegrationTest(factory)` with the `IntegrationTestWebAppFactory` collection fixture; use `RegisterAndLoginAsync()` + `Authenticate(token)` for authenticated calls. Define private response DTO records inside the test class (see `TodosTests.TodoDto`).
- **Naming:** classes `{Handler}Tests` / `{Feature}ValidatorsTests` / `{Feature}Tests`; methods `Handle_Should_{Outcome}_When{Condition}` (unit) or `{Action}_Should_{Outcome}[_When{Condition}]` (integration).
- **Structure:** every handler and integration test marks `// Arrange` `// Act` `// Assert` (`// Arrange` omitted only when there is no setup).
- **Assertions:** compare exact domain errors (`result.Error.ShouldBe(TodoItemErrors.NotFound(id))`); assert persisted state by re-reading from the context (unit) or via a GET request (integration); assert domain events with `entity.DomainEvents.ShouldContain(e => e is XDomainEvent)`.

Full annotated templates: [../add-feature/references/tests.md](../add-feature/references/tests.md) (if the `add-feature` skill is installed) or mirror `CreateTodoCommandHandlerTests`, `TodoValidatorsTests`, and `TodosTests` in this repo.

## Tests that pass for the wrong reason

A test that passes while asserting nothing looks identical to a real one in every report. Traps seen in practice:

- **The shared host neutralises what you test.** `IntegrationTestWebAppFactory` raises both rate-limit `PermitLimit`s to 100000 so ordinary tests are not throttled. A test of a limit must build its own host: `factory.WithWebHostBuilder(b => b.UseSetting("RateLimiting:Authentication:PermitLimit", "3")).CreateClient()` (the factory is `sealed`, so no subclass; store `factory` in a field from an explicit constructor — capturing it in a primary constructor while passing it to the base is CS9107). On the shared host it either fails confusingly or — if it only asserts requests succeed — passes and proves nothing.
- **The request is rejected earlier than you think.** A test expects 401 for an expired token and gets 401 — because the hand-crafted token's signature was invalid. Derive keys exactly as production does (`TokenProvider` uses `Encoding.UTF8.GetBytes(Jwt:Secret)`), or obtain tokens through the real `users/login` flow (`RegisterAndLoginAsync`).
- **The fixture is malformed.** A fixture that is invalid in two ways fails for the wrong one. Make fixtures valid in every respect except the one under test.
- **The seam under test isn't the seam exercised.** Integration tests replay migration files (`MigrateAsync`). Editing an `IEntityTypeConfiguration` without a migration changes nothing in the test database; for mapping-dependent behaviour (unique index, cascade) see add-entity "Red proof for mappings".

The check that catches all four: break the one line the test defends and watch it fail (red proof).
