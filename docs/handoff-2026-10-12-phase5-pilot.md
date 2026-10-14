> Nieaktualny. Wykonaj docs/handoff-2026-10-14-ownership-403.md

# Handoff: Faza 5, pilot end-to-end „archiwizacja todo itemu”

Uruchom tę sesję w `D:\Projects\clean-architecture_new`. Pracujesz według planu
`docs/plan-2026-10-12-agent-setup.md`, Faza 5. Pilot mierzy, czy proces Pococka (dostosowany do .NET) razem
z warstwą konwencji projektu doprowadza małą funkcję od rozmowy do zielonego kodu bez poprawiania agenta
w sprawie konwencji. Ty jesteś tym agentem i jednocześnie prowadzisz dziennik obserwacji.

## 1. Stan wejściowy

- `D:\Projects\clean-architecture_new`: repo git, gałąź `main`, HEAD to commit dodający ten plik, drzewo
  czyste. Fazy 0–4 są zamknięte; każda ma w planie linię **Wynik** z odstępstwami.
- Ostatni własny pełny przebieg (po Fazie 2; Fazy 3–4 zmieniły tylko dokumenty i `.scratch/`): build 0 błędów
  i 0 ostrzeżeń, UnitTests 22/22, ArchitectureTests 6/6, IntegrationTests 8/8 (z Dockerem). Powtórz go przed
  pilotem, nie zakładaj.
- Skille Pococka: fork na branchu `dotnet` w `D:\Projects\skills` (commit `2f5489b`), zainstalowany jako
  `mattpocock-skills@mattpocock` w wersji `1.3.1-dotnet.1` z lokalnego marketplace'u (katalog). superpowers
  jest wyłączony dla tego projektu w `.claude/settings.json`.

## 2. Co robimy dalej

Faza 5 planu na gałęzi `pilot/archive-todo` (jeszcze nie istnieje). Pliku zadania nie ma i nie powstanie:
spec i tickety zapisze sam proces w `.scratch/archive-todo/` (tracker lokalny, opis w
`docs/agents/issue-tracker.md`). Dziennik pilota: `docs/pilot-2026-10-archive-todo.md` (jeszcze nie istnieje).

## 3. Co przeczytać na start

- `docs/plan-2026-10-12-agent-setup.md`: sekcja „Cel i efekt końcowy” (kryteria sukcesu pilota), „Faza 5”
  (kroki) oraz linie **Wynik** pod Fazami 3 i 4 (co się zmieniło względem planu).
- `AGENTS.md` w całości. Najważniejsze: `## Agent skills` → „Process skills in this project” (jak `to-tickets`,
  `implement`, `tdd`, `code-review` mapują się na ten projekt), „Ownership” i „Endpoints” (fakty o 500 i
  o `HasPermission`).
- `GLOSSARY.md`: „Todo item”, „Completion”, „Owner”. „Archive” nie ma jeszcze definicji (pada tylko jako
  przykład nazwy `ArchiveTodoCommand`); to pierwsze pojęcie, które pilot ma dopisać.
- `docs/adr/`: 0004 (gdzie który test), 0005 (403 w przyszłości, dziś 500), 0006 (zdarzenia po zapisie,
  id przed `Raise(...)`).
- `.claude/skills/add-feature/SKILL.md`: co ma się wydarzyć przy każdym tickecie.

## 4. Otwarte pytania na bramkę

Pytania dzielą się na dwie grupy. Pierwszą rozstrzygnij z użytkownikiem przed pilotem. Drugą zostaw
grillowi: pilot mierzy, czy `/grill-with-docs` sam je wydobędzie, więc nie rozstrzygaj ich zawczasu, tylko
po grillu odhacz w dzienniku, które padły.

**Przed pilotem:**

- **P1. Co z kodem pilota po zakończeniu?** (a) Gałąź zostaje jako dowód, a do `main` trafia tylko dziennik
  i poprawki warstwy agentowej (`AGENTS.md`, skille, ADR-y). Template zostaje generyczny. **Rekomendacja.**
  (b) Scalić archiwizację do `main` jako funkcję przykładową template'u.
- **P2. Kolejność względem ticketu 403** (`.scratch/ownership-forbidden/`, `ready-for-agent`). (a) Najpierw
  pilot; 403 później jako osobny przebieg samego `/implement`. **Rekomendacja.** Archiwizacja filtruje po
  `UserId` (404), więc 403 jej nie dotyczy. (b) Najpierw 403 jako rozgrzewka.

**Dla grilla (lista kontrolna do dziennika, rekomendacje tylko dla orientacji):**

- Kto może archiwizować: tylko owner (filtr `UserId`, cudzy todo item to 404).
- Czy da się przywrócić (unarchive): rekomendacja tak, osobny use case.
- Czy zarchiwizowane znikają z `GET todos`: rekomendacja tak; `GET todos/{id}` nadal je zwraca.
- Relacja z Completion: czy można archiwizować nieukończony i czy można ukończyć zarchiwizowany.
- Cache: `GET todos/{id}` jest w `HybridCache`, więc archiwizacja musi go unieważnić.
- Zdarzenie domenowe `TodoItemArchived` z id nadanym przed `Raise(...)` (ADR 0006).

## 5. Dług i ustalenia przeniesione

**Nie spłacaj ich w pilocie bez decyzji.** Pilot ma mierzyć proces, a każda poprawka „przy okazji” zaciera
pomiar.

- `.scratch/ownership-forbidden/`: 403 zamiast 500 (ADR 0005), `ready-for-agent`. Osobny przebieg po pilocie
  (P2).
- `.scratch/refresh-token-lifetime/`: sesja bez bezwzględnego limitu, `needs-triage`.
- `.scratch/copy-todo-created-event/`: copy publikuje zdarzenie z pustym id. Ustalone z czytania kodu, bez
  testu; `needs-triage`.
- Faza 6 planu: zamrożenie `dotnet-workflow`, los starego `D:\Projects\clean-architecture` (ten nadal ma
  `Testcontainers.PostgreSql` 4.12.0, więc jego restore pada na NuGet audit), czy publikować template.
- Branch `dotnet` w `D:\Projects\skills` istnieje tylko lokalnie (jedyny remote to repo Matta). Decyzja
  o kopii zapasowej (własny fork na GitHubie) należy do Fazy 6.
- `wizard` w forku: zamiast portu na PowerShell jest instrukcja „uruchom z Git Bash”. Pełny port tylko na
  wyraźne życzenie użytkownika.

## 6. Środowisko i pułapki, które kosztowały czas

- **Skille Pococka, które może uruchomić tylko użytkownik:** `/grill-with-docs`, `/to-spec`, `/to-tickets`,
  `/implement`, `/retro` (mają `disable-model-invocation: true`). Nie wywołasz ich narzędziem Skill. W pilocie
  **wpisuje je użytkownik**: to część pomiaru. Ty możesz sam wywołać `code-review`, `tdd`, `domain-modeling`,
  `grilling`.
- **`/to-spec` składa spec z bieżącej rozmowy.** Grill i spec muszą być w tej samej sesji, inaczej spec
  powstanie z niczego.
- **Sprawdź, że działa fork, a nie upstream.** `installed_plugins.json` ma mieć wersję `1.3.1-dotnet.1`, a
  `tdd/tests.md` w `~/.claude/plugins/cache/mattpocock/mattpocock-skills/1.3.1-dotnet.1/` ma zawierać
  przykłady `csharp`. Jeśli na starcie sesji pojawia się wstrzyknięty „using-superpowers”, wyłączenie per
  projekt nie zadziałało: zatrzymaj się i zgłoś, bo pilot z trzema procesami naraz nic nie zmierzy.
- **Testy integracyjne wymagają Dockera** (Testcontainers, `postgres:17`). Najpierw `docker info`. Bez
  Dockera praca jest niezweryfikowana, a nie zielona.
- **NuGet audit potrafi zepsuć build z dnia na dzień.** 2026-10-12 restore padł na nowych advisory dla
  `SSH.NET` ciągniętego przez Testcontainers. Naprawa: podbić pakiet nadrzędny, nigdy suppression.
- **Analizatory są błędami** (`AnalysisMode=All`, `TreatWarningsAsErrors`, Sonar). Zakazy suppressions:
  `AGENTS.md`, „Build and verification”.
- **Nowa właściwość na istniejącej encji wymaga migracji**, a `add-feature` o migracji nie mówi (robi to
  `add-entity` dla nowych encji). Komenda jest w `AGENTS.md`. Testy integracyjne odtwarzają migracje
  (`MigrateAsync`), więc bez migracji kolumna nie istnieje w bazie testowej. Zanotuj w dzienniku, czy agent
  sam na to wpadł.
- **Git Bash przerabia ścieżki w argumentach:** `git show origin/main:plik` dostaje `origin\main;plik`.
  Poprzedź komendę `MSYS_NO_PATHCONV=1`.
- `claude -p` uruchomione z sesji desktop dostawało 401. Sesje bezobsługowe prowadź w CLI w terminalu
  użytkownika.

## 7. Workflow

Kroki Fazy 5 z planu, z bramkami u użytkownika:

1. Preflight (sekcja 9) i gałąź `pilot/archive-todo` od `main`.
2. Użytkownik: `/grill-with-docs archiwizacja todo itemu`. Bramka: zmiany w `GLOSSARY.md` i ewentualne ADR-y.
3. Użytkownik: `/to-spec`, potem `/to-tickets`. Bramka: podział na tickety, zanim powstaną pliki.
4. Użytkownik: `/implement` dla każdego ticketu. Commity na gałęzi pilota robi `/implement`; na `main` nic bez
   zgody.
5. `code-review` przeciw `main`, potem `ca-review`.
6. Dziennik z wynikiem względem kryteriów z sekcji „Cel”, potem użytkownik: `/retro`. Bramka: co z dziennika
   i retro trafia do `main` (P1).

Przez cały pilot prowadź `docs/pilot-2026-10-archive-todo.md`: przy każdym kroku zapisz, czy agent wywołał
`add-feature`/`add-entity`, czy pierwszy test był czerwony (z komunikatem), czy build przeszedł bez
suppressions i każdą korektę, której użytkownik musiał dokonać. Zmiany w `~/.claude/settings.json` i
instalacja pluginów to konfiguracja globalna: każda za osobną zgodą.

## 8. Lekcje metodyczne do powtórzenia

1. **Liczby testów tylko z własnego przebiegu.** Liczba 8/8 dla integracji krążyła w dokumentach jako fakt,
   choć pochodziła z raportu subagenta; potwierdził ją dopiero własny przebieg 2026-10-12.
2. **Sprawdź świeżość, zanim zbudujesz na „stanie zastanym”.** Plan miał kilka dni, a klon Pococka był już 18
   commitów za upstreamem, a restore padał na advisory opublikowanych po ostatnim zielonym buildzie.
3. **Rekomendacje planu weryfikuj z kodem.** Plan kazał traktować solucję z wieloma projektami jako sygnał
   monorepo, co zakwalifikowałoby sam ten template; lista tras dla ADR 0005 pominęła `POST todos`. Oba błędy
   wyszły dopiero przy czytaniu kodu.
4. **Słownik buduj z zachowania kodu, nie z nazw.** `Priority` okazał się tylko zapisywany, a `HasPermission`
   przepuszcza każdego zalogowanego. Słownik z samych nazw zakodowałby fałszywe znaczenia.
5. **Konflikt skill kontra projekt zamieniaj w ADR,** nie w regułę w promptcie. Skille Pococka czytają
   `docs/adr/` (patrz `docs/agents/domain.md`), więc ADR rozstrzyga konflikt trwale.
6. **W pilocie nie poprawiaj procesu w locie.** Każda ręczna korekta agenta to dane do dziennika, a nie
   problem do zamiecenia. Popraw warstwę agentową dopiero po `/retro`, na `main`.

## 9. Pierwszy krok i miejsce zatrzymania

Preflight, w tej kolejności: `git status` (czyste, na `main`), `docker info`, sprawdzenie forka i braku
superpowers (sekcja 6), `dotnet build CleanArchitecture.slnx` i `dotnet test CleanArchitecture.slnx` z własnymi
liczbami. Potem zadaj P1 i P2 jednym formularzem. Po odpowiedziach utwórz gałąź `pilot/archive-todo` i szkielet
dziennika, i **zatrzymaj się**: poproś użytkownika, żeby wpisał `/grill-with-docs archiwizacja todo itemu`.
