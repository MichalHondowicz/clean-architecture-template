> Nieaktualny. Wykonaj docs/handoff-2026-10-28-agent-loop-08.md

# Handoff: pętla bez nadzoru, ticket 07 (bramka 2: scalenie po „OK”)

## 1. Stan wejściowy

- **Template** `D:\Projects\clean-architecture_new`:
  - lokalna gałąź `chore/agent-loop`, niewypchnięta; HEAD to commit tego handoffu na `52c784e`;
  - drzewo czyste poza `.agents/` i `.codex/`, które celowo zostają nieśledzone.
- **Tickety** w `.scratch/agent-loop/issues/`:
  - 01–06: `done`;
  - 07, 08, 09: `ready-for-agent`;
  - 10, 11: `ready-for-human`;
  - 12: `needs-triage`.
- **Ostatni własny pełny przebieg**, po implementacji ticketu 06: build bez błędów i ostrzeżeń, testy zielone
  (ArchitectureTests 6, Application.UnitTests 54, IntegrationTests 14, HookTests 83). Od tamtej pory w
  template'cie zmieniały się tylko dokumenty w `.scratch/`.
- **Fork skilli** `D:\Projects\skills`, gałąź `dotnet`, HEAD `52969e4`, drzewo czyste. Plugin
  `mattpocock-skills` 1.3.1-dotnet.15 jest zainstalowany.
- **Sandbox** `D:\Projects\ship-sandbox`:
  - checkout główny na `chore/agent-loop` (`53439c4`), lokalny `main` na tym samym commicie;
  - worktree `toy-brief` (gałąź `feat/toy-brief`, `d643a8c`) stoi na bramce 2. Ma gotowy brief z dwiema
    propozycjami konwencji, więc nadaje się na żywy przebieg ticketu 07;
  - worktree `toy-overdue` i `toy-quota` są skończone i można je usunąć;
  - `origin` sandboksa to repozytorium template'u. Patrz pytanie 8.
- **Ustawienia:**
  - aplikacja: „Connect new sessions to Remote Control” i keep-awake są włączone;
  - samodzielne CLI jest zalogowane kontem claude.ai;
  - w `/config` oba przełączniki pushy są włączone.
- **Limit przy handoffie:** okno 5-godzinne 9% (reset 27.10 o 21:30), tydzień 40% (reset 30.10 o 07:00).

## 2. Co robimy dalej

Ticket **07: Bramka 2: scalenie po „OK”**, plik `.scratch/agent-loop/issues/07-merge-gate.md`. Ticket istnieje,
ma status `ready-for-agent`, a jego seam to pilot (ticket 10). Najpierw bramka w sesji: pytania z sekcji 4
w jednej rundzie. Potem użytkownik wpisuje `/mattpocock-skills:implement <ścieżka ticketu>`.

Kolejne tickety po 07: 08 (`/plan-feature`) i 09 (porządki i przewodnik). Na końcu pilot 10 i rozpisanie
etapu 2 (11), oba należą do użytkownika. Ticket 12 czeka na triage użytkownika i niczego nie blokuje.

## 3. Co przeczytać na start

- **`.scratch/agent-loop/issues/07-merge-gate.md`:** kryteria.
- **`.scratch/agent-loop/spec.md`:**
  - historie 34–39: brief, scalenie jedną wiadomością, fast-forward i CI, allowlista, propozycje konwencji;
  - decyzja „Bramka 2” (wyszukaj frazę „Bramka 2:”).
- **`## Comments` ticketu 06:** decyzje z bramki 06, przebieg C i dopisek o dostarczaniu pusha.
- **`D:\Projects\skills\skills\engineering\ship\SKILL.md`:**
  - sekcja „Branch review”, pozycja 1 listy wznowienia: przy istniejącym `brief.md` przebieg kończy się bez pusha.
    Ticket 07 zmienia to miejsce;
  - „End of the run” oraz „Never”: zakaz pytania i czekania, pushowania, scalania, usuwania gałęzi i worktree,
    edycji ścieżek chronionych.
- **`D:\Projects\skills\agents\standards-reviewer.md`:** format propozycji, czyli reguła, uzasadnienie i ticket.
  Nie ma tam pliku docelowego. Patrz pytanie 3.
- **`docs/agents/dod.json`:** `protectedPaths` obejmuje m.in. `AGENTS.md`, `CLAUDE.md`, `docs/agents/**`
  i `.claude/**`.
- **`.claude/settings.json`:** allowlista ma `Bash(git merge --ff-only *)` i `Bash(git worktree *)`, ale nie ma
  żadnego `git push`.
- **`D:\Projects\ship-sandbox\.claude\worktrees\toy-brief\.scratch\toy-brief\brief.md`:** prawdziwy brief
  z bramki 2, jako materiał do pytania 2.

## 4. Otwarte pytania na bramkę

Rozstrzygnij je z użytkownikiem w jednej rundzie, zanim zaczniesz implementację. Fakty oznaczone „sprawdź”
sprawdź przed rundą: na repozytorium tymczasowym albo w sandboksie, nie z pamięci.

1. **Czekanie na odpowiedź po briefie.** Dziś `/ship` po briefie kończy przebieg, a lista „Never” zabrania pytać
   i czekać. Warianty:
   - (a) po briefie sesja kończy turę jednym pytaniem. Odpowiedź w tej samej sesji, z komputera albo przez Remote
     Control, wznawia ją w nowej sekcji skilla „Gate 2”. Ponowny `/ship` przy briefie bez scalenia zadaje
     pytanie jeszcze raz, zamiast kończyć. „Never” dostaje wyjątek tylko dla tego pytania;
   - (b) osobna komenda w nowej sesji.

   Rekomendacja: (a), bo tak mówią historia 35 i spec. Sprawdź z użytkownikiem, czy sesję w tle widać w zakładce
   Code na telefonie i czy da się z niej odpisać. Przy testach pushy tego nie potwierdził.
2. **Format odpowiedzi.** Rekomendacja:
   - brief kończy się blokiem pytania: scalenie `OK` albo `nie`, a każda propozycja `<n>: tak` albo `<n>: nie`;
   - odpowiedź to jedna wiadomość, np. `OK, 1 tak, 2 nie`;
   - scala tylko jednoznaczne „OK”. Pytanie, uwaga albo coś niejasnego nie wysyła nic do `main`: sesja odpowiada
     i czeka dalej;
   - propozycja bez decyzji wypada i brief mówi o tym wprost.

   Wariant: propozycję bez decyzji dopytać przed scaleniem.
3. **Przyjęte propozycje a ścieżki chronione.** Reguły trafiają do `AGENTS.md` albo do checklisty
   `.claude/skills/ca-review/SKILL.md`. Obie ścieżki są w `protectedPaths`, a „Never” zabrania orkiestratorowi
   je edytować. Hook `guard-protected-paths.ps1` blokuje tylko implementera. Warianty:
   - (a) wyjątek dla orkiestratora: po decyzji użytkownika wpisuje przyjętą regułę dosłownie z briefu, osobnym
     commitem na gałęzi ficzera przed scaleniem (historia 39);
   - (b) orkiestrator tylko przygotowuje zmianę, a commit robi użytkownik.

   Rekomendacja: (a). Do tego `standards-reviewer` w każdej propozycji podaje plik docelowy, bo dziś go nie ma.
   `CLAUDE.md` mówi, że reguły projektu żyją w `AGENTS.md`.
4. **Fast-forward.** Fakty:
   - checkout główny template'u stoi na `chore/agent-loop`, nie na `main`;
   - sesja `/ship` siedzi w worktree ficzera;
   - sesja izolowana w worktree odmawia poleceń `git` skierowanych poza swój worktree.

   Warianty:
   - (a) z worktree ficzera: `git fetch origin`, sprawdzenie, czy `origin/main` jest przodkiem gałęzi, potem
     `git fetch . feat/<ficzer>:main` (aktualizuje lokalny `main` tylko przy fast-forward, bez przełączania
     checkoutu) i `git push origin main`;
   - (b) `git switch main` i `git merge --ff-only` w checkoucie użytkownika, co rusza jego drzewo.

   Rekomendacja: (a). Gdy `main` się przesunął: stop i raport, bez rebase'u. Sprawdź na repozytorium
   tymczasowym, co robi `git fetch . x:main`, gdy `main` jest wyciągnięty w innym checkoucie.
5. **Sprzątanie.** `ExitWorktree` z `remove` usuwa tylko worktree utworzony przez `EnterWorktree` w tej samej
   sesji. `/ship` zakłada worktree przez `git worktree add` i wchodzi do niego przez `EnterWorktree` z `path`,
   więc `remove` najpewniej nic nie zrobi. Rekomendacja:
   - `ExitWorktree` z `keep`;
   - potem w checkoucie głównym `git worktree remove .claude/worktrees/<ficzer>` i `git branch -d feat/<ficzer>`
     (`-d` odmawia dla gałęzi niescalonej);
   - wyjątek w „Never” tylko po „OK” i udanym pushu.

   Sprawdź w sandboksie.
6. **CI.** Fakty z pamięci projektu: repozytoria są prywatne, `gh` nie ma, a przeglądarka aplikacji ma sesję
   GitHuba, więc status CI da się odczytać, a logów nie. Warianty:
   - (a) wbudowana przeglądarka: strona commita albo Actions czytana przez `get_page_text`, ponowny odczyt co
     2 minuty (czekanie w tle przez PowerShell `Start-Sleep`), najwyżej 20 minut;
   - (b) tylko link w raporcie.

   Rekomendacja: (a), a przy czerwonym CI raport i push, bez revertu. Sprawdź, czy sesja w tle może używać
   wbudowanej przeglądarki bez zgody klikniętej przez użytkownika.
7. **Allowlista.** Dopisać dokładnie `Bash(git push origin main)` i `PowerShell(git push origin main)`, bez
   gwiazdki (historia 37), oraz to, czego wymaga wariant z pytania 4 (`git fetch`). Tryb auto blokuje edycję
   `.claude/settings.json`, więc linie dopisuje użytkownik, jak w tickecie 05. Sprawdź, czy klasyfikator trybu
   auto przepuszcza push do `main` przy tej regule.
8. **Żywy przebieg.** `origin` sandboksa to `D:/Projects/clean-architecture_new`, więc `git push origin main`
   z sandboksa przesunąłby lokalny `main` template'u. Rekomendacja:
   - przed przebiegiem utworzyć goły klon `D:\Projects\ship-sandbox-origin.git` i przestawić na niego `origin`
     sandboksa;
   - ficzer `toy-brief` stoi na bramce 2 z dwiema propozycjami. „OK” wpisuje użytkownik, najlepiej z telefonu
     (historia 35);
   - krok CI nie ma GitHuba w sandboksie: raport „brak CI dla tego remote”, a CI sprawdza pilot 10.

## 5. Dług i ustalenia przeniesione

Nie spłacaj ich tutaj bez decyzji użytkownika.

- **Ticket 12** (fallback powiadomień przez ntfy i hook `Notification`): `needs-triage`, pięć pytań w pliku.
  Push „gotowe do bramki 2” dziś nie dociera: cztery testy dały „Mobile push requested” bez dostarczenia.
- **Plugin `.15`:** verifier uruchamia skrypty narzędziem PowerShell. Tego nie sprawdził żaden przebieg, a bramka 2
  nie uruchamia verifiera, więc potwierdzi to dopiero pilot 10.
- **Do pilota (10), niesprawdzone na żywo:** rundy 2 i 3 gałęzi, uwagi otwarte w briefie, przywracanie kodu przez
  `git restore`, pauza limitu przed review gałęzi, push przy pytaniu o zgodę (teraz ticket 12) i krok CI.
- **Do ticketu 09 (przewodnik):**
  - ustawienie Remote Control dla nowych sesji;
  - przełączniki pushy w `/config`, widoczne dopiero w CLI zalogowanym przez `/login`;
  - zawodność pushy;
  - sesja `/ship` na Sonnecie z effortem `medium`.
- **Znane ograniczenie:** wznowiony ticket `in-progress` albo runda gałęzi w toku dostaje baseline na swoim
  bieżącym drzewie.
- **Sprzątanie na życzenie użytkownika:**
  - worktree `toy-quota` i `toy-overdue` w sandboksie;
  - zarchiwizowanie sesji przebiegów A, B, B2 i C oraz sesji testu pushy.

## 6. Środowisko i pułapki, które kosztowały czas

- **Sesja izolowana w worktree:**
  - odmawia złożonych poleceń z `git` (`cd`, `;`, `&&`, potok, `$(...)`);
  - jej narzędzie Bash odmawia **każdego** polecenia z `pwsh`, także prostego;
  - skrypty uruchamia się narzędziem PowerShell, więc subagent bez tego narzędzia ich nie uruchomi. Poprawka
    po przebiegu B2 („proste polecenia”) tego nie usunęła i wyszło to dopiero w przebiegu C.
- **Pushe:**
  - przy aktywnej aplikacji narzędzie pomija push („this terminal is active”);
  - po 3 minutach czekania i zablokowanym ekranie odpowiada „Mobile push requested”, ale nic nie dochodzi, także
    po wyłączeniu i włączeniu Remote Control;
  - przełączniki w `/config` to `agentPushNotifEnabled` i `inputNeededNotifEnabled` w `~/.claude/settings.json`.
- **Długie wpisy po polsku:** dopisuj przez Edit albo Write, nie przez heredoc w Bash. Heredoc z długim tekstem
  padł z błędem „unexpected EOF while looking for matching `''” i nic nie zapisał.
- **Plugin w trakcie przebiegu:** sesja w tle używa kopii pluginu z cache
  (`~/.claude/plugins/cache/mattpocock/mattpocock-skills/<wersja>`). Źródła forka można więc poprawiać w trakcie
  przebiegu, a bump wersji robi się po nim.
- **Dowody z przebiegu:** transkrypty subagentów leżą w
  `C:\Users\hondo\.claude\projects\<ścieżka-worktree-z-myślnikami>\<sesja>\subagents\agent-*.jsonl`, a
  `.meta.json` obok podaje `agentType` i model. Po każdym przebiegu przeszukaj je pod kątem „Refusing to run it”.
- **Start żywego przebiegu:** `start_session` z `initiation: user_asked`, `cwd` sandboksa, `use_worktree: false`,
  `permission_mode: auto`, modelem `claude-sonnet-5-5`, effortem `medium`, `wake_when_done: true` i promptem
  `/mattpocock-skills:ship <ficzer>`. Między subagentami sesja kończy turę („waiting for …”) i to jest normalne.
- **Koszt:** przebieg C (jeden ticket, review gałęzi na Opusie i jedna runda) zużył 10 punktów okna
  5-godzinnego i 1 punkt tygodnia.
- **Pozostałe pułapki z `docs/handoff-2026-10-26-agent-loop-05.md`, sekcja 6, nadal obowiązują:** `pwsh` jako
  narzędzie dotnet, `set -o pipefail` przed `--no-build`, Docker Desktop, kroki aktualizacji forka.

## 7. Workflow

Bez zmian względem ticketów 05 i 06:

1. Bramka w tej sesji: pytania w jednej rundzie, decyzje w `## Comments` ticketu jako „Decyzje przed
   implementacją”, commit.
2. Użytkownik wpisuje `/mattpocock-skills:implement <ticket>`.
3. `tdd` na seamach ticketu: tu pilot, więc testy procesu w `tests/HookTests` tylko tam, gdzie powstanie skrypt.
4. Red proof dla testów, które nie były czerwone w cyklu.
5. Pełny build i testy łańcuchem: `set -o pipefail`, potem
   `dotnet build CleanArchitecture.slnx && dotnet test CleanArchitecture.slnx --no-build`.
6. Review na osi Spec: subagent na Sonnecie, dwie rundy, z poleceniem sprawdzania wznowienia z każdego stanu.
7. Commity w obu repozytoriach, bump pluginu i aktualizacja.
8. Żywy przebieg w sandboksie, uruchamiany tylko na wyraźne słowa użytkownika.
9. Zamknięcie ticketu.

Push i scalenie `chore/agent-loop` do `main` należą do użytkownika, dopiero po pilocie.

## 8. Lekcje metodyczne do powtórzenia

1. **Po żywym przebiegu przeszukaj transkrypty subagentów, nie tylko raport sesji.** Odmowa `pwsh` była już
   w B2 (6 odmów verifiera), ale orkiestrator ją obszedł i raport tego nie pokazał. Transkrypty pokazały.
2. **Zasiane naruszenie sprawdza ścieżkę review.** `DateTime.UtcNow` w Application wywołał rundę poprawek, która
   inaczej by się nie uruchomiła. Przy bramce 2 zasiej podobnie: brief z propozycjami już jest.
3. **Przepis `git` sprawdzaj na repozytorium tymczasowym przed wpisaniem do skilla.** `git restore` z
   `:(exclude)` zastąpił serię revertów, która dawała konflikty na plikach uwag. Pytania 4 i 5 wymagają tego
   samego.
4. **Review Spec na Sonnecie ciągle znajduje dziury,** w 06 kolejno 11 i 7 uwag, głównie przy wznawianiu
   i rundach. Każ mu przechodzić wznowienie z każdego stanu pośredniego.
5. **Wynik narzędzia to nie dostarczenie.** „Mobile push requested” nie znaczy, że push doszedł. Fakt potwierdza
   dopiero użytkownik albo drugi kanał.
6. **Źródła forka poprawiaj w trakcie przebiegu, bump rób po nim.** Działająca sesja trzyma swoją wersję z cache.

## 9. Pierwszy krok i miejsce zatrzymania

1. Przeczytaj pliki z sekcji 3. Sprawdź fakty z pytań 4, 5 i 6: `git fetch . x:main` na repozytorium tymczasowym,
   `ExitWorktree` na worktree z `git worktree add` oraz dostęp sesji w tle do przeglądarki. Narzędzia szukaj
   przez `ToolSearch`, bo bywają odroczone.
2. Przedstaw użytkownikowi pytania z sekcji 4 w jednej rundzie: numerowane, z rekomendacją, sformułowane tak,
   żeby „tak” przyjmowało rekomendację.
3. Zapisz decyzje w `## Comments` ticketu 07, zacommituj i poproś użytkownika o
   `/mattpocock-skills:implement .scratch/agent-loop/issues/07-merge-gate.md`.
4. Zatrzymaj się po zamknięciu ticketu 07: commity w obu repozytoriach, zaktualizowany plugin, żywy przebieg
   w sandboksie na gołym `origin`. Nie zaczynaj ticketu 08 w tej samej sesji, jeśli kontekst przekroczył połowę
   okna albo okno 5-godzinne przekroczyło 70%.
