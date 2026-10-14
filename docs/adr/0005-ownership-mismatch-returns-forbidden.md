---
status: accepted
---

# Cudzy userId w żądaniu daje 403, nie 500

Gdy klient podawał w żądaniu `userId` inny niż zalogowany user (`POST todos`, `GET todos`,
`POST todos/{id}/copy`, `GET users/{userId}`), handler zwracał `UserErrors.Unauthorized()`. To był
`ErrorType.Failure`, więc odpowiedź to 500 „Server failure”, logowane jako awaria serwera, choć zawinił
klient. Decyzja: dodać `ErrorType.Forbidden` mapowany na 403 i zastąpić ten błąd fabryką
`UserErrors.Forbidden()` (kod `Users.Forbidden`), bo w HTTP „Unauthorized” znaczy 401. Todo item czytany po
własnym id dalej filtrujemy po `UserId`, więc cudzy todo item daje 404 i nie ujawnia, że istnieje.

Wyjątek: wyszukiwanie usera po e-mailu (`GetUserByEmailQueryHandler`, bez endpointu). E-mail da się zgadnąć,
a id (GUID) nie, więc 403 dla cudzego e-maila obok 404 dla nieznanego zdradzałoby, które konta istnieją. Cudzy
e-mail daje ten sam błąd co nieznany (`UserErrors.NotFoundByEmail`, 404), jak cudzy todo item czytany po
własnym id. Decyzja z triage 2026-10-14; do czasu wdrożenia (`.scratch/user-by-email-enumeration/`) handler
zwraca 403. Wdrożenie reszty: `.scratch/ownership-forbidden/`.

## Considered options

- Utrwalić 500: odrzucone, bo błąd klienta zaśmieca logi i monitoring jako awaria serwera.
- 404 także dla cudzego `userId`: odrzucone, bo klient sam podaje ten id, więc nie ma czego ukrywać, a 403
  nazywa błąd wprost.
