# 01: Cudzy userId w żądaniu daje 403

**What to build:** Zalogowany user, który poda w żądaniu `userId` innego usera (tworzenie todo itemu, lista
todo itemów, copy, profil usera), dostaje 403 z kodem i opisem błędu zamiast 500 „Server failure”, a serwer
nie loguje tego jako awarii. Decyzja: ADR 0005.

**Blocked by:** None (can start immediately)

**Status:** ready-for-agent

- [ ] Cudzy `userId` w `POST todos`, `GET todos`, `POST todos/{id}/copy` i `GET users/{userId}` daje 403
      z kodem i opisem błędu w ProblemDetails.
- [ ] Pozostałe porażki typu `Failure` nadal dają 500.
- [ ] Test integracyjny dla każdej z czterech tras oczekuje 403, a testy unit handlerów oczekują nowego typu
      błędu.
- [ ] `AGENTS.md` i skille projektu mówią 403 zamiast 500; ADR 0005 traci akapit o tymczasowym 500.
