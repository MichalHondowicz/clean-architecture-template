# 07: Bramka 2: scalenie po „OK”

**What to build:** Na brief odpowiadam jedną wiadomością w tej samej sesji, także z telefonu. Po „OK” agent
scala gałąź do `main`, wypycha zmiany, sprząta i sprawdza CI. O każdej propozycji konwencji decyduję osobno,
a przyjęte trafiają do repozytorium osobnym commitem.

**Blocked by:** 06

**Seams:** pilot (ticket 10).

**Status:** ready-for-agent

- [ ] Po briefie sesja czeka na odpowiedź. Odpowiedź przez Remote Control działa tak samo jak z komputera
      (historia 35).
- [x] Po „OK”: fast-forward do `main`, `git push origin main`, usunięcie gałęzi i worktree, status CI
      sprawdzony w przeglądarce i raport z wynikiem (historia 36).
- [x] Allowlista zawiera dokładnie `git push origin main`, bez gwiazdki. Push innej gałęzi albo z `--force`
      nadal wymaga zgody (historia 37).
- [x] Każda propozycja konwencji ma osobną decyzję, niezależną od zgody na scalenie (historia 38).
- [x] Przyjęte propozycje trafiają osobnym commitem orkiestratora na gałąź ficzera przed scaleniem.
      Odrzucone wypadają (historia 39).
- [x] Bez „OK” nic nie trafia do `main`.
- [x] Wersja forka podbita.

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

**2026-10-27, implementacja (`/implement`):**

- **Fork, `/ship`, nowa sekcja `## Gate 2`** (kroki 1–10):
  - pytanie po raporcie i `## Decision` w `brief.md` (czas, `merge: OK|no`, propozycje accepted, rejected albo
    dropped, odpowiedź dosłownie);
  - przyjęte reguły trafiają tym samym commitem: `ship: gate 2, conventions <n>` albo `ship: gate 2 decision`;
  - scalenie przepisem z decyzji 4 i 5, a CI odczytem strony Actions (decyzja 6);
  - stop w krokach 5–8 zostawia zapisaną decyzję i nic do cofania.
- **Wznawianie** opiera się na trackerze i refach git (*Start or resume*, krok 3):
  - gałęzi brak, a `main` ma brief: tylko CI i koniec;
  - gałąź jest w `main`, a ostatni wpis to `merge: OK`: dalej od kroku 6 bez wchodzenia do worktree, więc
    zapisane i zastosowane „OK” obejmuje push i sprzątanie;
  - zapisane „OK”, którego nie ma w `main`: pytanie jeszcze raz.
- **Zabezpieczenia dopisane w review:**
  - przed pushem `main` musi równać się końcowi gałęzi, więc cudze commity nie wychodzą;
  - gdy remote już ma ficzer, push jest pomijany;
  - gałąź znika dopiero po usunięciu worktree, przez `update-ref -d` z jej własnym końcem sprawdzonym względem
    `origin/main`;
  - sesja uruchomiona w worktree ficzera kończy przed pytaniem;
  - `git status --porcelain --untracked-files=no`, żeby nieśledzone `.agents/` i `.codex/` nie blokowały
    scalenia.
- **„Never”:** wyjątki tylko dla pytania bramki, scalenia i sprzątania po „OK” oraz wpisu przyjętej reguły.
- **`standards-reviewer`:** każda propozycja podaje plik i sekcję docelową oraz regułę gotową do wklejenia,
  w języku pliku docelowego.
  - **Doprecyzowanie decyzji 3:** „dosłownie z briefu” wymaga, żeby reguła dla angielskiego `AGENTS.md` była
    po angielsku. Brief cytuje ją bez tłumaczenia, a uzasadnienie zostaje po polsku.
- **Dokumentacja:**
  - fork: `docs/engineering/ship.md` (sekcja Gate 2, FAQ), README, `ask-matt`;
  - template: `docs/agents/issue-tracker.md` (`## Decision` i commity bramki).
- **Odstępstwo od decyzji 2:** odpowiedź bez jasnego słowa o scaleniu nie zapisuje niczego, także decyzji
  o propozycjach. Sesja pyta jeszcze raz.

**Dowody:**

- **Testy:** brak cyklu `tdd`. Seam to pilot, a ticket nie dodaje skryptu, więc `tests/HookTests` bez zmian.
- **Przepisy git:** sprawdzone na repozytorium tymczasowym przed bramką, z wynikami w faktach powyżej.
- **Pełny build i testy, łańcuchowo:** 0 ostrzeżeń, 6 + 54 + 14 + 83 = 157 testów zielonych.
- **Review na osi Spec (Sonnet), dwie rundy, z przejściem przez każdy stan wznowienia:**
  - **Runda 1, `fail`, 7 uwag:**
    - CI pomijane przy wznowieniu;
    - kolejność sprzątania;
    - niepotrzebne odtwarzanie worktree;
    - częściowa odpowiedź;
    - niezapisana reguła o zapisanym „OK”;
    - cudze commity w pushu;
    - nieudany fetch.

    Wszystkie poprawione.
  - **Runda 2, `pass`, 4 uwagi:**
    - sprzątanie blokowało się, gdy `main` poszedł dalej po pushu;
    - `ExitWorktree` bez wejścia;
    - sesja w worktree pytała przed stopem;
    - ponowne pytanie przy zapisanym „OK”.

    Wszystkie poprawione.

**Commity:** fork `2db5c3d` (plugin `1.3.1-dotnet.16`, zainstalowany).

**Do zamknięcia ticketu:**

- **Allowlista** (decyzja 7). Linie dopisuje użytkownik w `.claude/settings.json` template'u i sandboksa:
  - `Bash(git push origin main)` i `PowerShell(git push origin main)`;
  - `Bash(git fetch origin)` i `PowerShell(git fetch origin)`;
  - `Bash(git fetch . feat/*:main)` i `PowerShell(git fetch . feat/*:main)`;
  - `Bash(git update-ref -d refs/heads/feat/*)` i `PowerShell(git update-ref -d refs/heads/feat/*)`.

  `git merge --ff-only *`, `git worktree *`, `git merge-base*` i `git rev-parse*` już na niej są.
- **Żywy przebieg w sandboksie** (decyzja 8).

**2026-10-28, allowlista i żywy przebieg D w sandboksie:**

- **Allowlista:** osiem linii z decyzji 7 dopisane przez użytkownika.
  - Template: commit `b6c3898`.
  - Sandbox: commit `36d3411` na `chore/agent-loop` i `8a2a3e3` na `feat/toy-brief`, bo sesja w worktree
    czyta ustawienia z gałęzi ficzera.
- **Konfiguracja:**
  - goły origin `D:\Projects\ship-sandbox-origin.git`, a jego `main` na `53439c4`;
  - brief `toy-brief` uzupełniony ręcznie o pliki docelowe, angielskie reguły i blok pytania (commit
    `9844a38`), bo powstał na wersji `.15`;
  - plugin `.16`, Sonnet z effortem medium, tryb auto;
  - limit na starcie: okno 5-godzinne 9%, tydzień 45%.
- **Przebieg:**
  - `/ship toy-brief` w checkoucie głównym (`chore/agent-loop`) wszedł do worktree, odczytał Remote Control
    (`on`), pominął limit, środowisko i baseline, raportował i zadał jedno pytanie. Pusha nie wysłał, zgodnie
    ze skillem;
  - odpowiedź `Ok, 1 tak, 2 nie`;
  - commit `ship: gate 2, conventions 1` (`87b0b81`) z wpisem `## Decision` i regułą „Domain logic” dosłownie
    w `AGENTS.md` po **Entities**. Propozycja 2 odrzucona, w `AGENTS.md` jej nie ma;
  - `ExitWorktree keep`, `git fetch . feat/toy-brief:main` (checkout na innej gałęzi), `main` równy końcowi
    gałęzi, `git push origin main`: lokalny i zdalny `main` na `87b0b81`;
  - gałąź `feat/toy-brief` usunięta;
  - CI: `no CI for this remote`.
  - Bez żadnego pytania o zgodę: tryb auto przepuścił push. Nie wiadomo, czy zadziałała reguła z allowlisty, czy
    klasyfikator, bo polecenie było łańcuchem (niżej).
- **Znaleziska → fork `e47e934`, plugin `1.3.1-dotnet.17`:**
  - **Łańcuchy poleceń.** W checkoucie głównym, który nie jest izolowany, sesja łączyła polecenia `;`, a push
    i sprzątanie szły w jednym łańcuchu z warunkami. Wynik był poprawny, ale decyzja o pushu i usunięciu gałęzi
    zależała od kolejności w łańcuchu, a nie od odczytanego kodu wyjścia. Gate 2 każe teraz jedno polecenie na
    wywołanie, także w checkoucie głównym.
  - **Worktree trzymany przez proces.** `git worktree remove` zwrócił `Permission denied`, ale git wyrejestrował
    worktree i został pusty folder trzymany przez powłokę sesji. Gałąź poszła mimo `rm=255`, bo łańcuch tego nie
    pilnował. Skill mówi teraz: worktree jest usunięty, gdy nie pokazuje go `git worktree list`; pusty folder
    zgłasza się użytkownikowi.
- **Niesprawdzone na żywo** (do pilota 10):
  - ścieżka `git merge --ff-only` przy checkoucie na `main`;
  - krok CI w przeglądarce sesji w tle;
  - wznawianie przerwanej bramki;
  - odpowiedź niejasna i odpowiedź „nie”;
  - push innej gałęzi albo z `--force` przy obecnej allowliście.
- **Do sprzątnięcia przez użytkownika:** pusty folder `D:\Projects\ship-sandbox\.claude\worktrees\toy-brief`
  po zamknięciu sesji przebiegu.
