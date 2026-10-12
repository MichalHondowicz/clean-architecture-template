# 03: Unarchive todo itemu

**What to build:** Owner robi unarchive zarchiwizowanego todo itemu: todo item wraca na bieżącą listę, znika
z listy zarchiwizowanych, a moment archiwizacji przepada, także w odczycie po id, który był w cache.
Completion się nie zmienia. Spec: `.scratch/archive-todo/spec.md`.

**Blocked by:** 01 (Archive todo itemu), 02 (Lista zarchiwizowanych todo itemów)

**Status:** ready-for-agent

- [x] `PUT todos/{id}/unarchive` na zarchiwizowanym todo itemie daje 204; todo item wraca do `GET todos`,
      znika z `GET todos/archived`, a `GET todos/{id}` zwraca `archivedAt` równe null.
- [x] Odczyt `GET todos/{id}` wykonany przed unarchive (wpis w cache) po unarchive pokazuje `archivedAt`
      równe null: unarchive unieważnia cache odczytu po id.
- [x] Unarchive nie zmienia completion todo itemu.
- [x] Unarchive niezarchiwizowanego todo itemu daje 400 z kodem `TodoItems.NotArchived`.
- [x] Cudzy albo nieistniejący todo item daje 404 `TodoItems.NotFound`; pusty id daje 400 walidacji;
      bez access tokenu 401.
- [x] Unarchive podnosi `TodoItemUnarchivedDomainEvent` z id todo itemu.
- [x] Testy: unit handlera (NotFound dla nieistniejącego i cudzego, NotArchived, sukces kasuje moment
      i podnosi zdarzenie), walidatora, integracyjne przez HTTP (powyższe zachowania, w tym cache, 404 dla
      cudzego, 401); red proof dla sprawdzenia NotArchived albo unieważnienia cache.
- [x] Build bez ostrzeżeń i bez suppressions; pełny suite zielony z Dockerem.

## Comments

2026-10-12, `/implement`: zrobione na gałęzi `pilot/archive-todo`. Build 0/0 bez suppressions; UnitTests 38/38,
ArchitectureTests 6/6, IntegrationTests 26/26 (Docker). Naturalne red: brak trasy, nieaktualny cache, cudzy
todo item przywrócony, brak NotArchived, brak zdarzenia, brak reguły walidatora. Red proof (jeden łączony
przebieg, hashe zgodne): `.RequireAuthorization()`, filtr `UserId`, warunek NotArchived, reguła walidatora.
