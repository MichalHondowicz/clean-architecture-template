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

Ten sam błąd zwraca `GetUserByEmailQueryHandler` (bez endpointu), gdy e-mail należy do innego usera. Ryzyko
enumeracji kont w tym miejscu: `.scratch/user-by-email-enumeration/`. Wdrożenie: `.scratch/ownership-forbidden/`.

## Considered options

- Utrwalić 500: odrzucone, bo błąd klienta zaśmieca logi i monitoring jako awaria serwera.
- 404 także dla cudzego `userId`: odrzucone, bo klient sam podaje ten id, więc nie ma czego ukrywać, a 403
  nazywa błąd wprost.
