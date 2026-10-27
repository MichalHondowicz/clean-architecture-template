# 07: Bramka 2: scalenie po „OK”

**What to build:** Na brief odpowiadam jedną wiadomością w tej samej sesji, także z telefonu. Po „OK” agent
scala gałąź do `main`, wypycha zmiany, sprząta i sprawdza CI. O każdej propozycji konwencji decyduję osobno,
a przyjęte trafiają do repozytorium osobnym commitem.

**Blocked by:** 06

**Seams:** pilot (ticket 10).

**Status:** ready-for-agent

- [ ] Po briefie sesja czeka na odpowiedź. Odpowiedź przez Remote Control działa tak samo jak z komputera
      (historia 35).
- [ ] Po „OK”: fast-forward do `main`, `git push origin main`, usunięcie gałęzi i worktree, status CI
      sprawdzony w przeglądarce i raport z wynikiem (historia 36).
- [ ] Allowlista zawiera dokładnie `git push origin main`, bez gwiazdki. Push innej gałęzi albo z `--force`
      nadal wymaga zgody (historia 37).
- [ ] Każda propozycja konwencji ma osobną decyzję, niezależną od zgody na scalenie (historia 38).
- [ ] Przyjęte propozycje trafiają osobnym commitem orkiestratora na gałąź ficzera przed scaleniem.
      Odrzucone wypadają (historia 39).
- [ ] Bez „OK” nic nie trafia do `main`.
- [ ] Wersja forka podbita.

## Comments

**2026-10-27, decyzje przed implementacją** (bramka w sesji po handoffie
`docs/handoff-2026-10-27-agent-loop-07.md`; wszystkie rekomendacje przyjęte):

**Fakty sprawdzone przed bramką:**

- **`git fetch . feat/<f>:main`** (repozytorium tymczasowe):
  - przy fast-forwardzie przesuwa lokalny `main` bez przełączania checkoutu;
  - przy rozjechanym `main` odrzuca zmianę („non-fast-forward”, exit 1);
  - gdy `main` jest wyciągnięty w którymkolwiek checkoucie, odmawia („refusing to fetch into branch …
    checked out”, exit 128). `--update-head-ok` by to obszedł, ale zostawiłby drzewo tamtego checkoutu
    nieaktualne.
- **`git branch -d feat/<f>`** w checkoucie głównym stojącym na innej gałęzi niż `main` odmawia
  („not fully merged”), choć gałąź jest już w `main`: `-d` porównuje z HEAD.
- **`git update-ref -d refs/heads/feat/<f> refs/heads/main`** usuwa gałąź tylko wtedy, gdy jej koniec to
  dokładnie `main` (exit 0). W przeciwnym razie odmawia („cannot lock ref … expected …”, exit 1).
- **`git worktree remove .claude/worktrees/<f>`** z checkoutu głównego działa.
- **`ExitWorktree`:** według opisu narzędzia nie usuwa worktree, do którego sesja weszła z `path`, i każe
  użyć `keep`.
- **CI:** sesja na pierwszym planie czyta stronę Actions template'u przez `get_page_text` bez zgody do
  kliknięcia. Dostępu sesji w tle nie sprawdzono.

**Decyzje:**

1. **Czekanie po briefie w tej samej sesji.**
   - Po briefie sesja kończy turę jednym pytaniem. Odpowiedź w tej samej sesji, z komputera albo przez
     Remote Control, wznawia ją w nowej sekcji skilla `## Gate 2`.
   - Ponowny `/ship` przy briefie bez scalenia zadaje pytanie jeszcze raz, zamiast kończyć bez niczego.
   - „Never” dostaje wyjątek tylko dla tego pytania.
   - Otwarte: czy sesję w tle widać w zakładce Code na telefonie i czy da się z niej odpisać. Potwierdzi to
     użytkownik w żywym przebiegu.
2. **Format odpowiedzi.**
   - Brief kończy się blokiem pytania: scalenie `OK` albo `nie`, każda propozycja `<n>: tak` albo `<n>: nie`.
   - Odpowiedź to jedna wiadomość, np. `OK, 1 tak, 2 nie`.
   - Scala tylko jednoznaczne „OK”. Pytanie, uwaga albo coś niejasnego nie wysyła nic do `main`: sesja
     odpowiada i czeka dalej.
   - Propozycja bez decyzji wypada, a raport mówi o tym wprost.
3. **Przyjęte propozycje: commit orkiestratora.**
   - Po decyzji użytkownika orkiestrator wpisuje przyjętą regułę dosłownie z briefu do pliku docelowego,
     osobnym commitem na gałęzi ficzera przed scaleniem (historia 39).
   - Wyjątek w „Never” dla ścieżek chronionych obejmuje tylko ten zapis.
   - `standards-reviewer` podaje w każdej propozycji plik docelowy: `AGENTS.md` albo checklistę skilla
     standardów.
4. **Fast-forward.**
   - W worktree: `git fetch origin`, potem `git merge-base --is-ancestor origin/main feat/<f>`. Wynik inny
     niż 0: stop i raport, bez rebase'u.
   - `ExitWorktree` z `keep`, dalej w checkoucie głównym:
     - stoi na `main` i jest czysty: `git merge --ff-only feat/<f>`;
     - stoi na innej gałęzi: `git fetch . feat/<f>:main`;
     - stoi na `main` z niezacommitowanymi zmianami: stop i raport.
   - Nigdy `--update-head-ok`.
   - `git push origin main`. Bez `--force` remote odrzuci push, który nie jest fast-forwardem.
5. **Sprzątanie.**
   - `git worktree remove .claude/worktrees/<f>`, potem `git update-ref -d refs/heads/feat/<f> refs/heads/main`
     zamiast `git branch -d`.
   - Wyjątek w „Never” (usuwanie gałęzi i worktree, push, scalanie) tylko po „OK” i udanym pushu.
6. **CI.**
   - Odczyt strony Actions dla `main` przez `get_page_text` we wbudowanej przeglądarce, ponowny odczyt co
     2 minuty, najwyżej 20 minut. Czekanie: `Start-Sleep -Seconds 120` w tle narzędziem PowerShell.
   - Czerwone CI: raport i push, bez revertu.
   - Remote spoza GitHuba albo przeglądarka niedostępna w sesji w tle: raport „CI niesprawdzone” z linkiem.
   - Dostęp sesji w tle do przeglądarki sprawdzi pilot 10.
7. **Allowlista.**
   - Dokładnie `git push origin main` (bez gwiazdki), `git fetch origin`, `git fetch . feat/*:main` oraz
     `git update-ref -d refs/heads/feat/*`, każda reguła w wersji `Bash(...)` i `PowerShell(...)`.
   - Linie do `.claude/settings.json` template'u i sandboksa dopisuje użytkownik: tryb auto blokuje edycję
     tego pliku.
   - Czy klasyfikator trybu auto przepuszcza push do `main` przy tej regule, pokaże żywy przebieg.
8. **Żywy przebieg w sandboksie.**
   - Przed przebiegiem goły klon `D:\Projects\ship-sandbox-origin.git` i przestawiony na niego `origin`
     sandboksa.
   - Ficzer `toy-brief` stoi na bramce 2 z dwiema propozycjami. „OK” wpisuje użytkownik, najlepiej z telefonu.
   - Krok CI: raport „brak CI dla tego remote”.
   - Checkout sandboksa stoi na `chore/agent-loop`, więc przebieg sprawdza ścieżkę `git fetch . feat/<f>:main`.
     Ścieżkę `git merge --ff-only` sprawdzi pilot 10.
