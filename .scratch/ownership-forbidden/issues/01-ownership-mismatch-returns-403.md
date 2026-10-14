# 01: Cudzy userId w żądaniu daje 403

**What to build:** Zalogowany user, który poda w żądaniu `userId` innego usera (tworzenie todo itemu, lista
todo itemów, copy, profil usera), dostaje 403 z kodem i opisem błędu zamiast 500 „Server failure”, a serwer
nie loguje tego jako awarii. Decyzja: ADR 0005.

**Blocked by:** None (can start immediately)

**Status:** done

- [x] Cudzy `userId` w `POST todos`, `GET todos`, `POST todos/{id}/copy` i `GET users/{userId}` daje 403
      z kodem i opisem błędu w ProblemDetails.
- [x] Pozostałe porażki typu `Failure` nadal dają 500.
- [x] Test integracyjny dla każdej z czterech tras oczekuje 403, a testy unit handlerów oczekują nowego typu
      błędu.
- [x] `AGENTS.md` i skille projektu mówią 403 zamiast 500; ADR 0005 traci akapit o tymczasowym 500.

## Comments

**Bramka B1–B4 (2026-10-14), decyzje użytkownika:**

- B1: praca na gałęzi `fix/ownership-403` od `main`; po zielonym `ca-review` scalenie fast-forward do `main`
  za zgodą użytkownika.
- B2: nowa fabryka `UserErrors.Forbidden()` (kod `Users.Forbidden`) z nowym `ErrorType.Forbidden` → 403
  zastępuje `UserErrors.Unauthorized()`, bo w HTTP „Unauthorized” znaczy 401.
- B3: zakres to pięć wywołań, nie cztery trasy. `GetUserByEmailQueryHandler` (bez endpointu) dostaje tę samą
  zmianę i test unit. Ryzyko enumeracji kont (403 dla cudzego e-maila, 404 dla nieznanego):
  `.scratch/user-by-email-enumeration/issues/01-foreign-email-returns-not-found.md` (`needs-triage`).
- B4: review po tickecie bez zmian (obie osie `code-review`); punkt 7 retro czeka na Fazę 6.

Zakres testów wynikający z ADR 0004 (każdy zmieniony handler ma test unit na każdą ścieżkę porażki i sukces):
dziś testy unit ma tylko `CreateTodoCommandHandler`; brakuje `GetTodosQueryHandlerTests`,
`CopyTodoCommandHandlerTests`, `GetUserByIdQueryHandlerTests` i `GetUserByEmailQueryHandlerTests`.

**`/implement` (2026-10-14):**

Wywołane skille: `tdd` (cykle red → green), `code-review` (obie osie). Skille projektu nie były wywołane,
bo żaden nie pasuje 1:1 do zmiany przekrojowej. Testy pisane według konwencji `add-tests` i
`add-feature/references/tests.md`, red proof według kroku 7 `add-tests`.

Seamy uzgodnione z użytkownikiem przed pierwszym testem:

- S1, HTTP: cztery trasy z `userId` drugiego zarejestrowanego usera dają 403, a ProblemDetails ma `title` =
  `Users.Forbidden` i `detail` = opis błędu.
- S2, unit: pięć handlerów, każda ścieżka porażki i sukces (ADR 0004).
- S3, `CustomResults.Problem` wprost, dla kryterium „pozostałe `Failure` nadal dają 500”: po zmianie żaden handler
  nie zwraca `ErrorType.Failure`, więc żadna trasa HTTP tego nie pokaże. Test leży w projekcie IntegrationTests
  (już referuje Web.Api), bez HTTP i bez Dockera. To świadomy wyjątek od „Tests go through real HTTP” z
  `add-feature/references/tests.md`, zatwierdzony przez użytkownika jako seam S3.

Cykle red → green (każdy red z powodu `InternalServerError` zamiast `Forbidden`, poza cyklem 5):

1. `GET users/{userId}`: nowy `ErrorType.Forbidden`, `Error.Forbidden`, mapowanie w `CustomResults` (status
   403, `title` = kod, `detail` = opis, link RFC 7231 §6.5.3), `UserErrors.Forbidden()`, `GetUserByIdQueryHandler`.
2. `GET todos`: `GetTodosQueryHandler`.
3. `POST todos`: `CreateTodoCommandHandler`; istniejący test unit przemianowany na `..._ReturnForbidden_...`.
4. `POST todos/{id}/copy`: `CopyTodoCommandHandler`. Fixture: user B ma własny todo item, a user A kopiuje go
   z `userId` usera B, więc bez strażnika atak by się udał.
5. `GetUserByEmailQueryHandler` (bez endpointu): red na teście unit (`Users.Unauthorized`/`Failure` zamiast
   `Users.Forbidden`/`Forbidden`). Po nim `UserErrors.Unauthorized()` nie ma wywołań i został usunięty (B2).

Red proof testów, które nie były czerwone w cyklu; jeden mechanizm na przebieg, hash SHA-256 pliku przed
złamaniem równy hashowi po przywróceniu (11/11), bez plików `.bak`:

| # | Złamany mechanizm (warunek osłabiony, nie usunięty) | Test | FAIL |
|---|---|---|---|
| RP1 | strażnik ownera w `GetTodosQueryHandler` | `GetTodosQueryHandlerTests` Forbidden | `IsFailure` False |
| RP2 | filtr `UserId` w `GetTodosQueryHandler` | `..._ReturnOnlyOwnTodos_...` | 2 elementy zamiast 1 |
| RP3 | strażnik ownera w copy | `CopyTodoCommandHandlerTests` Forbidden | `Users.NotFound` zamiast `Users.Forbidden` |
| RP4 | strażnik „user nie istnieje” w copy (plus `user!`) | `..._WhenUserDoesNotExist` | `TodoItems.NotFound` zamiast `Users.NotFound` |
| RP5 | filtr ownera przy odczycie oryginału w copy | `..._WhenTodoBelongsToAnotherUser` | `IsFailure` False |
| RP6 | strażnik ownera w create | `CreateTodoCommandHandlerTests` Forbidden | `Users.NotFound` zamiast `Users.Forbidden` |
| RP7 | strażnik ownera w `GetUserByIdQueryHandler` | `GetUserByIdQueryHandlerTests` Forbidden | `Users.NotFound` zamiast `Users.Forbidden` |
| RP8 | strażnik „user nie istnieje” w `GetUserById` | `..._WhenUserDoesNotExist` | `General.Null` zamiast `Users.NotFound` |
| RP9 | strażnik „e-mail nie istnieje” w `GetUserByEmail` (plus `user!`) | `..._ReturnNotFoundByEmail_...` | `NullReferenceException` |
| RP10 | strażnik ownera w `GetUserByEmail` | `GetUserByEmailQueryHandlerTests` Forbidden | `IsFailure` False |
| RP11 | domyślne mapowanie na 500 w `CustomResults` (→ 403) | `CustomResultsTests` | 403 zamiast 500 |

Pełny przebieg na świeżym buildzie gałęzi (Docker 29.8.1): build 0 błędów, 0 ostrzeżeń; UnitTests 34/34
(22 + 12), ArchitectureTests 6/6, IntegrationTests 13/13 (8 + 4 trasy + `CustomResults`).

`code-review` (diff `git diff HEAD` z `git add -N`, obejście z handoffu): Standards bez twardych naruszeń, Spec
bez blokerów. Ustalenia i decyzje:

- „Serwer nie loguje tego jako awarii”: request log daje teraz 403 na poziomie Information, ale
  `LoggingDecorator` loguje każdą porażkę (także 404/400/409) jako Error. To dług wszystkich typów porażek, nie
  tego ticketu: `.scratch/expected-failure-log-level/issues/01-expected-failures-not-logged-as-errors.md`
  (`needs-triage`).
- Testy unit porównują wynik z `UserErrors.Forbidden()` zgodnie z konwencją „compare exact domain errors”.
  Record `Error` porównuje też `Type`, a typ fabryki przypinają cztery testy HTTP literałem 403. Bez zmian.
- Smelle (judgement calls), bez zmian: równoległe `switch` w `CustomResults` (kształt sprzed tego ticketu),
  powtórzona trójka asercji 403 w czterech testach HTTP, powtórzony fixture `User` w testach unit, opis
  „not authorized” w `Forbidden()` (pasuje do 403; jego zmiana zmieniłaby kontrakt testów).

Poprawki z retro Fazy 5 w tym przebiegu:

- Cykle zaczynały się od testu HTTP: tak, dla czterech tras; cykl 5 od testu unit, bo handler nie ma endpointu.
- Red proof po jednym mechanizmie: tak, 11 przebiegów po jednym mechanizmie (RP4 i RP9 z dodatkowym `!`, żeby
  złamanie się kompilowało, zgodnie z krokiem 7 `add-tests`).
- `ca-review` wymagał testów unit dla każdego zmienionego handlera: tak, punkt „New/changed handlers have unit
  tests covering every `Result.Failure` path plus the happy path” objął wszystkie pięć handlerów i każdy je ma.
  Inaczej niż w pilocie, ADR 0004 i checklista dały ten sam werdykt.

**`ca-review` gałęzi (2026-10-14), scope `git diff 7d1e78b` (commit `54607ff`), `.agents/` wyłączone:** blockers
0, convention violations 0, test gaps 0; każde kryterium ma implementację i test. Uwaga bez wagi blokera:
`CustomResultsTests` w IntegrationTests bez HTTP (wyjątek S3 opisany wyżej). Przebieg na świeżym buildzie
(Docker 29.8.1): build 0 błędów, 0 ostrzeżeń; UnitTests 34/34, ArchitectureTests 6/6, IntegrationTests 13/13.
Werdykt: gotowe. Scalenie do `main` czeka na zgodę użytkownika (B1).
