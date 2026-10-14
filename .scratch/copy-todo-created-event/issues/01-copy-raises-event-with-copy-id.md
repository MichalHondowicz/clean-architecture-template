# 01: Copy publikuje zdarzenie utworzenia z id kopii

**What to build:** Copy todo itemu publikuje zdarzenie utworzenia todo itemu z id nowo powstałej kopii. Dziś
kopia dostaje id dopiero przy dodaniu do kontekstu, już po `Raise(...)`, więc zdarzenie niesie pusty
identyfikator (ADR 0006: id musi być nadane przed `Raise(...)`). Utworzenie todo itemu nadaje id wcześniej
i nie ma tego problemu.

**Blocked by:** None (can start immediately)

**Status:** done

- [x] Zdarzenie utworzenia po copy niesie id kopii, równe id zwróconemu klientowi.
- [x] Test unit handlera copy sprawdza id w zdarzeniu i pada na obecnym kodzie (red proof).

## Comments

**Triage (2026-10-14), decyzja użytkownika (bramka T2):** `ready-for-agent`, pierwszy ticket do `/implement`,
na gałęzi `fix/copy-todo-created-event` od `main`. Review według reguły F4 z `AGENTS.md`: `code-review` ticketu
tylko na osi Spec, oś Standards raz na gałąź przez `ca-review`.

Stan kodu sprawdzony przy triage: `CopyTodoCommandHandler` nie nadaje `Id` i wywołuje `Raise(...)` przed
`context.TodoItems.Add(...)`, więc zdarzenie dostaje `Guid.Empty`, a EF Core nadaje id dopiero przy `Add`.
`CreateTodoCommandHandler` nadaje `Id = Guid.NewGuid()` przed `Raise(...)`. `TodoItemCreatedDomainEvent` nie ma
dziś handlera, więc id w zdarzeniu nie jest widoczne przez HTTP; seam dla kryteriów to test unit handlera.

**`/implement` (2026-10-14):**

Wywołane skille: `tdd` (jeden cykl red → green), `code-review` (tylko oś Spec, reguła F4), potem `ca-review`
gałęzi. Pozostałe handlery wywołujące `Raise(...)` nie mają tego błędu: Create i Register nadają id jawnie,
a Complete, Update i Delete działają na wczytanej encji.

Seam uzgodniony z użytkownikiem przed testem: S1, test unit `CopyTodoCommandHandler.Handle`; zdarzenie
`TodoItemCreatedDomainEvent` niesie `TodoItemId` równe `result.Value`. Decyzja użytkownika: zaostrzyć istniejący
happy path `Handle_Should_PersistUncompletedCopyAndRaiseDomainEvent_WhenValid` zamiast dodawać test, bo
powtórzyłby cały Arrange. Testu HTTP nie ma, bo zdarzenie nie ma handlera.

Cykl 1: red na niezmienionym handlerze (`createdEvent.TodoItemId should be <id kopii> but was
00000000-0000-0000-0000-000000000000`), green po `Id = Guid.NewGuid()` w inicjalizatorze kopii, jak w Create.
Ten red to red proof kryterium 2: test widział kod bez naprawy, więc osobne łamanie nic by nie dodało.
Pułapka po drodze: `ShouldContain` z Shouldly bierze drzewo wyrażeń, a w nim `is` z wzorcem to CS8122;
asercja idzie przez `OfType<TodoItemCreatedDomainEvent>().ShouldHaveSingleItem()`.

Pełny przebieg na świeżym buildzie gałęzi (Docker 29.8.1): build 0 błędów, 0 ostrzeżeń; UnitTests 34/34,
ArchitectureTests 6/6, IntegrationTests 13/13 (liczby bez zmian, bo test został zaostrzony, a nie dodany).

`code-review`, oś Spec (diff `git diff HEAD` przed commitem): 0 ustaleń; oba kryteria spełnione, bez zmian
spoza zakresu.

**`ca-review` gałęzi (2026-10-14), scope `git diff 23d77b3` (commit `1171214`), `.agents/` wyłączone:** blockers
0, convention violations 0, test gaps 0; każde kryterium ma implementację i test. Werdykt: gotowe. Scalenie do
`main` czeka na zgodę użytkownika.

Reguła F4 (pierwszy przebieg): zadziałała bez tarcia. Oś Spec potwierdziła oba kryteria i brak zmian spoza
zakresu, a `ca-review` nie znalazł niczego, co oś Spec przepuściła. Zmiana to jedna linia kodu i jedna asercja,
więc ten przebieg nie pokazuje, czy odłożenie osi Standards na koniec gałęzi nie gubi czegoś przy większych
ticketach. Do sprawdzenia przy refresh tokenie (encja, migracja, kilka plików).

Obserwacja procesu: pierwszy przebieg testu po zmianie poszedł w jednej komendzie `dotnet build; dotnet test
--no-build`. Build padł (CS8122), a test na starych binariach dał 4/4 Passed. Agent zauważył `1 Error(s)` przed
oceną wyniku. Wniosek: przy `--no-build` czytać wynik buildu przed wynikiem testów.
