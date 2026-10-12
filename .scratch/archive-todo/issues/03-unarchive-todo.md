# 03: Unarchive todo itemu

**What to build:** Owner robi unarchive zarchiwizowanego todo itemu: todo item wraca na bieżącą listę, znika
z listy zarchiwizowanych, a moment archiwizacji przepada, także w odczycie po id, który był w cache.
Completion się nie zmienia. Spec: `.scratch/archive-todo/spec.md`.

**Blocked by:** 01 (Archive todo itemu), 02 (Lista zarchiwizowanych todo itemów)

**Status:** ready-for-agent

- [ ] `PUT todos/{id}/unarchive` na zarchiwizowanym todo itemie daje 204; todo item wraca do `GET todos`,
      znika z `GET todos/archived`, a `GET todos/{id}` zwraca `archivedAt` równe null.
- [ ] Odczyt `GET todos/{id}` wykonany przed unarchive (wpis w cache) po unarchive pokazuje `archivedAt`
      równe null: unarchive unieważnia cache odczytu po id.
- [ ] Unarchive nie zmienia completion todo itemu.
- [ ] Unarchive niezarchiwizowanego todo itemu daje 400 z kodem `TodoItems.NotArchived`.
- [ ] Cudzy albo nieistniejący todo item daje 404 `TodoItems.NotFound`; pusty id daje 400 walidacji;
      bez access tokenu 401.
- [ ] Unarchive podnosi `TodoItemUnarchivedDomainEvent` z id todo itemu.
- [ ] Testy: unit handlera (NotFound dla nieistniejącego i cudzego, NotArchived, sukces kasuje moment
      i podnosi zdarzenie), walidatora, integracyjne przez HTTP (powyższe zachowania, w tym cache, 404 dla
      cudzego, 401); red proof dla sprawdzenia NotArchived albo unieważnienia cache.
- [ ] Build bez ostrzeżeń i bez suppressions; pełny suite zielony z Dockerem.
