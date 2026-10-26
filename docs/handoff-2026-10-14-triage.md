> Nieaktualny. Wykonaj docs/handoff-2026-10-26-agent-loop-05.md

# Handoff: triage ticketów po zamknięciu planu i pierwszy ticket z nową regułą review

Uruchom tę sesję w `D:\Projects\clean-architecture_new`. Plan `docs/plan-2026-10-12-agent-setup.md` jest
zamknięty (fazy 0–6). Zaczynasz od triage czterech ticketów, które czekają na decyzję, a potem prowadzisz jeden
z nich przez `/implement`. To pierwszy przebieg z regułą F4 z `AGENTS.md`, więc przy okazji sprawdzasz, czy ona
działa.

## 1. Stan wejściowy

- Gałąź `main`, HEAD to commit dodający ten plik. Drzewo czyste poza nieśledzonym `.agents/` (sekcja 5: nie
  ruszaj go). `main` jest zsynchronizowany z `origin/main`.
- Ostatni własny przebieg (2026-10-14, Docker 29.8.1, świeży build `57ff7e2`, ostatniego commitu z kodem): build
  0 błędów, 0 ostrzeżeń; UnitTests 34/34, ArchitectureTests 6/6, IntegrationTests 13/13. CI na GitHubie dla
  `57ff7e2`: Build #22, Success. Powtórz przebieg przed pracą, nie zakładaj go.
- Remote `origin` to prywatne repo `MichalHondowicz/clean-architecture-template`. W chwili pisania nie było
  otwartych PR-ów, ale Dependabot działa codziennie, więc nowe mogą czekać.
- Gałąź `pilot/archive-todo` (lokalnie i na `origin`) to dowód pilota Fazy 5: nie scalaj jej i nie otwieraj
  z niej PR-a.

## 2. Co robimy dalej

Triage czterech ticketów `needs-triage` z `.scratch/` (pliki istnieją, lista w T2), potem `/implement` jednego
ticketu `ready-for-agent`. Stoimy przed bramką T1–T4 (sekcja 4). Nowych plików ticketów nie zakładasz, chyba że
bramka tak zdecyduje.

## 3. Co przeczytać na start

- `AGENTS.md`: „Process skills in this project” → punkt `code-review` (reguła F4: w `/implement` review ticketu
  tylko na osi Spec, oś Standards raz na gałąź przez `ca-review`); „Code conventions” → Ownership i Tests.
- Cztery tickety z T2 i `docs/agents/triage-labels.md` (role statusów). `docs/agents/issue-tracker.md`: jak
  zamyka się ticket po `/implement`.
- `.scratch/ownership-forbidden/issues/01-ownership-mismatch-returns-403.md`, sekcja `## Comments`: wzór
  zamkniętego ticketu (seamy, cykle, tabela red proof, wynik review).
- ADR-y według ticketu: `docs/adr/0006-domain-events-after-commit.md` (copy event), `0005` (enumeracja po
  e-mailu), `0002` (typy porażek, dla poziomu logowania).
- `docs/plan-2026-10-12-agent-setup.md`, Faza 6, „Wynik”: decyzje F1–F4 i obsługa Dependabota.
- Dla T3: `docs/pilot-2026-10-archive-todo.md`, „Obserwacje procesu”, punkt 7 (testy HTTP sprawdzają tylko
  status).

## 4. Otwarte pytania na bramkę

Zadaj je jednym formularzem (AskUserQuestion, najwyżej 4 pytania) przed jakąkolwiek zmianą.

- **T1. Co robimy najpierw?**
  - (a) Triage czterech ticketów (T2), potem `/implement` jednego ticketu `ready-for-agent`. **Rekomendacja.**
    Decyzje są krótkie, a `/implement` sprawdzi regułę F4, której jeszcze nikt nie użył.
  - (b) Przygotowanie publikacji template'u: licencja upstreamu Milana Jovanovića (w kopii nie ma `LICENSE`)
    i własne `README.md` (dziś to README Milana).
  - (c) Grill punktu 10 retro (T3).
- **T2. Status czterech ticketów** (role z `docs/agents/triage-labels.md`). Rekomendacje:
  - `.scratch/copy-todo-created-event/issues/01-copy-raises-event-with-copy-id.md`: `ready-for-agent`.
    Realny błąd w kodzie template'u, kryteria kompletne, reguła już jest w ADR 0006. Rekomendowany pierwszy
    ticket do `/implement`.
  - `.scratch/refresh-token-lifetime/issues/01-absolute-session-lifetime.md`: pierwsze kryterium każe ustalić
    wartość limitu przy triage. Gdy użytkownik ją poda, wpisz ją do ticketu i ustaw `ready-for-agent`; bez
    wartości `needs-info`.
  - `.scratch/user-by-email-enumeration/issues/01-foreign-email-returns-not-found.md`: (a) `ready-for-agent`
    z dopiskiem w ADR 0005, że wyszukiwanie po e-mailu jest wyjątkiem (404 zamiast 403), albo (b) `wontfix`,
    dopóki handler nie ma endpointu. Rekomendacja (a): zmiana to jedna linia i test, a bez niej pierwszy
    endpoint po e-mailu odziedziczy enumerację.
  - `.scratch/expected-failure-log-level/issues/01-expected-failures-not-logged-as-errors.md`: wymaga decyzji,
    jaki poziom logu dostaje który `ErrorType` (propozycja: `Failure` → Error, pozostałe → Warning). Decyzja
    dotyczy wszystkich handlerów, więc zapisz ją jako ADR 0007; potem `ready-for-agent`.
- **T3. Punkt 10 retro, część o testach HTTP:** czy test HTTP ścieżki porażki sprawdza kod błędu
  (`ProblemDetails.title`), a nie tylko status?
  - (a) Tak, gdy kryterium ticketu nazywa błąd; zapisać jako konwencję w `AGENTS.md` (Tests) i w `add-tests`.
    **Rekomendacja:** sam status bywa poprawny ze złego powodu (404 z routingu zamiast z handlera).
  - (b) Nie, kod sprawdzają testy unit.
  Druga część punktu 10 (metody domenowe `Archive`/`Unarchive`) dotyczy tylko gałęzi `pilot/archive-todo`
  i może poczekać.
- **T4. Publikacja template'u:** publiczne wydanie dopiero po sprawdzeniu licencji upstreamu przez użytkownika.
  Do tego czasu repo zostaje prywatne. Opcję „Template repository” w ustawieniach GitHuba zaznacza użytkownik.

## 5. Dług i ustalenia przeniesione

**Nie spłacaj ich tutaj bez decyzji.** Każda pozycja ma swój ticket albo właściciela.

- Cztery tickety z T2: każdy idzie osobnym `/implement` po decyzji z triage, nigdy przy okazji innego.
- Punkt 10 retro: część o metodach domenowych dotyczy gałęzi `pilot/archive-todo` (decyzja P1: dowód pilota,
  bez PR-a i bez scalania).
- Licencja i README przed publikacją (T4).
- `.agents/skills/`: nieśledzona kopia czterech skilli projektu z 2026-10-13, nieaktualna. Decyzja użytkownika:
  nie ruszać, nie commitować, edytować wyłącznie `.claude/skills/`.
- `dotnet-workflow` (`D:\Projects\dotnet-workflow`) zamrożony, a stary `D:\Projects\clean-architecture` jest jego
  fixture'em; oba bez zmian (decyzja F1 z Fazy 6).

## 6. Środowisko i pułapki, które kosztowały czas

- **Docker Desktop staje** między sesjami. Sprawdzaj `docker info --format '{{.ServerVersion}}'` osobną komendą;
  gdy pada, `Start-Process "C:\Program Files\Docker\Docker\Docker Desktop.exe"` i czekaj, aż odpowie.
- **Nieaktualne binaria po `git switch`:** zawsze `dotnet build` przed `dotnet test --no-build`.
- **Brak Pythona** (`python` to alias Sklepu i nic nie robi) i **brak `gh`**. Pliki edytuj narzędziem Edit.
- **GitHub:** wbudowana przeglądarka (`mcp__Claude_Browser__*`) ma sesję użytkownika, więc widzi prywatne repo.
  Logi kroków w Actions wymagają ponownego logowania; nie loguj się za użytkownika. Status przebiegów i PR-ów
  czytaj JavaScriptem z atrybutów `aria-label` (np. „completed successfully: Run 22 of Build. …”), bo
  `get_page_text` go nie pokazuje. Liczb testów z CI nie zobaczysz, więc licz je lokalnie.
- **`.agents/` psuje domyślny scope `ca-review`:** `git status --porcelain` nie jest pusty, więc skill bierze
  `git diff HEAD` zamiast diffu gałęzi. Wywołuj `ca-review` z `base <sha>` i z poleceniem wyłączenia `.agents/`.
- **`code-review` przed commitem** porównuje `<fixed-point>...HEAD`; obejście: `git add -N <nowe pliki>` i diff
  `git diff HEAD`.
- **Skille tylko dla użytkownika** (`/implement`, `/grill-with-docs`, `/to-spec`, `/to-tickets`, `/retro`) działają
  tylko jako osobna wiadomość zaczynająca się od ukośnika. Nie wywołasz ich narzędziem Skill.
- **Red proof przy analizatorach jako błędach:** złamanie musi się kompilować. Osłabiaj warunek
  (`&& x == Guid.Empty`) zamiast go usuwać; gdy osłabienie zostawia możliwy null, dodaj `!` (CS8602). Przed
  złamaniem zapisz SHA-256 pliku (`Get-FileHash`), po przywróceniu porównaj.
- **Dependabot:** domyślny limit 5 otwartych PR-ów; reszta przychodzi po scaleniu. Grupy `microsoft-dotnet`
  i `opentelemetry` w `.github/dependabot.yml`. PR-y zmieniające sąsiednie linie `Directory.Packages.props`
  konfliktują przy scalaniu; rozwiązanie to obie nowe wersje.
- **CI buduje w Release** (`.github/workflows/build.yml`), lokalnie zwykle Debug. Przed zmianą CI odtwórz ciąg
  Release lokalnie.
- **Host testów integracyjnych działa w środowisku Development** (domyślne dla `WebApplicationFactory`), więc
  Swagger jest tam wystawiony.

## 7. Workflow

1. Preflight i bramka T1–T4 (sekcja 9).
2. Zapisz decyzje z triage w ticketach: linia `**Status:**`, wartość limitu w tickecie refresh tokenu, notatka
   w `## Comments` z datą. Commit na `main`: `docs: triage of open tickets`. ADR 0007 tylko, jeśli bramka
   rozstrzygnęła poziomy logowania.
3. Dla wybranego ticketu: gałąź `fix/<slug>` od `main` (jak przy tickecie 403), potem zatrzymanie i prośba do
   użytkownika o `/implement <ścieżka ticketu>`. W `/implement`: `tdd` z seamami potwierdzonymi przez
   użytkownika, cykl od testu HTTP, red proof po jednym mechanizmie, a `code-review` **tylko na osi Spec**
   (reguła F4). Potem `ca-review` całej gałęzi z `base <sha>`.
4. Zamknięcie ticketu według `docs/agents/issue-tracker.md`: odhaczone kryteria, wynik w `## Comments`,
   `Status: done`. Do komentarza dopisz, czy reguła F4 zadziałała: co znalazła oś Spec i co dopiero `ca-review`.
5. Scalenie do `main` (fast-forward) i push tylko za zgodą użytkownika.
6. Nowe PR-y Dependabota przeglądaj tylko na prośbę użytkownika, procedurą z lekcji 1–4.

## 8. Lekcje metodyczne do powtórzenia

1. **Liczby testów tylko z własnego przebiegu.** CI na GitHubie pokazuje sam status, a przy podbiciu runnera
   testów zielony status może znaczyć zero wykrytych testów. Przy PR-ze z runnerem lub frameworkiem testów
   porównaj liczby z poprzednim przebiegiem.
2. **Sprawdzaj stan, który naprawdę wyląduje.** CI PR-a testuje go względem `main` z chwili otwarcia. Przy
   kilku PR-ach zależności scal je razem w worktree w scratchpadzie (`git worktree add --detach`) i tam zbuduj
   oraz przetestuj; potem `git worktree remove`.
3. **Przy zmianie wersji głównej przeczytaj informacje o wydaniu i zestaw je z użyciem w kodzie** (grep).
   NSubstitute 6 łamał API, którego nasze testy nie używają; wiadomo to dopiero po grepie.
4. **Zmianę bez testu zmierz sondą.** Swashbuckle 10.3.0 zmieniał generowanie OpenAPI; jednorazowy test
   w worktree (poza repo) wygenerował dokument na obu wersjach i porównał hashe.
5. **Red proof po jednym mechanizmie na przebieg**, z hashem pliku przed i po. W tickecie 403 dało to 11 czystych
   przebiegów bez fałszywych wyników.
6. **Kryterium niewidoczne przez HTTP testuj na funkcji tłumaczącej, po zgodzie użytkownika.** „`Failure` nadal
   daje 500” nie miało trasy, więc test wywołuje `CustomResults.Problem` wprost; wyjątek zapisany w tickecie.
7. **Nie wprowadzaj konwencji przy okazji.** W edycji skilla prawie weszła reguła o kodach błędów w testach
   HTTP, a to otwarty punkt 10 retro (T3). Konwencje idą przez bramkę.
8. **Twierdzenia review sprawdzaj w kodzie, zanim zadziałasz.** Uwagę o `LoggingDecorator` potwierdził odczyt
   pliku; dopiero wtedy powstał ticket, a nie poprawka „przy okazji”.

## 9. Pierwszy krok i miejsce zatrzymania

Preflight, w tej kolejności:

1. `git status`: `main`, czyste poza `.agents/`; `git fetch` i porównanie z `origin/main`.
2. `docker info` jako osobna komenda; w razie potrzeby uruchom Docker Desktop (sekcja 6).
3. `dotnet build CleanArchitecture.slnx`.
4. `dotnet test CleanArchitecture.slnx` z własnymi liczbami (oczekiwane 34/6/13).

Potem przeczytaj pliki z sekcji 3 i zadaj T1–T4 jednym formularzem. Po odpowiedziach zapisz triage w ticketach,
zacommituj, utwórz gałąź dla wybranego ticketu i **zatrzymaj się**: poproś użytkownika, żeby wpisał
`/implement <ścieżka ticketu>`.
