# Dziennik pilota: archiwizacja todo itemu (Faza 5)

Start: 2026-10-12. Gałąź `pilot/archive-todo` od `main` (`111f2d4`). Plan: `docs/plan-2026-10-12-agent-setup.md`,
Faza 5. Handoff: `docs/handoff-2026-10-12-phase5-pilot.md`. Spec i tickety: `.scratch/archive-todo/`.

Agent prowadzący pilot jest jednocześnie autorem tego dziennika. Każda korekta, którą musiał zrobić
użytkownik, trafia do sekcji „Korekty użytkownika”, także jeśli wydaje się drobna.

## Kryteria sukcesu (plan, sekcja „Cel i efekt końcowy”)

| Kryterium | Wynik |
|---|---|
| Build: 0 błędów, 0 ostrzeżeń, bez suppressions | _do uzupełnienia_ |
| Testy zielone: unit, architecture, integration (z Dockerem) | _do uzupełnienia_ |
| `ca-review` bez uwag | _do uzupełnienia_ |
| `GLOSSARY.md` zaktualizowany (co najmniej „Archive”) | _do uzupełnienia_ |
| Użytkownik nie poprawiał agenta w kwestii konwencji | _do uzupełnienia_ |

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
| 02 | `add-feature` tak (razem z `tdd`); `add-entity` nie | HTTP I1: `HttpStatusCode.OK` oczekiwany, `NotFound` (trasa nie istniała) | Nie dotyczy (brak zmian schematu) | Tak: 0/0, bez `#pragma` | 2 przypadki: autoryzacja, filtr ownera; hashe zgodne | `371556e` |
| 01 | `add-feature` tak (razem z `tdd`, jedną decyzją na starcie); `add-entity` nie (brak nowej encji) | Unit H1: błąd kompilacji CS0234 (brak `Application.Todos.Archive`). Pierwszy red na asercji: HTTP I1 `HttpStatusCode.NoContent` oczekiwany, `NotFound` (trasa nie istniała) | Tak, sam, ale z handoffu (sekcja 6). Bez migracji `MigrateAsync` rzuca `PendingModelChangesWarning`. Nagłówek `// <auto-generated />`: decyzja użytkownika | Tak: 0/0; jedyny `#pragma` w `.Designer.cs` z `dotnet ef` | 4 przypadki: autoryzacja, filtr `UserId`, cache, walidator; hashe zgodne | `34fa4f7` |

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
- **Środowisko (ticket 01):** Docker Desktop zatrzymał się między preflightem a implementacją. Pierwszy przebieg testu
  integracyjnego padł na `DockerUnavailableException`, więc nie był to red. Agent uruchomił Docker Desktop
  ponownie (`Start-Process`) i w tym czasie zrobił cykl H1.

## Krok 5: `code-review` przeciw `main`, potem `ca-review`

_do uzupełnienia_

## Krok 6: wynik i `/retro`

_do uzupełnienia_

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
