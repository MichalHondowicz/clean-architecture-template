# 01: Wyszukiwanie usera po cudzym e-mailu nie zdradza, że konto istnieje

**What to build:** `GetUserByEmailQueryHandler` dla e-maila innego usera zwraca ten sam błąd co dla e-maila,
którego nie ma w bazie (`UserErrors.NotFoundByEmail`), więc odpowiedź nie zdradza, czy konto istnieje. Po
tickecie `.scratch/ownership-forbidden/issues/01-ownership-mismatch-returns-403.md` cudzy e-mail daje
`UserErrors.Forbidden()` (403), a nieznany 404, czyli różnica statusów pozwala enumerować konta. Handler nie
ma dziś endpointu, więc problem jest uśpiony do czasu jego wystawienia. Do rozstrzygnięcia przy triage: czy
to odstępstwo od ADR 0005 (403 dla niezgodności ownera) wymaga nowego ADR-a.

**Blocked by:** `.scratch/ownership-forbidden/issues/01-ownership-mismatch-returns-403.md`

**Status:** done

- [x] Cudzy e-mail i nieistniejący e-mail dają ten sam błąd `UserErrors.NotFoundByEmail`.
- [x] Test unit handlera dla cudzego e-maila oczekuje `NotFoundByEmail` i pada na obecnym kodzie (red proof).

## Comments

**Triage (2026-10-14), decyzja użytkownika (bramka T2):** `ready-for-agent`, bez nowego ADR-a. ADR 0005 dostał
przy triage akapit o wyjątku: wyszukiwanie usera po e-mailu daje dla cudzego e-maila 404 zamiast 403. Ticket
blokujący (`ownership-forbidden/01`) ma `Status: done`, więc ten jest odblokowany.

Stan kodu sprawdzony przy triage: `GetUserByEmailQueryHandler` zwraca `UserErrors.Forbidden()` dla cudzego
e-maila i `UserErrors.NotFoundByEmail` dla nieznanego. Test
`GetUserByEmailQueryHandlerTests.Handle_Should_ReturnForbidden_WhenEmailBelongsToAnotherUser` przypina dziś
403, więc w tym tickecie zmienia oczekiwanie. Handler nie ma endpointu, więc seam to test unit.

**`/implement` (2026-10-14):**

Wywołane skille: `tdd` (jeden cykl red → green), `code-review` (tylko oś Spec, reguła F4), potem `ca-review`
gałęzi.

Seam uzgodniony z użytkownikiem przed testem: S1, test unit `GetUserByEmailQueryHandler.Handle`. Decyzja
użytkownika: odwrócić istniejący test na
`Handle_Should_ReturnNotFoundByEmail_WhenEmailBelongsToAnotherUser` (Arrange bez zmian), bo stary przypinał
zachowanie, które ticket usuwa. Testy nieznanego i własnego e-maila zostają jako regresja.

Cykl 1: red na niezmienionym handlerze (`Users.Forbidden` zamiast `Users.NotFoundByEmail`), green po zmianie
błędu w gałęzi cudzego e-maila na `UserErrors.NotFoundByEmail`. ADR 0005 stracił zdanie przejściowe „do czasu
wdrożenia handler zwraca 403”.

Red proof strażnika: warunek osłabiony (`&& user.Id == Guid.Empty`), odwrócony test padł na `IsFailure` False;
hash SHA-256 pliku przed złamaniem równy hashowi po przywróceniu, bez plików `.bak`.

Pełny przebieg na świeżym buildzie gałęzi (Docker 29.8.1): build 0 błędów, 0 ostrzeżeń; UnitTests 36/36,
ArchitectureTests 6/6, IntegrationTests 14/14 (liczby bez zmian, bo test został odwrócony, a nie dodany).

`code-review`, oś Spec (diff `git diff HEAD` przed commitem): 0 ustaleń. Obie gałęzie zwracają tę samą
instancję `UserErrors.NotFoundByEmail`, więc kod, opis i typ błędu są identyczne; ADR 0005 zgadza się z kodem.
Obserwacja bez wagi defektu: wyjątek „e-mail → 404” jest zapisany tylko w ADR 0005, a `AGENTS.md` (Ownership)
go nie wymienia.

**`ca-review` gałęzi (2026-10-14), scope `git diff 7c62722` (commit `78d8026`), `.agents/` wyłączone:** blockers
0, convention violations 0, test gaps 0; każde kryterium ma implementację i test. Uwaga bez wagi blokera:
dwie gałęzie zwracają ten sam błąd i dałoby się je złączyć filtrem po `IUserContext.UserId` w zapytaniu
(styl, nie zachowanie). Przebieg na świeżym buildzie: build 0/0; UnitTests 36/36, ArchitectureTests 6/6,
IntegrationTests 14/14. Werdykt: gotowe. Scalenie do `main` czeka na zgodę użytkownika.

Reguła F4 (trzeci przebieg): oś Spec i `ca-review` dały te same wnioski bez blokerów; przy zmianie jednej
linii podział osi nie miał czego rozdzielić.

Obserwacja procesu: Docker Desktop stanął między sesjami i wstał po 5 s od `Start-Process`; sprawdzenie
`docker info` przed pełnym suite złapało to przed fałszywym wynikiem testów integracyjnych.
