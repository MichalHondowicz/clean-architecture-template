> Nieaktualny. Wykonaj docs/handoff-2026-10-14-triage.md

# Handoff: ticket 403 dla cudzego `userId` (ADR 0005)

Uruchom tę sesję w `D:\Projects\clean-architecture_new`. Wdrażasz decyzję z ADR 0005: niezgodność ownera
podana przez klienta w `userId` ma dawać 403, a nie 500. To pierwszy przebieg po poprawkach warstwy agentowej
z retro Fazy 5, więc przy okazji sprawdzasz, czy te poprawki działają.

## 1. Stan wejściowy

- Gałąź `main`, HEAD to commit dodający ten plik. Drzewo czyste poza nieśledzonym `.agents/` (sekcja 5:
  nie ruszaj go).
- Faza 5 planu (`docs/plan-2026-10-12-agent-setup.md`) zamknięta. Pilot „archiwizacja todo itemu” leży na
  lokalnej gałęzi `pilot/archive-todo` jako dowód (decyzja P1 a). Nie merguj jej i nie usuwaj.
- Ostatni własny przebieg na `main` (2026-10-14, Docker 29.8.1, świeży build): build 0 błędów, 0 ostrzeżeń;
  UnitTests 22/22, ArchitectureTests 6/6, IntegrationTests 8/8. Powtórz go przed pracą, nie zakładaj.
- Repo nie ma remote'a (`git remote -v` jest puste). CI z `.github/workflows/build.yml` (od 2026-10-14 także na
  `pull_request`) jeszcze nigdy się nie uruchomiło. Jedyną weryfikacją są Twoje własne przebiegi.

## 2. Co robimy dalej

Ticket `.scratch/ownership-forbidden/issues/01-ownership-mismatch-returns-403.md` (plik istnieje,
`Status: ready-for-agent`). Zgodnie z decyzją P2 a z pilota idzie jako osobny przebieg samego `/implement`, bez
grilla, specu i `to-tickets`, bo ticket już jest. Stoimy przed bramką B1–B4 (sekcja 4).

## 3. Co przeczytać na start

- Ticket 403: kryteria akceptacji, w tym ostatnie (dokumenty i skille mają mówić 403).
- `docs/adr/0005-ownership-mismatch-returns-forbidden.md`: decyzja i odrzucone opcje (dlaczego nie 404 dla
  cudzego `userId`).
- `AGENTS.md`: „Code conventions” → „Ownership” i „Endpoints” (fakt o 500, który zmieniasz) oraz „Process skills
  in this project” (zmienione w retro).
- `docs/adr/0004-test-strategy.md`: nowa konsekwencja, czyli każdy zmieniony handler ma test unit.
- `docs/pilot-2026-10-archive-todo.md`: sekcja „`/retro` (2026-10-14) i bramka P1” (co i gdzie zmieniono
  w warstwie agentowej) oraz „Obserwacje procesu” (pułapki pilota).
- Kod, który ticket rusza: `src/SharedKernel/ErrorType.cs`, `src/Web.Api/Infrastructure/CustomResults.cs`
  (typ błędu → status, tytuł, opis, link RFC), `src/Domain/Users/UserErrors.cs`.

## 4. Otwarte pytania na bramkę

Zadaj je jednym formularzem przed `/implement`.

- **B1. Gdzie praca?**
  - (a) Gałąź `fix/ownership-403` od `main`; po zielonym `ca-review` scalenie fast-forward do `main` za zgodą
    użytkownika. **Rekomendacja.** ADR 0005 to decyzja dla template'u, więc kod, inaczej niż w pilocie, ma
    trafić na `main`.
  - (b) Commity prosto na `main`.
- **B2. Nazwa błędu.**
  - (a) Nowa fabryka `UserErrors.Forbidden()` (kod `Users.Forbidden`, nowy `ErrorType.Forbidden`) zamiast
    `Unauthorized()`, bo w HTTP „Unauthorized” znaczy 401. **Rekomendacja.**
  - (b) Zostawić `UserErrors.Unauthorized()` i zmienić tylko jego typ.
- **B3. Zakres: pięć wywołań, cztery trasy.** `UserErrors.Unauthorized()` zwracają handlery Create, Get i Copy
  todo itemów, `GetUserById` oraz `GetUserByEmailQueryHandler`. Ticket i ADR wymieniają cztery trasy. Piąty
  handler nie ma endpointu i zwraca błąd, gdy e-mail należy do innego usera.
  - (a) Objąć go zmianą jak resztę i założyć ticket `needs-triage`: przy wyszukiwaniu po e-mailu 403 zdradza,
    że konto istnieje (enumeracja kont), a 404 `NotFoundByEmail` by tego nie zdradzało. **Rekomendacja.**
  - (b) Od razu zwracać tam `NotFoundByEmail`.
  - (c) Zostawić tam 500.
- **B4. Punkt 7 z retro:** review po każdym tickecie tylko na osi Spec, a oś Standards raz na końcu przez
  `ca-review`. To zmiana w forku Pococka, czyli konfiguracja globalna.
  - (a) Bez zmian na ten przebieg (jeden ticket, mały koszt); decyzja w Fazie 6. **Rekomendacja.**
  - (b) Zmienić fork teraz.

## 5. Dług i ustalenia przeniesione

**Nie spłacaj ich tutaj bez decyzji.**

- `.scratch/copy-todo-created-event/` (`needs-triage`): copy publikuje zdarzenie z pustym id. To ten sam
  `CopyTodoCommandHandler`, który rusza ticket 403. Nie naprawiaj go przy okazji.
- `.scratch/refresh-token-lifetime/` (`needs-triage`): sesja bez bezwzględnego limitu.
- Retro, punkt 10: metody domenowe `TodoItem.Archive()`/`Unarchive()` zamiast porównań `ArchivedAt` z null
  oraz kody błędów w asercjach testów HTTP. To decyzje projektowe na osobny grill; dotyczą gałęzi pilota.
- `.agents/skills/`: nieśledzona kopia czterech skilli projektu, powstała 2026-10-13 poza sesją agenta.
  Użytkownik zdecydował: nie ruszać, nie commitować. Kopia nie ma poprawek z retro. Edytuj wyłącznie
  `.claude/skills/`. Źródło kopii nie jest ustalone.
- Faza 6 planu: los `dotnet-workflow`; stary `D:\Projects\clean-architecture` (jego restore pada na NuGet audit
  przez `Testcontainers.PostgreSql` 4.12.0); publikacja template'u (bez remote'a CI nie działa); kopia zapasowa
  forka (branch `dotnet` w `D:\Projects\skills` jest tylko lokalnie); port `wizard` na PowerShell tylko na
  wyraźne życzenie.

## 6. Środowisko i pułapki, które kosztowały czas

- **Docker Desktop staje** między sesjami i w trakcie (2026-10-12 i 2026-10-14). Objawy: `docker info` pisze
  „failed to connect to the docker API at npipe:////./pipe/dockerDesktopLinuxEngine”, a Testcontainers rzuca
  `DockerUnavailableException`. To nie jest red. Uruchom go:
  `Start-Process "C:\Program Files\Docker\Docker\Docker Desktop.exe"`, potem czekaj w pętli, aż `docker info`
  odpowie (30–60 s).
- **Nieaktualne binaria.** Nie łącz `docker info && dotnet build; dotnet test --no-build`. Gdy Docker leży, build
  się nie wykona, a testy pójdą na starych binariach. 2026-10-14 po `git switch` z pilota na `main` w wynikach
  były testy archiwizacji, których na `main` nie ma. Po każdym `git switch`: najpierw `dotnet build`, potem
  `--no-build`.
- **Brak Pythona.** `python` uruchamia alias Sklepu Microsoft i nic nie robi, a łańcuch komend idzie dalej.
  Pliki edytuj narzędziem Edit albo `sed`.
- **Analizatory jako błędy a TDD.** Nieczytany parametr konstruktora to CS9113, czyli błąd kompilacji, a nie red.
  Zaczynaj cykl od testu HTTP (skill `add-feature`, krok 5). W red proof złamanie musi się kompilować: osłabiaj
  warunek (np. `&& query.UserId == Guid.Empty`) zamiast go usuwać, bo usunięcie zostawia nieużywany `using`
  (IDE0005).
- **`code-review` przed commitem.** Skill porównuje `<fixed-point>...HEAD`, a `/implement` każe robić review przed
  commitem. Obejście: `git add -N <nowe pliki>` i diff `git diff HEAD`.
- **Skille tylko dla użytkownika** (`/grill-with-docs`, `/to-spec`, `/to-tickets`, `/implement`, `/retro`): komenda
  działa tylko jako osobna wiadomość zaczynająca się od ukośnika. Nie wywołasz ich narzędziem Skill.
- **Git Bash przerabia ścieżki:** przed `git show ref:plik` i `git diff main...HEAD` dodaj `MSYS_NO_PATHCONV=1`.
- **NuGet audit** potrafi zepsuć restore z dnia na dzień. Podbij pakiet nadrzędny, nigdy suppression.

## 7. Workflow

1. Preflight i bramka B1–B4 (sekcja 9).
2. Użytkownik wpisuje `/implement .scratch/ownership-forbidden/issues/01-ownership-mismatch-returns-403.md`.
   Agent wywołuje `tdd`. Żaden skill projektu nie pasuje 1:1, bo zmiana jest przekrojowa (SharedKernel, Domain,
   Web.Api, testy pięciu ścieżek, dokumenty). `add-tests` pasuje do testów istniejących use case'ów. Zapisz
   w `## Comments` ticketu, które skille wywołałeś.
3. Ostatnie kryterium ticketu (dokumenty mówią 403) obejmuje każde miejsce z „500”:
   `grep -rn "500" AGENTS.md .claude/skills docs/adr`. Dziś to `AGENTS.md`, `add-entity`, `add-feature`
   (z `references/command-slice.md`), `add-tests`, `ca-review`, ADR 0002 i ADR 0005.
4. `code-review` (wywołuje go `/implement`), potem `ca-review` całej gałęzi.
5. Zamknięcie ticketu według nowej konwencji z `docs/agents/issue-tracker.md`: odhaczone kryteria, wynik
   w `## Comments`, `Status: done`.
6. Bramka u użytkownika: scalenie do `main` (B1).

Poprawki z retro, które ten przebieg sprawdza. Zapisz wynik w `## Comments` ticketu:

- czy cykle zaczynały się od testu HTTP;
- czy red proof szedł po jednym mechanizmie;
- czy `ca-review` wymagał testów unit dla każdego zmienionego handlera (ADR 0004).

## 8. Lekcje metodyczne do powtórzenia

1. **Liczby testów tylko z własnego przebiegu, na świeżym buildzie bieżącej gałęzi.** 2026-10-14 `--no-build` po
   `git switch` zaraportowałby 30 testów z innej gałęzi.
2. **Listy z ADR-ów i ticketów sprawdzaj grepem w kodzie.** Lista tras w ADR 0005 jest niepełna drugi raz
   z rzędu: wcześniej brakowało `POST todos`, teraz `GetUserByEmailQueryHandler`.
3. **Review po tickecie nie zastępuje review całej gałęzi.** Review ticketu sprawdza jego kryteria. Luki między
   ticketami (historie speca, testy zmienionych handlerów) wyszły w pilocie dopiero na końcu.
4. **Red proof: jeden mechanizm na przebieg.** W pilocie łączony przebieg dał fałszywy wynik, bo dwa złamane
   mechanizmy dzieliły ścieżkę odczytu. Test, który nigdy nie był czerwony, niczego nie dowodzi.
5. **Fakty w skillach sprawdzaj z narzędziem i kodem.** Trzy twierdzenia skilli okazały się nieprawdą: nagłówek
   migracji z `dotnet-ef` 10, globalne usingi i „migracja bez zmian nic nie psuje”. Poprawione w `698509c`,
   ale kopia w `.agents/` nadal je ma.
6. **Konflikt dwóch reguł zamieniaj w decyzję zapisaną w jednym miejscu.** ADR 0004 i checklista `ca-review`
   dały w pilocie dwa różne werdykty, dopóki ADR tego nie rozstrzygnął.
7. **Nie spłacaj długu przy okazji.** Dzięki temu pomiar w pilocie był czysty, a dług ma swoje tickety.

## 9. Pierwszy krok i miejsce zatrzymania

Preflight, w tej kolejności:

1. `git status`: `main`, czyste poza `.agents/`.
2. `docker info` jako osobna komenda; w razie potrzeby uruchom Docker Desktop (sekcja 6).
3. `dotnet build CleanArchitecture.slnx`.
4. `dotnet test CleanArchitecture.slnx` z własnymi liczbami (oczekiwane 22/6/8).

Potem przeczytaj pliki z sekcji 3 i zadaj B1–B4 jednym formularzem. Po odpowiedziach utwórz gałąź (jeśli
B1 a) i **zatrzymaj się**: poproś użytkownika, żeby wpisał
`/implement .scratch/ownership-forbidden/issues/01-ownership-mismatch-returns-403.md`.
