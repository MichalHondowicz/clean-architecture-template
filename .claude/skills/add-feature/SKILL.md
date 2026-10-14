---
name: add-feature
description: Scaffold a complete Clean Architecture feature slice — command or query, custom handler, FluentValidation validator, minimal API endpoint, and tests (unit, validator, integration). Use when the user asks to add a feature, use case, command, query, or endpoint to this Clean Architecture template.
argument-hint: <feature description, e.g. "archive a todo item" or "get todos due this week">
---

# Add a Feature (Vertical Slice)

Scaffold a full use case following this template's conventions: an Application-layer use case with a custom command/query handler, a Web.Api minimal-API endpoint, and tests. No MediatR — this codebase uses its own `ICommand`/`IQuery` abstractions with Scrutor-registered handlers and decorators.

## Workflow

1. **Classify the use case.** A state change is a **command**; a read is a **query**. Derive names from the existing pattern: use case verb + entity, e.g. `ArchiveTodoCommand`, `GetOverdueTodosQuery`.
2. **Check the Domain layer.** If the entity, its `{Entity}Errors` class, or a needed domain event doesn't exist, add it first (see the `add-entity` skill). Commands that change state should raise a domain event via `entity.Raise(...)`. A new property on an existing entity needs a migration (command in `AGENTS.md`, header line in `add-entity`); without it `MigrateAsync` throws `PendingModelChangesWarning` and every integration test fails.
3. **Create the Application slice** in `src/Application/{Feature}/{UseCase}/` — command/query, handler, validator (commands only), response DTO (queries only). Templates: [references/command-slice.md](references/command-slice.md) and [references/query-slice.md](references/query-slice.md).
4. **Create the endpoint** in `src/Web.Api/Endpoints/{Feature}/{UseCase}.cs`. Template: [references/endpoint.md](references/endpoint.md).
5. **Write tests** — handler unit tests, validator tests, and an integration test. Start each red → green cycle at the HTTP seam: the endpoint hides the handler's constructor, so a dependency enters the handler only when a red test demands it (an unused constructor parameter is CS9113, a build error rather than a red). Then cover the failure combinatorics with handler unit tests. Templates: [references/tests.md](references/tests.md).
6. **Verify:** `dotnet build CleanArchitecture.slnx` then `dotnet test CleanArchitecture.slnx`. All three test projects must pass, including `ArchitectureTests` (layer dependency rules).
7. **Red proof — prove the key test can fail.** Green proves nothing until you have seen it red. Pick the mechanism this use case exists for (the guard clause, the ownership filter, the state change) and:
   1. break it deliberately — comment out the guard, drop the `userContext.UserId` filter, skip the assignment. Before breaking, record `sha256sum <file>` (PowerShell `Get-FileHash`). The break must still compile — analyzers are errors. Deleting a clause can leave an unread parameter (CS9113) or a null dereference (CS8602); keep the symbol in use (`_ = userContext;`, `!`). A build error is not a red proof. Never add `#pragma` or `<NoWarn>`, even temporarily. Break one mechanism per run; batch several only when no broken path feeds another test's assertion, since a shared read path fakes the red;
   2. run the test that should catch it and read the FAIL — confirm it fails for that reason, not an unrelated one;
   3. restore by re-editing the exact original text — never `git checkout`, `git restore` or `git stash` (they discard the rest of the uncommitted work) — and confirm the hash matches. `git diff` alone proves nothing: it omits untracked new files. Leave no `.bak` copies;
   4. run again and see green.

   If the test still passes with the mechanism broken, the test is worthless — fix it and repeat. For mapping-dependent behaviour (unique index, cascade) see add-entity "Red proof for mappings".
8. **Report** what you ran (build, which test projects, the red proof with its failure message) and what you did not: tests that could not run leave the work red (e.g. Docker down, so the integration tests never ran) — say so and never report green. Never report a test run you did not see.

## Non-negotiable conventions

- **Folder = use case.** One folder per use case under `src/Application/{Feature}/` (e.g. `Todos/Archive/`), containing all files for that slice.
- **Handlers are `internal sealed`** with primary constructors, implementing `ICommandHandler<TCommand>`, `ICommandHandler<TCommand, TResponse>`, or `IQueryHandler<TQuery, TResponse>`.
- **No manual DI registration.** Handlers, validators, and endpoints are discovered by assembly scanning (`Scrutor`, `AddValidatorsFromAssembly`, `AddEndpoints`). Never touch `DependencyInjection.cs` for a new slice.
- **Return `Result` / `Result<T>`, never throw** for expected failures. Errors come from static factory methods on `{Entity}Errors` in the Domain layer with codes like `"Todos.NotFound"`.
- **Validation lives in a `{Command}Validator`** (FluentValidation). It runs automatically via `ValidationDecorator` — the handler never validates input shape itself. Queries have no validators (the decorator only wraps commands).
- **Data access via `IApplicationDbContext`** (from `Application.Abstractions.Data`) — never reference Infrastructure from Application.
- **Authorization check** in handlers that act on user-owned data: compare `IUserContext.UserId` and return `UserErrors.Unauthorized()` on mismatch, or filter queries by `userContext.UserId` — prefer the `UserId` filter (→ 404). `UserErrors.Unauthorized()` answers 500, so never write a test or AC expecting 401/403 for an ownership mismatch.
- **Queries project directly to a `{X}Response` DTO** with `.Select(...)` — never return domain entities. Cache hot reads with `HybridCache` using a `{Feature}CacheKeys` static class; invalidate in the commands that mutate the cached data.
- **Endpoints** implement `IEndpoint`, resolve the handler interface directly from DI, use `result.Match(Results.Ok, CustomResults.Problem)` (or `Results.NoContent` for void commands), tag with the `Tags` class, and call `.RequireAuthorization()`.

## Naming reference

| Artifact | Pattern | Example |
|---|---|---|
| Command | `{Verb}{Entity}Command` | `ArchiveTodoCommand` |
| Query | `Get{X}Query` | `GetOverdueTodosQuery` |
| Handler | `{Command/Query}Handler` | `ArchiveTodoCommandHandler` |
| Validator | `{Command}Validator` | `ArchiveTodoCommandValidator` |
| Response | `{X}Response` | `TodoResponse` |
| Endpoint | `{UseCase}.cs` in `Endpoints/{Feature}/` | `Endpoints/Todos/Archive.cs` |
| Unit test | `{Handler}Tests` | `ArchiveTodoCommandHandlerTests` |
| Test method | `Handle_Should_{Outcome}_When{Condition}` | `Handle_Should_ReturnNotFound_WhenTodoDoesNotExist` |
