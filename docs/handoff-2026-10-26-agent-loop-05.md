> Nieaktualny. Wykonaj docs/handoff-2026-10-27-agent-loop-07.md

# Handoff: pętla bez nadzoru, ticket 05 (limity, wznawianie, pomiar)

## 1. Stan wejściowy

- **Template** `D:\Projects\clean-architecture_new`, lokalna gałąź `chore/agent-loop` (niewypchnięta). HEAD to
  commit tego handoffu na `1147d5f`. Tickety 01–04 z `.scratch/agent-loop/issues/` są `done`.
- **Ostatni własny pełny przebieg** (przy zamknięciu ticketu 04): build 0 błędów i 0 ostrzeżeń; Architecture 6,
  Application.UnitTests 54, IntegrationTests 14, HookTests 59, wszystkie zielone.
- **Nieśledzone:**
  - `.agents/` celowo zostaje do ticketu 09;
  - `.codex/` pojawił się 2026-10-26 o 20:41, nie z tej sesji. Patrz pytanie 6.
- **Fork skilli** `D:\Projects\skills`, gałąź `dotnet`, HEAD `e71cd59`, drzewo czyste. Plugin
  `mattpocock-skills` 1.3.1-dotnet.9 jest zainstalowany, więc nowa sesja widzi `/mattpocock-skills:ship`
  i role `mattpocock-skills:implementer`, `:verifier`, `:spec-reviewer`.
- **Sandbox** `D:\Projects\ship-sandbox`: klon z mini-przebiegu ticketu 03, ficzer `toy-overdue` skończony na
  gałęzi `feat/toy-overdue` w worktree `.claude/worktrees/toy-overdue`. Folder jest zaufany w aplikacji.

## 2. Co robimy dalej

Ticket **05: Limity, wznawianie i pomiar**, plik `.scratch/agent-loop/issues/05-quota-resume-and-metrics.md`
(istnieje, `ready-for-agent`, seam: żywy mini-przebieg z progiem ustawionym tuż nad bieżącym zużyciem).
Potem w kolejności zależności: 06, 07, 08, 09, a na końcu pilot 10 i rozpisanie etapu 2 (11), oba dla
użytkownika. Każdy ticket robi się przez `/implement <ścieżka ticketu>`, a tę komendę wpisuje użytkownik.

## 3. Co przeczytać na start

- **`.scratch/agent-loop/issues/05-quota-resume-and-metrics.md`:** kryteria i seam.
- **`.scratch/agent-loop/spec.md`:** sekcje „Limity i wznawianie”, „Pomiar”, „Role i modele”,
  „Uprawnienia”.
- **`## Comments` ticketów 03 i 04:**
  - wynik mini-przebiegu (czas, zużycie limitu, ustawienia sesji w tle);
  - punkty „Do 05” i „Do pilota (10)”;
  - dlaczego `claude -p` odpada.
- **`D:\Projects\skills\skills\engineering\ship\SKILL.md`:**
  - sekcja „Work the tickets”, bo tam przed każdym ticketem wchodzi sprawdzenie limitu;
  - „Start or resume”, bo stan i wznowienie są już opisane.
- **`docs/agents/dod.json`:** progi `quota` (`weeklyPercent` 80, `fiveHourPercent` 90), czytane przez
  `/ship`.
- **`.claude/hooks/run-dod.ps1` i `dod-common.ps1`:** wzorzec skryptu z testami na seamie procesu
  (`tests/HookTests`). Użyj go, jeśli część ticketu da się zrobić deterministycznie.

## 4. Otwarte pytania na bramkę

Rozstrzygnij je z użytkownikiem jedną rundą, zanim zaczniesz implementację.

1. **Mechanizm wznowienia po resecie limitu.** `claude -p` odpada: samodzielne CLI nie jest zalogowane
   (`claude auth status` daje `loggedIn: false`), bo aplikacja desktopowa ma własne logowanie. Warianty:
   - (a) zadanie zaplanowane aplikacji (narzędzia `mcp__scheduled-tasks__*`), które o godzinie resetu otwiera
     nową sesję z `/mattpocock-skills:ship <ficzer>` w worktree ficzera, na Sonnecie, w trybie auto;
   - (b) `ScheduleWakeup` albo `CronCreate` w tej samej sesji `/ship`; działa tylko, gdy sesja żyje;
   - (c) Harmonogram zadań Windows plus `claude -p`; wymaga zalogowania CLI przez użytkownika.

   Rekomendacja: (a). Najpierw sprawdź fakty: czy zadanie zaplanowane umie uruchomić komendę skilla, w jakim
   folderze, z jakim modelem i trybem uprawnień, i czy działa przy zamkniętym oknie aplikacji.
2. **Odczyt zużycia limitu.** `mcp__ccd_session_mgmt__get_usage` zwraca procent okna 5-godzinnego
   i tygodniowego oraz godzinę resetu. Sprawdź, czy jest dostępne w sesji `/ship` uruchomionej w tle.
   Rekomendacja: używać go. Bez narzędzia `/ship` nie pauzuje, a tabela pomiaru dostaje „brak danych”.
3. **Effort.** Sesja mini-przebiegu odziedziczyła effort `xhigh`. Pole `effort` istnieje we frontmatterze
   agenta i skilla. Rekomendacja: implementer `medium`, weryfikator `low`, orkiestrator `medium`. Pilot
   (ticket 10) zmierzy, czy jakość nie spada.
4. **Model sesji `/ship`.** `model:` w skillu obowiązuje tylko w pierwszej turze, a każde powiadomienie
   o końcu pracy sub-agenta zaczyna nową turę na modelu sesji. Rekomendacja: sesję `/ship` zawsze uruchamia
   się na Sonnecie (zadanie zaplanowane z wariantu 1a też), a przewodnik (ticket 09) to opisze.
5. **Tabela pomiaru.** Ticket mówi „tabela w katalogu ficzera”. Rekomendacja: `.scratch/<ficzer>/metrics.md`,
   jeden wiersz na ticket (zużycie okna 5-godzinnego i tygodniowego na starcie i na końcu, rundy, model
   w każdej roli, czas, wynik) oraz wiersz sumy po ostatnim tickecie. Commit razem z `ship: <NN> done`.
6. **Nieśledzony `.codex/`** (nie z tej sesji): kopia `.claude/hooks/*.ps1` i `hooks.json` z hookami
   `PreToolUse` i `SubagentStop` dla Codexa, ze ścieżkami absolutnymi do głównego checkoutu. W worktree ficzera
   wskazywałaby skrypty z głównego checkoutu, a przy każdej zmianie hooków będzie nieaktualna, tak jak
   `.agents/skills/`. Zapytaj użytkownika, skąd jest i czy ma zostać. Rekomendacja: nie commitować; jeśli
   Codex ma być wykonawcą (krok 3 z grillowania, odłożony do pomiarów z pilotów), kopię ma generować skrypt
   ze ścieżkami względnymi, a nie ręczne kopiowanie.

## 5. Dług i ustalenia przeniesione

Nie spłacaj ich tutaj bez decyzji użytkownika.

- **Do ticketu 06 i 07:** brief gotowości, push „gotowe do bramki 2”, scalenie po „OK”, propozycje
  konwencji.
- **Do ticketu 08:** `/plan-feature` jest tylko dla człowieka, a taki skill nie może wołać innych skilli tylko
  dla człowieka (`grill-with-docs`, `to-spec`, `to-tickets`). Musi używać skilli wywoływanych przez model
  (`grilling`, `domain-modeling`) i mieć własne kroki speca i ticketów (reguła forka: `.agents/invocation.md`
  w `D:\Projects\skills`).
- **Do ticketu 09:** przewodnik `docs/workflow.md` (sesja `/ship` na Sonnecie, jednorazowe zaufanie folderu,
  `claude -p` nieużywalne), usunięcie `.agents/skills/`, sekcja procesu w `AGENTS.md`.
- **Do ticketu 10 (pilot), niesprawdzone na żywo:**
  - push przy awarii środowiska;
  - ścieżka `stuck` i `needs-info` z briefem;
  - rundy weryfikatora i recenzenta;
  - push przy pytaniu o zgodę;
  - czy powiadomienie o końcu pracy sub-agenta przychodzi przed werdyktem blokady DoD.
- **Do ticketu 11:** `tests/ArchitectureTests/**` jest chronione, więc regułę „`Mobile.Core` nie zależy od
  MAUI” doda człowiek albo orkiestrator, nie implementer.
- **Bez testu:** ścieżka przekroczenia czasu mutexu w `Invoke-DodExclusive` (30 minut).
- **CI na Ubuntu nie uruchomione dla `tests/HookTests`.** Gałąź nie jest wypchnięta, a workflow rusza tylko
  na `main` i PR. Sprawdzi się przy scaleniu albo `workflow_dispatch`.

## 6. Środowisko i pułapki, które kosztowały czas

- **`pwsh` jako narzędzie dotnet.** `pwsh` 7.6 jest zainstalowany jako narzędzie dotnet. W skrypcie
  `(Get-Process -Id $PID).Path` wskazuje `dotnet.exe`, więc ponowne uruchomienie „bieżącej powłoki” po ścieżce
  pada. Wołaj `pwsh` po nazwie z `PATH`.
- **Nazwy zmiennych w PowerShellu nie rozróżniają wielkości liter.** Pętla `foreach ($check in …)`
  nadpisała parametr `-Check` z `ValidateSet` i skrypt padał.
- **`--no-build` za `grep` w Git Bash.** `dotnet build … | grep … && dotnet test --no-build` puszcza testy
  na starych binariach, bo `grep` zwraca sukces. Najpierw `set -o pipefail`.
- **Docker Desktop staje.** `run-dod.ps1 -Check environment` sam go uruchamia (pole `start` w
  `docs/agents/dod.json`) i czeka.
- **Fork skilli** (`D:\Projects\skills`, gałąź `dotnet`):
  - po każdej zmianie podbij wersję w `.claude-plugin/plugin.json`, potem
    `claude plugin marketplace update mattpocock` i `claude plugin update mattpocock-skills@mattpocock`;
  - bieżąca sesja nie widzi nowej wersji, nowa sesja tak;
  - zasady forka (`CLAUDE.md` w forku): bez pauz (em-dash) w prozie; promowany skill wymaga wpisu w README
    i README kubełka, `plugin.json`, `agents/openai.yaml` i strony `docs/<kubełek>/<skill>.md`;
  - `claude plugin validate .` ostrzega o `CLAUDE.md` w korzeniu; to stare ostrzeżenie, nie nasze.
- **Sesje w tle aplikacji desktopowej:**
  - folder musi być raz zaufany (użytkownik otwiera go przez New session);
  - aplikacja uruchamia sub-agentów w tle;
  - start mini-przebiegu: `start_session` z `initiation: user_asked`, `cwd` sandboksa, `permission_mode: auto`,
    modelem Sonnet i promptem `/mattpocock-skills:ship <ficzer>`.
- **Sandbox `D:\Projects\ship-sandbox`** to lokalny klon gałęzi `chore/agent-loop` z commita `bd1fbf1`.
  Przed kolejnym mini-przebiegiem zrób w nim `git pull`, bo bez tego nie ma mutexu, walidacji DoD ani
  sprawdzenia solucji, i dodaj nowe tickety zabawkowe (ficzer `toy-overdue` jest skończony).
- **Limit Pro.** Mini-przebieg z dwoma ticketami zużył ok. 5% tygodnia. Długie sesje na Opusie z dużym
  kontekstem zużywają okno 5-godzinne szybko; sprawdzaj `get_usage` przed żywym przebiegiem.

## 7. Workflow

Bez zmian względem ticketów 01–04, czyli `/implement` na ticket:

1. `tdd` na seamie z ticketu. Seamy są zatwierdzone, więc nie pytaj o nie.
2. Testy, które nie były czerwone w cyklu, dostają red proof.
3. Pełny build i testy łańcuchem: `set -o pipefail`, potem
   `dotnet build CleanArchitecture.slnx && dotnet test CleanArchitecture.slnx --no-build`.
4. Review na osi Spec: sub-agent na Sonnecie, z diffem obu repozytoriów i ticketem.
5. Poprawki uwag, najpierw czerwony test.
6. Zamknięcie ticketu: odhaczone kryteria, `## Comments`, `**Status:** done`.
7. Commity w obu repozytoriach i aktualizacja pluginu.

Żywy mini-przebieg ticketu 05 uruchamiasz jako sesję w tle aplikacji w sandboksie, tak jak w 03 (szczegóły:
sekcja 6). Wznawianie sprawdzasz, ustawiając w `dod.json` sandboksa próg tuż nad bieżącym zużyciem. Nie
zmieniaj progów w template'cie.

Push i scalenie do `main` należą do użytkownika, a gałąź `chore/agent-loop` scala się dopiero po pilocie.

## 8. Lekcje metodyczne do powtórzenia

1. **Review Spec na Sonnecie w każdym tickecie** znalazł realne dziury: 6 w 02, 11 w 03, 11 w 04. Każ mu
   sprawdzać konkretne wejścia na repozytorium tymczasowym, a nie tylko czytać diff. Bez tego te błędy
   trafiłyby do pilota.
2. **Oczekiwany tekst w teście nie może występować w komendzie, którą skrypt cytuje.** Inaczej test przechodzi
   przez samo zacytowanie, nawet gdy mechanizm nie działa. Buduj go przy wykonaniu (`'a ' + 'b'`). Tak ukrył
   się błąd z `dotnet.exe`.
3. **Red proof psuje jeden mechanizm, sprawdza hash przed i po i filtruje po nazwie metody.** Filtr
   `FullyQualifiedName~` nie zawiera argumentów Theory, więc filtr z argumentem nie trafia w żaden test
   i wygląda jak zielony.
4. **Żywy przebieg znajduje to, czego testy nie widzą.** Mini-przebieg w 03 ujawnił wyścig blokady DoD
   z orkiestratorem o zablokowane DLL-e i sub-agentów w tle. Testy hooków tego nie mogły pokazać.
5. **Raport sesji albo sub-agenta to nie dowód.** Po mini-przebiegu sprawdź sam commity, `## Comments`
   ticketów i `check-diff.ps1` na zakresach.
6. **Najpierw tekst skilla, potem bump wersji i aktualizacja pluginu, na końcu commit.** Inaczej żywy
   przebieg w nowej sesji używa starej wersji z cache pluginu.

## 9. Pierwszy krok i miejsce zatrzymania

1. Przeczytaj pliki z sekcji 3 i sprawdź fakty z pytań 1 i 2 (narzędzia zadań zaplanowanych,
   `get_usage` w sesji w tle). Szukaj ich przez `ToolSearch`, bo bywają odroczone.
2. Przedstaw użytkownikowi pytania z sekcji 4 jako jedną rundę: numerowane, z rekomendacją, sformułowane tak,
   żeby „tak” przyjmowało rekomendację.
3. Decyzje zapisz w `## Comments` ticketu 05 jako „Decyzje przed implementacją” i poproś użytkownika
   o wpisanie `/implement .scratch/agent-loop/issues/05-quota-resume-and-metrics.md`.
4. Zatrzymaj się po zamknięciu ticketu 05 (commity w obu repozytoriach, plugin zaktualizowany). Nie zaczynaj
   ticketu 06 w tej samej sesji, jeśli kontekst przekroczył połowę okna albo limit 5-godzinny przekroczył 70%.
