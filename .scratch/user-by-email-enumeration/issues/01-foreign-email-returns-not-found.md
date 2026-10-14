# 01: Wyszukiwanie usera po cudzym e-mailu nie zdradza, że konto istnieje

**What to build:** `GetUserByEmailQueryHandler` dla e-maila innego usera zwraca ten sam błąd co dla e-maila,
którego nie ma w bazie (`UserErrors.NotFoundByEmail`), więc odpowiedź nie zdradza, czy konto istnieje. Po
tickecie `.scratch/ownership-forbidden/issues/01-ownership-mismatch-returns-403.md` cudzy e-mail daje
`UserErrors.Forbidden()` (403), a nieznany 404, czyli różnica statusów pozwala enumerować konta. Handler nie
ma dziś endpointu, więc problem jest uśpiony do czasu jego wystawienia. Do rozstrzygnięcia przy triage: czy
to odstępstwo od ADR 0005 (403 dla niezgodności ownera) wymaga nowego ADR-a.

**Blocked by:** `.scratch/ownership-forbidden/issues/01-ownership-mismatch-returns-403.md`

**Status:** ready-for-agent

- [ ] Cudzy e-mail i nieistniejący e-mail dają ten sam błąd `UserErrors.NotFoundByEmail`.
- [ ] Test unit handlera dla cudzego e-maila oczekuje `NotFoundByEmail` i pada na obecnym kodzie (red proof).

## Comments

**Triage (2026-10-14), decyzja użytkownika (bramka T2):** `ready-for-agent`, bez nowego ADR-a. ADR 0005 dostał
przy triage akapit o wyjątku: wyszukiwanie usera po e-mailu daje dla cudzego e-maila 404 zamiast 403. Ticket
blokujący (`ownership-forbidden/01`) ma `Status: done`, więc ten jest odblokowany.

Stan kodu sprawdzony przy triage: `GetUserByEmailQueryHandler` zwraca `UserErrors.Forbidden()` dla cudzego
e-maila i `UserErrors.NotFoundByEmail` dla nieznanego. Test
`GetUserByEmailQueryHandlerTests.Handle_Should_ReturnForbidden_WhenEmailBelongsToAnotherUser` przypina dziś
403, więc w tym tickecie zmienia oczekiwanie. Handler nie ma endpointu, więc seam to test unit.
