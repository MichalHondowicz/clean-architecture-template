# 02: Lista zarchiwizowanych todo itemów

**What to build:** Owner widzi listę swoich zarchiwizowanych todo itemów, od ostatnio zarchiwizowanego, i może
w niej znaleźć todo item do przywrócenia bez znajomości jego id. Owner pochodzi z access tokenu, więc trasa
nie przyjmuje `userId`. Spec: `.scratch/archive-todo/spec.md`.

**Blocked by:** 01 (Archive todo itemu)

**Status:** ready-for-agent

- [x] `GET todos/archived` zwraca wyłącznie zarchiwizowane todo itemy zalogowanego ownera, każdy
      z `archivedAt`; niezarchiwizowane i cudze todo itemy się w niej nie pojawiają.
- [x] Lista jest posortowana malejąco po momencie archiwizacji.
- [x] Owner bez zarchiwizowanych todo itemów dostaje 200 z pustą listą.
- [x] Trasa nie ma parametrów; bez access tokenu 401. Wynik nie jest cache'owany.
- [x] Testy: unit handlera (tylko zarchiwizowane todo itemy ownera, kolejność na danych z różnymi
      momentami archiwizacji), integracyjne przez HTTP (zarchiwizowany jest na liście, niezarchiwizowany
      i cudzy nie, pusta lista, 401); red proof dla filtra ownera albo filtra zarchiwizowanych.
- [x] Build bez ostrzeżeń i bez suppressions; pełny suite zielony z Dockerem.

## Comments

2026-10-12, `/implement`: zrobione na gałęzi `pilot/archive-todo`. Build 0/0 bez suppressions; UnitTests 31/31,
ArchitectureTests 6/6, IntegrationTests 20/20 (Docker). Red proof: `.RequireAuthorization()` (401 → 500),
filtr ownera (handler zwraca 2 zamiast 1; HTTP pokazuje cudzy todo item). Kolejność sprawdzona tylko testem
unit na InMemory, zgodnie z uzgodnionymi seamami.
