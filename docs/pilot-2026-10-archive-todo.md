# Dziennik pilota: archiwizacja todo itemu (Faza 5)

Start: 2026-10-12. Gałąź `pilot/archive-todo` od `main` (`111f2d4`). Plan: `docs/plan-2026-10-12-agent-setup.md`,
Faza 5. Handoff: `docs/handoff-2026-10-12-phase5-pilot.md`. Spec i tickety: `.scratch/archive-todo/`.

Agent prowadzący pilot jest jednocześnie autorem tego dziennika. Każda korekta, którą musiał zrobić
użytkownik, trafia do sekcji „Korekty użytkownika”, także jeśli wydaje się drobna.

## Kryteria sukcesu (plan, sekcja „Cel i efekt końcowy”)

| Kryterium | Wynik |
|---|---|
| Build: 0 błędów, 0 ostrzeżeń, bez suppressions | Spełnione (szczegóły: krok 6) |
| Testy zielone: unit, architecture, integration (z Dockerem) | Spełnione: 40/40, 6/6, 30/30 |
| `ca-review` bez uwag | Spełnione po poprawkach z review gałęzi (krok 5) |
| `GLOSSARY.md` zaktualizowany (co najmniej „Archive”) | Spełnione: Archive, Unarchive, Copy |
| Użytkownik nie poprawiał agenta w kwestii konwencji | Spełnione: zero korekt |

## Preflight (2026-10-12, własny przebieg)

- `git status`: czyste drzewo na `main`, HEAD `111f2d4`.
- `docker info`: serwer 29.8.1, Docker Desktop.
- Fork Pococka: `installed_plugins.json` ma `mattpocock-skills@mattpocock` w wersji `1.3.1-dotnet.1`
  (`gitCommitSha` `2f5489b`); `skills/engineering/tdd/tests.md` w cache tej wersji zawiera przykłady `csharp`.
- superpowers: `"superpowers@claude-plugins-official": false` w `.claude/settings.json`; na starcie sesji nie
  pojawił się wstrzyknięty „using-superpowers”, a lista skilli sesji nie zawiera skilli superpowers.
- `dotnet build CleanArchitecture.slnx`: 0 błędów, 0 ostrzeżeń.
- `dotnet test CleanArchitecture.slnx`: UnitTests 22/22, ArchitectureTests 6/6, IntegrationTests 8/8.

## Decyzje przed pilotem

- **P1 (a):** gałąź pilota zostaje jako dowód; do `main` trafia tylko dziennik i poprawki warstwy agentowej
  (`AGENTS.md`, skille, ADR-y). Template zostaje generyczny.
- **P2 (a):** najpierw pilot; ticket 403 (`.scratch/ownership-forbidden/`) później jako osobny przebieg
  samego `/implement`.

## Zastrzeżenie metodyczne

Przed grillem agent przeczytał handoff, w tym listę kontrolną pytań dla grilla z rekomendacjami (sekcja 4
handoffu), a także `GLOSSARY.md`, ADR-y 0004–0006 i `add-feature`. Pomiar „czy `/grill-with-docs` sam
wydobędzie pytania” jest więc zanieczyszczony: agent zna listę. Odhaczenie pytania mówi, że padło w grillu,
ale nie rozstrzyga, czy padłoby bez handoffu.

## Krok 2: `/grill-with-docs archiwizacja todo itemu`

Przebieg: skill wywołał `grilling` i `domain-modeling`. 11 pytań w 3 rundach, wszystkie rekomendacje przyjęte
(runda 1 odpowiedzią „kontynuuj”, zinterpretowaną jako akceptacja; rundy 2 i 3 jawnym „akceptuję wszystkie
rekomendacje”). Fakty z kodu agent zebrał sam (`find`, `cat` na `Domain/Todos`, `Application/Todos`,
`Endpoints/Todos`), a nie przez sub-agenta, jak każe `grilling`: odstępstwo od skilla.

Lista kontrolna z handoffu:

- [x] Kto może archiwizować: Q2 (runda 1), tylko owner, cudzy todo item to 404.
- [x] Czy da się przywrócić: Q3 (runda 1) tak, osobny use case; nazwa Unarchive w Q4.
- [x] Widoczność: Q5 (runda 2), `GET todos` pomija zarchiwizowane, `GET todos/{id}` je zwraca.
- [x] Relacja z Completion: Q1 (archiwizować można nieukończony) i Q6 (zarchiwizowany można ukończyć,
  edytować, skopiować, usunąć; niezależne osie).
- [x] Cache: Q11 (runda 3), ale jako założenie z konwencji do potwierdzenia, nie jako otwarta decyzja.
- [x] Zdarzenie domenowe: Q11, `TodoItemArchivedDomainEvent` i `TodoItemUnarchivedDomainEvent`; również jako
  założenie z konwencji.

Pytania spoza listy:

- Q4: nazwa cofnięcia (Unarchive, nie Restore/Reopen) i trasy `PUT todos/{id}/archive`, `.../unarchive` (204).
- Q6 (część): Copy zarchiwizowanego daje kopię niezarchiwizowaną.
- Q7: ponowna archiwizacja i Unarchive niezarchiwizowanego to błąd 400 (`TodoItems.AlreadyArchived`,
  `TodoItems.NotArchived`), spójnie z Completion.
- Q8: moment archiwizacji jako jedno pole `ArchivedAt` (null = niezarchiwizowany), kasowane przez Unarchive.
- Q9: jak owner znajduje zarchiwizowane, skoro Unarchive wymaga id. Wyszło ze scenariusza; dodało use case
  `GET todos/archived` (owner z `IUserContext`, bez `userId`, sortowanie od ostatnio zarchiwizowanego, bez
  cache).
- Q10: kolizja nazwy „Active” (w aplikacjach todo zwykle „nieukończony”); bez nowego terminu.

Zakres urósł w grillu: z jednego use case'u do trzech nowych (Archive, Unarchive, GetArchived) i zmiany dwóch
istniejących zapytań (`GET todos` filtruje, `GET todos/{id}` zwraca `ArchivedAt`).

Zmiany w `GLOSSARY.md`: nowe hasła **Archive** i **Unarchive**, w **Copy** dopisane „niezarchiwizowany”.

ADR-y: żaden. Kandydaci odrzuceni wg trzech warunków `domain-modeling`: niezależne osie archive/completion
i `GET todos/archived` bez `userId` są łatwe do odwrócenia (template bez zewnętrznych klientów), więc nie
spełniają warunku „trudne do odwrócenia”.

Bramka: użytkownik potwierdził wspólne rozumienie i zmiany w słowniku.

## Krok 3: `/to-spec`, `/to-tickets`

Tarcie: pierwsza próba to wiadomość „potwierdzam, /to-spec”. Komenda w środku zdania nie uruchamia skilla,
a agent nie może jej wywołać sam (`disable-model-invocation: true`). Agent poprosił o osobną wiadomość.

`/to-spec`: agent przedstawił seamy (HTTP główny, handler dla porażek i czasu, walidator; bez nowych),
użytkownik je przyjął. Spec: `.scratch/archive-todo/spec.md`, `Status: ready-for-agent`, 22 historyjki.
Spec sam z siebie wymienia migrację EF Core dla `ArchivedAt` (zastrzeżenie: agent znał tę pułapkę
z handoffu, sekcja 6) i wskazuje test integracyjny, który pilnuje unieważnienia cache.

`/to-tickets`: agent nie znalazł potrzeby prefactoringu (`ArchivedAt` jak `CompletedAt`, bez konwersji
w konfiguracji EF). Podział przyjęty bez zmian w pierwszej iteracji:

- 01 Archive todo itemu (bez blokad): nowy use case plus filtr w `GET todos` i `archivedAt` w `GET todos/{id}`,
  żeby slice był weryfikowalny przez HTTP; migracja.
- 02 Lista zarchiwizowanych (blokuje 01).
- 03 Unarchive (blokują 01 i 02; krawędź 02 → 03 produktowa: bez listy owner nie zna id do przywrócenia).

Napięcie z `AGENTS.md` („one ticket per use case”): 01 dodaje jeden use case i zmienia dwa istniejące
zapytania. Agent zgłosił to sam w pytaniu o podział; reguła nie mówi, gdzie trafiają zmiany istniejących
use case'ów potrzebne nowemu. Kandydat do `/retro`.

## Krok 4: `/implement`

| Ticket | `add-entity` / `add-feature` | Pierwszy test czerwony (komunikat) | Migracja (sam?) | Build bez suppressions | Red proof | Commit |
|---|---|---|---|---|---|---|
| 01 | `add-feature` tak (razem z `tdd`, jedną decyzją na starcie); `add-entity` nie (brak nowej encji) | Unit H1: błąd kompilacji CS0234 (brak `Application.Todos.Archive`). Pierwszy red na asercji: HTTP I1 `HttpStatusCode.NoContent` oczekiwany, `NotFound` (trasa nie istniała) | Tak, sam, ale z handoffu (sekcja 6). Bez migracji `MigrateAsync` rzuca `PendingModelChangesWarning`. Nagłówek `// <auto-generated />`: decyzja użytkownika | Tak: 0/0; jedyny `#pragma` w `.Designer.cs` z `dotnet ef` | 4 przypadki: autoryzacja, filtr `UserId`, cache, walidator; hashe zgodne | `34fa4f7` |
| 02 | `add-feature` tak (razem z `tdd`); `add-entity` nie | HTTP I1: `HttpStatusCode.OK` oczekiwany, `NotFound` (trasa nie istniała) | Nie dotyczy (brak zmian schematu) | Tak: 0/0, bez `#pragma` | 2 przypadki: autoryzacja, filtr ownera; hashe zgodne | `371556e` |
| 03 | `add-feature` tak (razem z `tdd`); `add-entity` nie | HTTP I1: `HttpStatusCode.NoContent` oczekiwany, `NotFound` (trasa nie istniała) | Nie dotyczy | Tak: 0/0, bez `#pragma` | 4 przypadki w jednym przebiegu: autoryzacja, filtr `UserId`, NotArchived, walidator; hashe zgodne | `f7dbaa1` |

Ticket 01 w szczegółach:

- **Cykle red → green** (każdy red przeczytany przed zmianą kodu):
  - H1 unit NotFound: CS0234, potem green.
  - I1 HTTP archive: `NoContent` ≠ `NotFound`. Green wymagał pola, handlera, endpointu, `ArchivedAt`
    w `GET todos/{id}` i migracji.
  - I2 HTTP lista: lista zawierała oba todo itemy. Filtr w `GetTodosQueryHandler`.
  - I3 HTTP cache: `todoAfterArchiving!.ArchivedAt should not be null but was`. `HybridCache.RemoveAsync`.
  - I4 HTTP cudzy: `NotFound` ≠ `NoContent`. Filtr `UserId`.
  - H5 unit AlreadyArchived: `Error.None` zamiast `TodoItems.AlreadyArchived`. Warunek w handlerze.
  - H6 unit sukces: brak `TodoItemArchivedDomainEvent` w `DomainEvents`. `Raise(...)`.
  - V1 walidator (pusty walidator bez reguł): `Expected a validation error for property TodoItemId`. Reguła.
- **Zielone od pierwszego uruchomienia:** H2 unit cudzy (wszedł razem z poprawką I4; złamany w RP2), 401 HTTP
  (RP1), completion bez zmian (charakteryzacja, bez red proof), walidator dla poprawnej komendy, HTTP 400
  AlreadyArchived (red tylko na poziomie unit H5), HTTP 400 dla pustego id (RP4).
- **Red proof** (hash przed złamaniem i po przywróceniu zgodny):
  - RP1 bez `.RequireAuthorization()`: `Unauthorized` ≠ `InternalServerError`.
  - RP2 bez filtra `UserId` (`_ = userContext;`): `Error.None` zamiast NotFound.
  - RP3 bez `RemoveAsync` (`_ = cache;`): `archivedAt` null.
  - RP4 bez `NotEmpty`: `BadRequest` ≠ `NotFound`.
- **Weryfikacja końcowa:** build 0/0; UnitTests 29/29, ArchitectureTests 6/6, IntegrationTests 15/15.
- **`code-review` przed commitem** (dwa sub-agenty, Standards i Spec, na `git diff HEAD` z `git add -N`):
  - twardych naruszeń i scope creep brak;
  - poprawione przed commitem: asercja completion dla nieukończonego, testy HTTP 400 dla AlreadyArchived
    i pustego id, nazwa testu `ArchiveTodo_Should_SetArchivedAt`, `CompleteTodo_Should_MarkTodoAsCompleted`
    przepięty na helper `CreateTodoAsync`;
  - odłożone do ticketu 03: warunek `ArchivedAt == null` / `is not null` powtarza się w handlerach
    (Primitive Obsession, cena wyboru jednego pola zamiast `IsArchived`).
- **Środowisko:** Docker Desktop zatrzymał się między preflightem a implementacją. Pierwszy przebieg testu
  integracyjnego padł na `DockerUnavailableException`, więc nie był to red. Agent uruchomił Docker Desktop
  ponownie (`Start-Process`) i w tym czasie zrobił cykl H1.

Ticket 02 w szczegółach:

- **Cykle red → green:**
  - I1 HTTP: `OK` ≠ `NotFound` (trasa nie istniała). Minimalny slice bez filtrów.
  - I2 HTTP: niezarchiwizowany był na liście (`should not contain … but does`). Filtr `ArchivedAt != null`.
  - I3 HTTP: cudzy zarchiwizowany był na liście. `IUserContext` i filtr ownera. Zależność weszła do
    konstruktora dopiero tutaj, a seam HTTP konstruktora nie widzi, więc problem CS9113 z ticketu 01 nie wrócił.
  - H1 unit kolejność: kolejność wstawiania zamiast najnowszego na górze. `OrderByDescending`.
- **Zielone od pierwszego uruchomienia:** unit „tylko zarchiwizowane ownera” (RP2), HTTP pusta lista, HTTP 401
  (RP1).
- **Red proof:**
  - RP1 bez `.RequireAuthorization()`: `Unauthorized` ≠ `InternalServerError`.
  - RP2 bez filtra ownera: unit „should have single item but had 2”, HTTP „cudzy” czerwony. HTTP „pusta
    lista” został zielony, bo w świeżej bazie tego przebiegu nikt jeszcze nic nie zarchiwizował. Ten test broni
    kontraktu „200 z pustą listą”, nie filtra ownera.
- **Weryfikacja końcowa:** build 0/0; UnitTests 31/31, ArchitectureTests 6/6, IntegrationTests 20/20.
- **`code-review`:**
  - jedno twarde naruszenie: nazwa testu unit bez `_When{Condition}` (poprawiona);
  - smells: helper `ArchiveTodoAsync` dublowany inline w teście ticketu 01 (przepięty); warunek
    `ArchivedAt != null` już w trzech miejscach (odłożone do 03); kolejność sprawdzana tylko na InMemory,
    choć ADR 0004 kieruje tłumaczenie zapytań do testów integracyjnych (zostawione zgodnie z seamami).
- **Środowisko (dziennik):** w systemie nie ma Pythona (alias Microsoft Store). Skrypt edycji dziennika się nie wykonał,
  a commit `2f423b0` zabrał tylko częściowe zmiany; naprawione kolejnym commitem.

Ticket 03 w szczegółach:

- **Kolejność cykli zmieniona po lekcji z 01:** najpierw HTTP, potem handler. Dzięki temu każda zależność
  (`HybridCache`, `IUserContext`) weszła do konstruktora dopiero wtedy, gdy wymusił ją czerwony test HTTP, i żaden
  cykl nie zaczął się od błędu kompilacji.
- **Cykle red → green:**
  - I1 HTTP: `NoContent` ≠ `NotFound` (trasa nie istniała). Handler po samym id, bez cache, filtra i warunku.
  - I2 HTTP cache: `ArchivedAt should be null but was 2026-10-12T20:14:57…`. `HybridCache.RemoveAsync`.
  - I3 HTTP cudzy: `NotFound` ≠ `NoContent`. `IUserContext` i filtr `UserId`.
  - H1 unit: `Error.None` zamiast `TodoItems.NotArchived`. Warunek.
  - H2 unit: brak `TodoItemUnarchivedDomainEvent` (asercje `ArchivedAt` i completion przeszły). `Raise(...)`.
  - V1 walidator bez reguł: `Expected a validation error for property TodoItemId`. Reguła.
- **Zielone od pierwszego uruchomienia, dopisane od razu (lekcja z review 01):** unit NotFound dla nieistniejącego
  i cudzego, HTTP 400 NotArchived, HTTP 400 dla pustego id, HTTP 401.
- **Red proof** w jednym przebiegu (cztery niezależne mechanizmy, każdy pod innym testem): bez autoryzacji test
  401 dostał 404 (a nie 500 jak w 01, bo zdjęty filtr ownera przestał czytać `UserId`); bez filtra `UserId` unit
  i HTTP „cudzy” czerwone; bez warunku NotArchived unit i HTTP 400 czerwone (`NoContent`); bez reguły walidatora
  HTTP 400 dostał `NotFound`. Hashe trzech plików zgodne po przywróceniu.
- **Weryfikacja końcowa:** build 0/0; UnitTests 38/38, ArchitectureTests 6/6, IntegrationTests 26/26.
- **`code-review`:**
  - Standards bez twardych naruszeń. Smell: nazwa testu sukcesu ukrywała asercje completion; wydzielony
    `Handle_Should_KeepCompletion_WhenTodoIsCompleted`.
  - Testy HTTP sprawdzają tylko status, nie kod błędu (jak w 01 i w testach sprzed pilota): zostawione.
  - Spec bez scope creep; luki HTTP (404 dla nieistniejącego, completion) pokryte testami unit: zostawione.
  - Reguła „zarchiwizowany” jest teraz w czterech miejscach. Oba sub-agenty: nie ruszać w tickecie. Wyliczana
    `IsArchived` nie przełoży się w zapytaniach EF, wspólne wyrażenie to Speculative Generality, a metody
    domenowe `TodoItem.Archive(...)`/`Unarchive()` zmieniłyby styl projektu (szablon `add-feature` trzyma
    mutację w handlerze, `AGENTS.md` dopuszcza publiczne settery), więc wymagają ADR-a. Kandydat do `/retro`.

## Krok 5: `code-review` przeciw `main`, potem `ca-review`

`code-review` (`git diff main...HEAD`, dwa sub-agenty):

- **Standards:** bez twardych naruszeń. Uwagi:
  - brak `GetTodosQueryHandlerTests`, choć handler się zmienił (checklista `ca-review`: „New/changed handlers have
    unit tests”); w review ticketu 01 ta sama luka przeszła jako „akceptowalna wg ADR 0004”;
  - `EnsureSuccessStatusCode()` w `// Assert` trzech testów (przepuszcza każde 2xx);
  - niespójne nazwy „RemoveTodoFromList” / „ReturnTodoToCurrentList”;
  - `TodoDto` deserializuje trzy kontrakty (lista `GET todos` nie ma `ArchivedAt`), pułapka na przyszłe asercje;
  - propozycja hasła „Current list” w słowniku: odrzucona, bo w Q10 użytkownik wybrał brak nowego terminu.
- **Spec:** bez scope creep i bez błędów zachowania. Historie 15–18 (Complete/Update/Delete/Copy zarchiwizowanego)
  działają z konstrukcji, ale **żaden ticket nie miał ich w kryteriach i żaden test ich nie pokrywał**.

Poprawki przed `ca-review` (`add-tests`, commit `02ea947`): `GetTodosQueryHandlerTests` (ścieżka Unauthorized
i filtr zarchiwizowanych), cztery testy HTTP dla historii 15–18, `ShouldBe(NoContent)` zamiast
`EnsureSuccessStatusCode()`, nazwa `ArchiveTodo_Should_RemoveTodoFromCurrentList`.

Red proof dla nowych testów (pięć plików, hashe zgodne po przywróceniu):

- `GetTodos`: warunek Unauthorized wyłączony przez `&& query.UserId == Guid.Empty` (usunięcie zostawiłoby
  nieużywany `using`, czyli IDE0005) i zdjęty filtr; oba testy unit czerwone.
- Complete, Update, Delete: dopisany filtr `ArchivedAt == null`; testy HTTP dostały `NotFound` zamiast
  `NoContent`.
- Copy: kopia dziedziczy `ArchivedAt`. W łączonym przebiegu test padł z **innego powodu**: na liście był też
  oryginał, bo równocześnie zdjęty filtr `GetTodos`. Powtórzony w izolacji: bieżąca lista pusta. Łączony red
  proof zawodzi, gdy łamane mechanizmy dzielą ścieżkę odczytu.

`ca-review` (zakres: `git diff 111f2d4`, czyli merge-base z `main`; drzewo czyste, plików nieśledzonych brak):
0 blockerów, 0 naruszeń konwencji, 0 luk w testach. Build 0/0; UnitTests 40/40, ArchitectureTests 6/6,
IntegrationTests 30/30 (Docker 29.8.1), własny przebieg na `02ea947`. Jedyne `#pragma` i `using Infrastructure.*`
w diffie są w wygenerowanym `.Designer.cs` (zwolniony nagłówkiem `// <auto-generated />`).

## Krok 6: wynik i `/retro`

| Kryterium | Wynik |
|---|---|
| Build: 0 błędów, 0 ostrzeżeń, bez suppressions | **Spełnione.** 0/0 przy każdym tickecie i na końcu. W plikach ręcznych nie ma suppressions. Nagłówek `// <auto-generated />` w pliku migracji dodany ręcznie za zgodą użytkownika, bo `dotnet-ef` 10.0.9 go nie emituje. |
| Testy zielone: unit, architecture, integration (z Dockerem) | **Spełnione.** 40/40, 6/6, 30/30, własny przebieg (było 22, 6, 8). |
| `ca-review` bez uwag | **Spełnione, ale dopiero po poprawkach z `code-review` całej gałęzi.** Bez nich `ca-review` zgłosiłby brak `GetTodosQueryHandlerTests`. |
| `GLOSSARY.md` zaktualizowany | **Spełnione.** Archive, Unarchive, Copy („niezarchiwizowany”), commit `be20074`. |
| Użytkownik nie poprawiał agenta w kwestii konwencji | **Spełnione.** Zero korekt. Jedna decyzja na prośbę agenta (nagłówek migracji), bo konwencje jej nie obejmowały. |

Pilot przeszedł przez cały przepływ `/grill-with-docs` → `/to-spec` → `/to-tickets` → `/implement` ×3 →
`code-review` → `ca-review`. Najważniejsze sygnały z pomiaru:

- Proces sam wydobył wszystkie punkty z listy kontrolnej handoffu i pięć spoza niej (z zastrzeżeniem
  metodycznym: agent znał listę).
- Konwencje projektu wystarczyły do kodu bez korekt; luki były w testach, nie w kodzie produkcyjnym.
- Review per ticket sprawdza kryteria ticketu, więc historie speca, które nie trafiły do żadnego ticketu (15–18),
  wyszły dopiero w review całej gałęzi.

### `/retro` (2026-10-14) i bramka P1

`/retro` dało 10 kandydatów (pełna lista w sekcji „Obserwacje procesu”, numeracja poniżej według ważności).
Decyzja użytkownika: rekomendacja. Na `main` trafiły dziennik i punkty 1–6, 8, 9:

| # | Zmiana | Gdzie |
|---|---|---|
| 1 | Każda historia speca trafia do kryteriów jakiegoś ticketu, także spełniona przez obecny kod (test regresji); review z podanym specem mapuje każdą historię na test | `AGENTS.md` (`to-tickets`), `ca-review` |
| 2 | Migracja dla nowego pola istniejącej encji; nagłówek `// <auto-generated />` w pliku migracji (`dotnet-ef` 10.x go nie emituje); `PendingModelChangesWarning` zamiast „nie zmienia bazy testowej”; usunięta nieprawda o `SharedKernel` w globalnych usingach; przykład archiwizacji na samym `ArchivedAt` | `add-feature`, `add-entity`, `add-tests`, `references/tests.md`, `references/command-slice.md`, `AGENTS.md`, ADR 0004 |
| 3 | Każdy nowy lub zmieniony handler ma test unit; HTTP to drugi seam, nie zastępstwo | ADR 0004 |
| 4 | CI także na `pull_request` do `main` | `.github/workflows/build.yml` |
| 5 | Cykl red → green zaczyna się na seamie HTTP (konstruktor handlera ukryty, brak CS9113) | `add-feature` |
| 6 | Red proof: jeden mechanizm na przebieg, chyba że żadna złamana ścieżka nie zasila cudzej asercji | `add-feature`, `add-tests` |
| 8 | Zamknięcie ticketu: odhaczone kryteria, wynik w `## Comments`, `Status: done` | `docs/agents/issue-tracker.md` |
| 9 | Zmiany istniejących use case'ów potrzebne nowemu idą do jego ticketu | `AGENTS.md` (`to-tickets`) |

Nie wdrożone:

- **7** (review po ticketach tylko na osi Spec, Standards raz na końcu): zmiana w forku Pococka, czyli
  konfiguracja globalna; czeka na osobną zgodę.
- **10** (metody domenowe `Archive`/`Unarchive` zamiast surowych porównań `ArchivedAt`; kody błędów w testach
  HTTP): decyzje projektowe na osobny grill.

Kod archiwizacji, migracja, słownik (Archive, Unarchive, Copy), spec i tickety zostają na gałęzi
`pilot/archive-todo` jako dowód (P1).

Weryfikacja na `main` przed commitem: build 0/0; UnitTests 22/22, ArchitectureTests 6/6, IntegrationTests 8/8.
Pierwsza próba była nieważna: Docker Desktop znów nie działał, `docker info && dotnet build` przerwał łańcuch
przed buildem, a `dotnet test --no-build` puścił binaria z gałęzi pilota (w wynikach testy archiwizacji, których
na `main` nie ma). Lekcja: po `git switch` zawsze build przed `--no-build`, a sprawdzenie Dockera osobno.

Zauważone przy bramce: 2026-10-13 w repo pojawił się nieśledzony katalog `.agents/skills/`, kopia 1:1 czterech
skilli projektu (bez `README.md`), utworzona poza tą sesją. Decyzja użytkownika: nie ruszać i nie commitować;
poprawki skilli są tylko w `.claude/skills/`, więc kopia jest od teraz nieaktualna.

## Korekty użytkownika

Format: krok, co zrobił agent, co poprawił użytkownik, czy korekta dotyczy konwencji projektu.

_brak korekt._ Decyzje, o które agent sam poprosił, bo konwencje ich nie rozstrzygały (to nie korekty):

- Krok 4, ticket 01: migracja z `dotnet-ef` 10.0.9 bez nagłówka `// <auto-generated />`, a obie wcześniejsze
  migracje go mają; build padał na IDE0161. Użytkownik wybrał dopisanie nagłówka (rekomendacja agenta)
  zamiast przerabiania wygenerowanego kodu pod analizatory.

## Obserwacje procesu

Kandydaci do `/retro` (nie poprawiać w pilocie):

1. **TDD w .NET z analizatorami jako błędami.** Szkielet z `NotImplementedException` albo nowa zależność
   w konstruktorze, której jeszcze nie używamy, to CS9113 (nieczytany parametr), więc pierwszy red nowej klasy
   jest błędem kompilacji, a nowej zależności nie da się dodać przed jej użyciem. Prawdziwy red dla filtra
   ownera dał dopiero test HTTP, który konstruktora handlera nie widzi. Fork `tdd` tego nie opisuje.
2. **Dryf skilli projektu względem kodu i narzędzi:**
   - `add-feature/references/command-slice.md` modeluje archiwizację parą `IsArchived` + `ArchivedAt`,
     sprzecznie ze specem (Q8); agent poszedł za specem, ale przykład w skillu sugeruje model;
   - `add-feature/references/tests.md` twierdzi, że `GlobalUsings.cs` testów unit importuje `SharedKernel`;
     nie importuje;
   - `add-entity` twierdzi, że `dotnet ef` emituje `// <auto-generated />` w pliku migracji; 10.0.9 robi to
     tylko w `.Designer.cs`;
   - ADR 0004: „zmiana mapowania bez migracji nie zmienia bazy testowej” jest za słabe; EF Core rzuca
     `PendingModelChangesWarning` przy `MigrateAsync`, więc brak migracji wysypuje cały suite integracyjny.
3. **„One ticket per use case” a zmiany istniejących use case'ów** potrzebne nowemu (krok 3).
4. **Sub-agenci:** `grilling` każe szukać faktów przez sub-agenta (agent zrobił to sam), `code-review` uruchamia
   dwa sub-agenty (agent je uruchomił). `code-review` zakłada diff `<fixed-point>...HEAD`, a `implement` każe
   robić review przed commitem; obejście: `git add -N` i `git diff HEAD`.
5. **Komendy użytkownika:** `/to-spec` w środku zdania nie uruchamia skilla (krok 3).
6. **Anemiczna encja a reguła stanu:** „zarchiwizowany” jako surowe porównanie `ArchivedAt` z null w czterech
   miejscach; metody domenowe zamknęłyby guard, mutację i `Raise` w encji, ale to zmiana stylu projektu (ADR).
7. **Testy HTTP sprawdzają tylko status,** nie kod błędu z ProblemDetails; kod sprawdzają wyłącznie testy unit.
   Konwencja tego nie rozstrzyga.
8. **`to-tickets` gubi historie „bez zmian w kodzie”:** historie 15–18 speca (operacje na zarchiwizowanym) nie
   trafiły do kryteriów żadnego ticketu, bo decyzja brzmiała „istniejące use case'y się nie zmieniają”. Wyszły
   dopiero w review całej gałęzi.
9. **ADR 0004 kontra checklista `ca-review`:** ADR woli HTTP jako seam, checklista wymaga testów unit dla każdego
   zmienionego handlera. Review ticketu 01 uznał brak `GetTodosQueryHandlerTests` za akceptowalny, review gałęzi
   za lukę.
10. **Łączony red proof** zawodzi, gdy łamane mechanizmy dzielą ścieżkę odczytu (Copy + filtr `GetTodos`).
    Skille `add-feature`/`add-tests` opisują red proof pojedynczo i nie ostrzegają przed łączeniem.
11. **`add-tests`** powtarza nieprawdę `add-feature` o `SharedKernel` w globalnych usingach.
12. **Tracker bez stanu „zrobione”:** tickety po implementacji mają odhaczone kryteria, ale nadal
    `Status: ready-for-agent`; `docs/agents/triage-labels.md` nie ma roli na zamknięcie.
