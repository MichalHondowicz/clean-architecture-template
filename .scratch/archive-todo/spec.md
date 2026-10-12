# Spec: archiwizacja todo itemu

**Status:** ready-for-agent

## Problem Statement

Owner ma na bieżącej liście todo itemy, którymi nie chce się teraz zajmować, ale nie chce ich tracić. Dziś
ma tylko dwa wyjścia. Completion jest nieodwracalne i kłamie, jeśli rzecz nie została zrobiona. Delete
usuwa todo item bez śladu. Bieżąca lista rośnie więc i miesza rzeczy aktualne z odłożonymi.

## Solution

Owner może zarchiwizować swój todo item. Zarchiwizowany todo item znika z bieżącej listy, ale nadal
istnieje: da się go odczytać po id i znaleźć na liście zarchiwizowanych, posortowanej od ostatnio
zarchiwizowanego. Archiwizację cofa unarchive, które przywraca todo item na bieżącą listę. Archive nie
zmienia completion i nie blokuje innych operacji: zarchiwizowany todo item można ukończyć, edytować,
skopiować i usunąć jak każdy inny.

## User Stories

1. Jako owner chcę zarchiwizować todo item, żeby zniknął z bieżącej listy bez usuwania go.
2. Jako owner chcę zarchiwizować nieukończony todo item, żeby odłożyć rzecz, której teraz nie zrobię, bez
   udawania, że jest zrobiona.
3. Jako owner chcę zarchiwizować ukończony todo item, żeby ukończone rzeczy nie zaśmiecały bieżącej listy.
4. Jako owner chcę, żeby archive nie zmieniało completion, żeby po unarchive todo item był w tym samym
   stanie co przed archiwizacją.
5. Jako owner chcę, żeby system zapisał moment archiwizacji, żeby wiedzieć, kiedy odłożyłem todo item.
6. Jako owner chcę, żeby bieżąca lista (`GET todos`) nie zawierała zarchiwizowanych todo itemów, żeby
   widzieć tylko to, czym się zajmuję.
7. Jako owner chcę odczytać zarchiwizowany todo item po id, żeby zobaczyć jego szczegóły razem z momentem
   archiwizacji.
8. Jako owner chcę widzieć listę moich zarchiwizowanych todo itemów, żeby znaleźć ten do przywrócenia bez
   znajomości jego id.
9. Jako owner chcę, żeby lista zarchiwizowanych była posortowana od ostatnio zarchiwizowanego, żeby świeżo
   odłożone rzeczy były na górze.
10. Jako owner chcę, żeby lista zarchiwizowanych zawierała wyłącznie moje todo itemy.
11. Jako owner chcę zrobić unarchive todo itemu, żeby wrócił na bieżącą listę.
12. Jako owner chcę, żeby unarchive kasowało moment archiwizacji, żeby przywrócony todo item nie wyglądał
    na zarchiwizowany.
13. Jako owner chcę dostać czytelny błąd, gdy archiwizuję todo item już zarchiwizowany, żeby wiedzieć, że
    nic się nie zmieniło.
14. Jako owner chcę dostać czytelny błąd, gdy robię unarchive todo itemu, który nie jest zarchiwizowany.
15. Jako owner chcę ukończyć zarchiwizowany todo item bez wcześniejszego unarchive, żeby oznaczyć rzecz
    zrobioną także po jej odłożeniu.
16. Jako owner chcę edytować opis zarchiwizowanego todo itemu.
17. Jako owner chcę usunąć zarchiwizowany todo item, żeby pozbyć się go na stałe.
18. Jako owner chcę skopiować zarchiwizowany todo item i dostać kopię niezarchiwizowaną, żeby od razu była
    na bieżącej liście.
19. Jako owner chcę, żeby nikt inny nie mógł zarchiwizować mojego todo itemu ani zrobić jego unarchive,
    a taka próba wyglądała tak samo jak dla nieistniejącego todo itemu (404).
20. Jako owner chcę, żeby odczyt po id zaraz po archive albo unarchive pokazywał nowy stan, a nie wersję
    sprzed zmiany.
21. Jako owner chcę dostać błąd walidacji, gdy w archive albo unarchive podam pusty id todo itemu.
22. Jako owner chcę, żeby archive, unarchive i lista zarchiwizowanych wymagały zalogowania, żeby nikt bez
    access tokenu nie widział ani nie zmieniał moich todo itemów.

## Implementation Decisions

- **Model.** `TodoItem` dostaje opcjonalny moment archiwizacji `ArchivedAt` (UTC). Null znaczy
  „niezarchiwizowany”. Bez osobnej flagi bool: dwa pola na jeden fakt mogłyby sobie przeczyć. Moment pochodzi
  z `IDateTimeProvider`.
- **Schemat.** Nowa kolumna nullable w tabeli todo itemów, więc potrzebna jest migracja EF Core. Istniejące
  wiersze dostają null, czyli są niezarchiwizowane.
- **Błędy** w `TodoItemErrors`: `TodoItems.AlreadyArchived` i `TodoItems.NotArchived`, oba typu Problem (400).
  Cudzy albo nieistniejący todo item to istniejący `TodoItems.NotFound` (404).
- **Zdarzenia domenowe:** `TodoItemArchivedDomainEvent(TodoItemId)` i `TodoItemUnarchivedDomainEvent(TodoItemId)`.
  Todo item już istnieje, więc ma id przed `Raise(...)` (ADR 0006). Żaden handler ich nie słucha.
- **Use case Archive:** komenda `ArchiveTodoCommand(TodoItemId)`. Wczytuje todo item ownera (filtr po
  `IUserContext.UserId`). Brak todo itemu daje NotFound, a todo item już zarchiwizowany daje AlreadyArchived.
  W przeciwnym razie ustawia `ArchivedAt`, podnosi zdarzenie, zapisuje i unieważnia cache odczytu po id.
- **Use case Unarchive:** komenda `UnarchiveTodoCommand(TodoItemId)`. Brak todo itemu daje NotFound,
  a todo item niezarchiwizowany daje NotArchived. W przeciwnym razie kasuje `ArchivedAt`, podnosi zdarzenie,
  zapisuje i unieważnia cache odczytu po id.
- **Use case GetArchived:** zapytanie `GetArchivedTodosQuery` bez parametrów. Owner pochodzi z
  `IUserContext`, więc nie ma ścieżki „cudzy `userId`” z ADR 0005. Zwraca zarchiwizowane todo itemy ownera
  posortowane malejąco po `ArchivedAt`, bez cache. Ma własny response DTO z tymi samymi polami co lista
  bieżąca plus `ArchivedAt`.
- **Walidatory** obu komend: `TodoItemId` `NotEmpty`.
- **Zmiany w istniejących use case'ach:**
  - `GetTodosQuery` zwraca tylko todo itemy niezarchiwizowane. Kontrakt (parametr `userId`, 500 dla cudzego)
    się nie zmienia.
  - Response `GetTodoByIdQuery` dostaje `ArchivedAt`. Klucz cache się nie zmienia.
  - Complete, Update, Delete i Copy się nie zmieniają. Kopia ma `ArchivedAt` równe null, bo nowy todo item
    nie ustawia tego pola.
- **Kontrakty API** (wszystkie `.WithTags(Tags.Todos)` i `.RequireAuthorization()`, bez tokenu 401):
  - `PUT todos/{id}/archive`: 204; 404 dla cudzego albo nieistniejącego; 400 dla już zarchiwizowanego
    i dla pustego id.
  - `PUT todos/{id}/unarchive`: 204; 404 dla cudzego albo nieistniejącego; 400 dla niezarchiwizowanego
    i dla pustego id.
  - `GET todos/archived`: 200 z listą (może być pusta), bez parametrów. Ograniczenie `guid` na
    `todos/{id:guid}` wyklucza kolizję tras.
  - `GET todos/{id}`: odpowiedź ma `archivedAt` (null dla niezarchiwizowanego).
  - `GET todos`: pomija zarchiwizowane.
- **Cache:** archive i unarchive unieważniają `TodoCacheKeys.ById(userId, todoItemId)` po zapisie. Nie ma
  nowych kluczy cache.

## Testing Decisions

- **Dobry test** sprawdza zachowanie widoczne z zewnątrz przez najwyższy dostępny seam. Nie sprawdza
  szczegółów implementacji. Nie mockuje `DbContext`. NSubstitute stoi tylko na granicy: `IUserContext`,
  `IDateTimeProvider` (ADR 0001, ADR 0004).
- **Seamy** (potwierdzone z użytkownikiem): HTTP z Testcontainers jako główny, handler na `TestDbContext` dla
  kombinatoryki porażek i czasu, walidator przez `TestValidate`. Nowych seamów nie ma.
- **Testy integracyjne** (wzór: `TodosTests`, `RegisterAndLoginAsync`, `Authenticate`):
  - archive zwraca 204, todo item znika z `GET todos` i pojawia się w `GET todos/archived`;
  - `GET todos/{id}` przed archiwizacją (zapisuje wpis w cache), archive, potem `GET todos/{id}` pokazuje
    `archivedAt` różne od null; ten test pilnuje unieważnienia cache;
  - unarchive przywraca todo item do `GET todos`, usuwa go z `GET todos/archived` i kasuje `archivedAt`;
  - archive cudzego todo itemu daje 404 (dwóch zalogowanych userów);
  - każda nowa trasa bez tokenu daje 401.
- **Testy unit handlerów** (wzór: `CompleteTodoCommandHandlerTests`):
  - Archive: NotFound dla nieistniejącego, NotFound dla todo itemu innego ownera, AlreadyArchived, sukces
    ustawia `ArchivedAt` z `IDateTimeProvider` i podnosi `TodoItemArchivedDomainEvent`;
  - Unarchive: NotFound, NotArchived, sukces kasuje `ArchivedAt` i podnosi `TodoItemUnarchivedDomainEvent`;
  - GetArchived: zwraca tylko zarchiwizowane todo itemy ownera, posortowane malejąco po `ArchivedAt`
    (dane z różnymi momentami archiwizacji, więc kolejność jest deterministyczna).
- **Testy walidatorów** (wzór: `TodoValidatorsTests`): pusty `TodoItemId` nie przechodzi, niepusty przechodzi,
  osobno dla obu komend.
- **Red proof** według `add-feature`: zepsuć mechanizm, dla którego istnieje dany use case (unieważnienie
  cache, filtr `UserId`, filtr zarchiwizowanych w `GET todos`, sprawdzenie AlreadyArchived), zobaczyć
  czerwony test z właściwym komunikatem, przywrócić.
- Cudzy todo item w trasach po własnym id to zawsze 404. Żaden test nie oczekuje 401, 403 ani 500 dla
  niezgodności ownera.

## Out of Scope

- 403 dla cudzego `userId` (ADR 0005, `.scratch/ownership-forbidden/`) i każda zmiana kontraktu `GET todos`
  poza filtrem zarchiwizowanych.
- Zamrożenie zarchiwizowanego todo itemu (blokada Complete/Update): odrzucone w grillu.
- Historia cykli archive/unarchive.
- Stronicowanie i filtrowanie listy zarchiwizowanych.
- Archiwizacja wielu todo itemów naraz i automatyczna archiwizacja (np. ukończonych po N dniach).
- Handlery zdarzeń `TodoItemArchivedDomainEvent` i `TodoItemUnarchivedDomainEvent`.
- Istniejący dług: zdarzenie copy z pustym id (`.scratch/copy-todo-created-event/`), brak bezwzględnego limitu
  sesji (`.scratch/refresh-token-lifetime/`), `Priority` zapisywany, ale niezwracany w odpowiedziach.

## Further Notes

- Słownik: hasła Archive i Unarchive powstały w grillu, a hasło Copy zostało doprecyzowane („nieukończony
  i niezarchiwizowany”).
- ADR-u nie ma. Niezależne osie archive/completion i `GET todos/archived` bez `userId` to decyzje łatwe do
  odwrócenia, bo template nie ma zewnętrznych klientów.
- Spec powstał w pilocie Fazy 5 (`docs/pilot-2026-10-archive-todo.md`). Kod zostaje na gałęzi
  `pilot/archive-todo` i nie trafia do `main`.
