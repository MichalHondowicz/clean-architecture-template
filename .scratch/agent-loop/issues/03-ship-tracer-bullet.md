# 03: Pierwsza wersja `/ship`: tickety bez nadzoru

**What to build:** Wpisuję `/ship <feature>` dla ficzera z ticketami `ready-for-agent` i odchodzę. Pętla
sprawdza środowisko, tworzy worktree i gałąź ficzera, bierze tickety po kolei z frontiera, dla każdego
uruchamia implementera z kontraktem i oznacza ticket `done` z wpisem w `## Comments`. Ponowne `/ship` po
przerwaniu wznawia od tego samego miejsca. Ta wersja nie ma jeszcze rund review (04) ani limitów (05).
Dotyczy obu repozytoriów: fork dostaje skill `/ship` (tylko dla człowieka), rolę implementera, zmianę
w `tdd` i format ticketu w `to-tickets`. Template dostaje kontrakt implementera, status `in-progress`, opis
formatu ticketu w dokumentacji trackera i allowlistę.

**Blocked by:** 01

**Seams:** żywy mini-przebieg: `claude -p --plugin-dir` z forkiem na ficzerze-zabawce (1–2 trywialne tickety)
w tymczasowym worktree. Obserwacja: statusy ticketów, `## Comments`, historia gałęzi i liczba pytań o zgodę.

**Status:** done

- [x] `/ship <feature>` jest tylko dla człowieka i działa na Sonnecie (historia 8).
- [x] Pre-flight: Docker działa albo `/ship` uruchamia Docker Desktop i czeka na niego; komputer dostaje
      czuwanie; worktree jest czysty. Niepowodzenie Dockera lub buildu bazowego zatrzymuje ficzer i wysyła
      push „awaria środowiska” z jednym zdaniem (historie 10, 11, 31, 43).
- [x] Worktree i gałąź `feat/<slug>` per ficzer; główny checkout zostaje nietknięty (historia 9).
- [x] Tickety idą po kolei, w kolejności zależności, bez równoległości i bez agenta scalającego. Ticket
      w pracy ma status `in-progress`, opisany w etykietach trackera obok `done` (historia 14).
- [x] Start i wznowienie to ta sama operacja: przerwany przebieg po ponownym `/ship` kontynuuje od ticketu
      `in-progress` i nie powtarza ticketów `done` (historia 13).
- [x] Rola implementera z forka (Sonnet, `maxTurns`) dostaje wejście z kontraktu. Jej nazwa jest zgodna
      z plikiem DoD, więc hooki z 01 i 02 działają w jej sesji (historie 15, 16, 17).
- [x] `tdd` traktuje seamy z sekcji `Seams:` zatwierdzonego ticketu jako uzgodnione i nie pyta o nie. Bez
      ticketu zachowuje się jak dziś (historie 3, 15).
- [x] Kontrakt implementera spisany w dokumentacji agentów template'u. Wejście: ticket, worktree, gałąź,
      numer rundy, plik uwag. Wyjście: commity, wpis w `## Comments` z nazwą testu i dowodem czerwieni dla
      każdego `ACn`, status `done` albo `stuck`. Zakazy: chronione ścieżki z pliku DoD (historie 17, 20).
- [x] Format ticketu (sekcja `Seams:`, kryteria `AC1…n` z planowaną nazwą testu) opisany w dokumentacji
      trackera i w szablonie `to-tickets` (historia 4).
- [x] Allowlista projektu obejmuje `git worktree`, `git switch`, `git merge --ff-only` i uruchomienie Docker
      Desktop. Sesja pętli działa w trybie auto, nigdy w `bypassPermissions` (historia 12).
- [x] Żywy mini-przebieg: ficzer-zabawka dochodzi do `done` bez żadnego pytania o zgodę. Wynik (statusy,
      czas, zużycie limitu, ewentualne pytania) zapisany w `## Comments` tego ticketu.
- [x] `implement` i `implement-spec` w forku są bez zmian (historia 62).
- [x] Wersja forka podbita, plugin zaktualizowany.

## Comments

**2026-10-26, `/implement`:**

- **Fork** (`D:\Projects\skills`, gałąź `dotnet`): `62167d3`, `821b835`, `c3e73cb`, `c89dda4`; plugin
  `1.3.1-dotnet.7` zainstalowany.
  - `skills/engineering/ship`: user-only, `model: sonnet`.
  - `agents/implementer.md`: Sonnet, `maxTurns: 150`, bez narzędzia `Agent`.
  - Zmiany w `tdd` (seamy z ticketu, także nagłówek `## Seams`) i w `to-tickets` (`Seams:` oraz
    `AC1…n` z planowanym testem).
  - README, README kubełka, router `ask-matt`, strona `docs/engineering/ship.md`, resync stron `tdd`
    i `to-tickets`.
  - `implement` i `implement-spec` bez zmian.
- **Template:**
  - `run-dod.ps1` (pre-flight: sprawdzenia środowiska ze `start` i czekaniem; komendy DoD po kolei;
    walidacja kształtu pliku DoD z exit 2);
  - `docs/agents/implementer-contract.md`, status `in-progress`, format ticketu w `issue-tracker.md`;
  - `start` Dockera w `dod.json`, `.claude/worktrees/` w `.gitignore`;
  - allowlista: `git worktree`, `git switch`, `git merge --ff-only` i dokładna komenda `run-dod.ps1`.
  - Uruchomienia Docker Desktop nie ma na allowliście osobno: robi je `run-dod.ps1` jako proces potomny,
    więc wystarcza wpis na ten skrypt.
- **Zmiany w hookach z 01 i 02:**
  - blokada DoD przepuszcza `STATUS: stuck`, bez tego implementer kręciłby się do `maxTurns`; kontrakt to
    opisuje;
  - DoD wykonuje się na wyłączność (nazwany mutex na katalog repozytorium) w blokadzie i w `run-dod.ps1`.
- **Testy:** build 0/0; Architecture 6, Unit 54, Integration 14, Hook 58, wszystkie zielone. Red w cyklu:
  start środowiska, `stuck`, wyłączność DoD, niekompletny plik DoD (4 przypadki). Red proof dla pozostałych
  testów `run-dod` (hashe zgodne): awaria środowiska, zatrzymanie na pierwszej porażce, zielone komendy,
  niepoprawny JSON.
- **Żywy mini-przebieg**, sesja w tle aplikacji desktopowej, tryb auto, Sonnet:
  - sandbox `D:\Projects\ship-sandbox`, ficzer `toy-overdue`;
  - 01 `done` od początku, 02 `in-progress` (symulacja przerwania), 03 zależny od 02;
  - 12:16–12:27 UTC, ok. 11 min.
  - Przebieg: keep-awake, `EnterWorktree` do `.claude/worktrees/toy-overdue` na `feat/toy-overdue`,
    środowisko i baseline zielone. Ticket 02 wznowiony bez nowego commita startowego (`cfdb7a3`
    implementer, `6b26032` done), potem 03 (`b7c8de4` start, `a293bf5` implementer, `7804a73` done).
  - Każde `ACn` ma test i dowód czerwieni: 3 czerwone w cyklu, 2 red proofy z hashem.
  - `check-diff` na obu zakresach: 0 naruszeń; w `testsToReview` jest klasa testów z 02, do której 03
    dopisał metody.
  - Żadne pytanie o zgodę nie zatrzymało sesji.
  - Zużycie limitu (razem z moimi turami): okno 5-godzinne 0% → 20% po resecie, tydzień 11% → 16%.
  - Sesja odziedziczyła effort `xhigh`. Do rozważenia w 05: niższy `effort` orkiestratora i implementera.
- **Odstępstwo od seamu ticketu:** zamiast `claude -p` była sesja desktopowa, z wyboru użytkownika.
  `claude -p` padł po 5 s: samodzielne CLI nie jest zalogowane, a folder sandboksa nie był zaufany.
  Wniosek dla 05: wznawianie nie może polegać na `claude -p`.
- **Co mini-przebieg wykrył (poprawione):**
  1. Sprawdzenie DoD orkiestratora ścigało się z blokadą DoD implementera (zablokowane DLL-e, fałszywa
     czerwień). Poprawka: mutex.
  2. Aplikacja uruchamia sub-agentów w tle. Skill każe teraz czekać na raport, a nie wołać „w foreground”.
- **Review Spec (Sonnet), poprawione:**
  - plik DoD `{}` dawał zielony pre-flight, teraz walidacja kształtu;
  - mutex po przekroczeniu czasu zwalniał nieposiadaną blokadę, teraz jawny błąd (bez testu: ścieżka
    30-minutowa);
  - `git revert` na pustym zakresie;
  - najnowszy commit `ship: start`;
  - resztki w worktree przy wznowieniu;
  - nazwane narzędzie powiadomień;
  - tryb auto zapisany w skillu;
  - `## Seams` w `tdd`;
  - kontrakt a `stuck`.
- **Świadomie ponad ticket:** ścieżka `stuck` (stash, `git revert`, `needs-info`, zależne czekają). Bez niej
  pętla nie miałaby zdefiniowanego zachowania. Rundy poprawek, weryfikator i push przy `needs-info` zostają
  w 04.
- **Niesprawdzone na żywo:** push przy awarii środowiska i ścieżka `stuck` (środowisko było zielone,
  tickety zbiegły). Do pilota (10, scenariusze b i d).
- **Do 04:** weryfikator uruchamia `check-diff.ps1` (lista z `Base`) i ocenia `testsToReview`. Otwarte
  pytanie: czy powiadomienie o końcu pracy sub-agenta przychodzi przed werdyktem blokady DoD; skill radzi
  sobie czekaniem na kolejny raport, a pilot to potwierdzi.
- **Do 05:** sesja `/ship` musi działać na Sonnecie, bo `model:` skilla obowiązuje tylko w pierwszej turze,
  a każde powiadomienie o sub-agencie to nowa tura na modelu sesji. Opisać w przewodniku (09).
- Sandbox `D:\Projects\ship-sandbox` i jego sesja zostały do wglądu; można je usunąć.
