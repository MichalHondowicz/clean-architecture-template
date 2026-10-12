---
status: accepted
---

# Cudzy userId w żądaniu daje 403, nie 500

Gdy klient podaje w żądaniu `userId` inny niż zalogowany user (`POST todos`, `GET todos`,
`POST todos/{id}/copy`, `GET users/{userId}`), handler zwraca `UserErrors.Unauthorized()`. To
`ErrorType.Failure`, więc odpowiedź to 500 „Server failure”, logowane jako awaria serwera, choć zawinił
klient. Decyzja: dodać `ErrorType.Forbidden` mapowany na 403 i użyć go dla tego błędu. Todo item czytany po
własnym id dalej filtrujemy po `UserId`, więc cudzy todo item daje 404 i nie ujawnia, że istnieje.

Wdrożenie: `.scratch/ownership-forbidden/`. Do czasu jego zamknięcia obowiązuje fakt z `AGENTS.md`: ta
porażka odpowiada 500 i żaden test ani kryterium akceptacji nie oczekuje jeszcze 403.

## Considered options

- Utrwalić 500: odrzucone, bo błąd klienta zaśmieca logi i monitoring jako awaria serwera.
- 404 także dla cudzego `userId`: odrzucone, bo klient sam podaje ten id, więc nie ma czego ukrywać, a 403
  nazywa błąd wprost.
