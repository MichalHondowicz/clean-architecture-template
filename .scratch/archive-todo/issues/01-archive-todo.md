# 01: Archive todo itemu

**What to build:** Owner archiwizuje swój todo item. Todo item znika z bieżącej listy (`GET todos`), ale
nadal istnieje: `GET todos/{id}` zwraca go z momentem archiwizacji, także gdy odczyt był wcześniej
w cache. Archive nie zmienia completion; zarchiwizować można todo item ukończony i nieukończony. Spec:
`.scratch/archive-todo/spec.md`.

**Blocked by:** None (can start immediately)

**Status:** ready-for-agent

- [ ] `PUT todos/{id}/archive` na własnym todo itemie daje 204; todo item znika z `GET todos`, a
      `GET todos/{id}` zwraca go z `archivedAt` różnym od null.
- [ ] `GET todos/{id}` dla niezarchiwizowanego todo itemu zwraca `archivedAt` równe null.
- [ ] Odczyt `GET todos/{id}` wykonany przed archiwizacją (wpis w cache) po archiwizacji pokazuje
      `archivedAt` różne od null: archive unieważnia cache odczytu po id.
- [ ] Archive todo itemu ukończonego i nieukończonego się udaje i nie zmienia jego completion.
- [ ] Ponowna archiwizacja zarchiwizowanego daje 400 z kodem `TodoItems.AlreadyArchived`.
- [ ] Cudzy albo nieistniejący todo item daje 404 `TodoItems.NotFound`; pusty id daje 400 walidacji;
      bez access tokenu 401.
- [ ] Moment archiwizacji pochodzi z `IDateTimeProvider`; archive podnosi `TodoItemArchivedDomainEvent`
      z id todo itemu.
- [ ] Migracja EF Core dodaje opcjonalny moment archiwizacji; istniejące todo itemy są niezarchiwizowane.
- [ ] Testy: unit handlera (NotFound dla nieistniejącego i cudzego, AlreadyArchived, sukces z momentem
      i zdarzeniem), walidatora, integracyjne przez HTTP (powyższe zachowania, w tym cache, 404 dla cudzego,
      401); red proof dla unieważnienia cache albo filtra zarchiwizowanych w `GET todos`.
- [ ] Build bez ostrzeżeń i bez suppressions; pełny suite zielony z Dockerem.
