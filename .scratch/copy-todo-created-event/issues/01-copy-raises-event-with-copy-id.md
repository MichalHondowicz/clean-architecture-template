# 01: Copy publikuje zdarzenie utworzenia z id kopii

**What to build:** Copy todo itemu publikuje zdarzenie utworzenia todo itemu z id nowo powstałej kopii. Dziś
kopia dostaje id dopiero przy dodaniu do kontekstu, już po `Raise(...)`, więc zdarzenie niesie pusty
identyfikator (ADR 0006: id musi być nadane przed `Raise(...)`). Utworzenie todo itemu nadaje id wcześniej
i nie ma tego problemu.

**Blocked by:** None (can start immediately)

**Status:** ready-for-agent

- [ ] Zdarzenie utworzenia po copy niesie id kopii, równe id zwróconemu klientowi.
- [ ] Test unit handlera copy sprawdza id w zdarzeniu i pada na obecnym kodzie (red proof).

## Comments

**Triage (2026-10-14), decyzja użytkownika (bramka T2):** `ready-for-agent`, pierwszy ticket do `/implement`,
na gałęzi `fix/copy-todo-created-event` od `main`. Review według reguły F4 z `AGENTS.md`: `code-review` ticketu
tylko na osi Spec, oś Standards raz na gałąź przez `ca-review`.

Stan kodu sprawdzony przy triage: `CopyTodoCommandHandler` nie nadaje `Id` i wywołuje `Raise(...)` przed
`context.TodoItems.Add(...)`, więc zdarzenie dostaje `Guid.Empty`, a EF Core nadaje id dopiero przy `Add`.
`CreateTodoCommandHandler` nadaje `Id = Guid.NewGuid()` przed `Raise(...)`. `TodoItemCreatedDomainEvent` nie ma
dziś handlera, więc id w zdarzeniu nie jest widoczne przez HTTP; seam dla kryteriów to test unit handlera.
