# 01: Wyszukiwanie usera po cudzym e-mailu nie zdradza, że konto istnieje

**What to build:** `GetUserByEmailQueryHandler` dla e-maila innego usera zwraca ten sam błąd co dla e-maila,
którego nie ma w bazie (`UserErrors.NotFoundByEmail`), więc odpowiedź nie zdradza, czy konto istnieje. Po
tickecie `.scratch/ownership-forbidden/issues/01-ownership-mismatch-returns-403.md` cudzy e-mail daje
`UserErrors.Forbidden()` (403), a nieznany 404, czyli różnica statusów pozwala enumerować konta. Handler nie
ma dziś endpointu, więc problem jest uśpiony do czasu jego wystawienia. Do rozstrzygnięcia przy triage: czy
to odstępstwo od ADR 0005 (403 dla niezgodności ownera) wymaga nowego ADR-a.

**Blocked by:** `.scratch/ownership-forbidden/issues/01-ownership-mismatch-returns-403.md`

**Status:** needs-triage

- [ ] Cudzy e-mail i nieistniejący e-mail dają ten sam błąd `UserErrors.NotFoundByEmail`.
- [ ] Test unit handlera dla cudzego e-maila oczekuje `NotFoundByEmail` i pada na obecnym kodzie (red proof).
