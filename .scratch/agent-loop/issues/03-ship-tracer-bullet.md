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

**Status:** ready-for-agent

- [ ] `/ship <feature>` jest tylko dla człowieka i działa na Sonnecie (historia 8).
- [ ] Pre-flight: Docker działa albo `/ship` uruchamia Docker Desktop i czeka na niego; komputer dostaje
      czuwanie; worktree jest czysty. Niepowodzenie Dockera lub buildu bazowego zatrzymuje ficzer i wysyła
      push „awaria środowiska” z jednym zdaniem (historie 10, 11, 31, 43).
- [ ] Worktree i gałąź `feat/<slug>` per ficzer; główny checkout zostaje nietknięty (historia 9).
- [ ] Tickety idą po kolei, w kolejności zależności, bez równoległości i bez agenta scalającego. Ticket
      w pracy ma status `in-progress`, opisany w etykietach trackera obok `done` (historia 14).
- [ ] Start i wznowienie to ta sama operacja: przerwany przebieg po ponownym `/ship` kontynuuje od ticketu
      `in-progress` i nie powtarza ticketów `done` (historia 13).
- [ ] Rola implementera z forka (Sonnet, `maxTurns`) dostaje wejście z kontraktu. Jej nazwa jest zgodna
      z plikiem DoD, więc hooki z 01 i 02 działają w jej sesji (historie 15, 16, 17).
- [ ] `tdd` traktuje seamy z sekcji `Seams:` zatwierdzonego ticketu jako uzgodnione i nie pyta o nie. Bez
      ticketu zachowuje się jak dziś (historie 3, 15).
- [ ] Kontrakt implementera spisany w dokumentacji agentów template'u. Wejście: ticket, worktree, gałąź,
      numer rundy, plik uwag. Wyjście: commity, wpis w `## Comments` z nazwą testu i dowodem czerwieni dla
      każdego `ACn`, status `done` albo `stuck`. Zakazy: chronione ścieżki z pliku DoD (historie 17, 20).
- [ ] Format ticketu (sekcja `Seams:`, kryteria `AC1…n` z planowaną nazwą testu) opisany w dokumentacji
      trackera i w szablonie `to-tickets` (historia 4).
- [ ] Allowlista projektu obejmuje `git worktree`, `git switch`, `git merge --ff-only` i uruchomienie Docker
      Desktop. Sesja pętli działa w trybie auto, nigdy w `bypassPermissions` (historia 12).
- [ ] Żywy mini-przebieg: ficzer-zabawka dochodzi do `done` bez żadnego pytania o zgodę. Wynik (statusy,
      czas, zużycie limitu, ewentualne pytania) zapisany w `## Comments` tego ticketu.
- [ ] `implement` i `implement-spec` w forku są bez zmian (historia 62).
- [ ] Wersja forka podbita, plugin zaktualizowany.
