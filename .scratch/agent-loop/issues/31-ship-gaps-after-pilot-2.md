# 31: Luki `/ship` z przeglądu stanu po pilocie 2

**What to build:** `/ship` nie czeka na CI, którego repozytorium nie ma. Na scalonym ficzerze mówi, co dalej
z jego otwartymi ticketami. W raporcie widać, że fallback ntfy jest wyłączony.

**Blocked by:** None

**Seams:** `ship/SKILL.md` i strona `docs/engineering/ship.md` w forku; przewodnik `docs/workflow.md`; obserwacja
w następnym przebiegu `/ship` w Koszyku (bramka 2 bez workflow, raport bez tematu ntfy).

**Status:** done

**Skąd:** przegląd stanu pluginu 2026-11-05, po pierwszym przebiegu pilota 2 (ticket 26). Decyzja użytkownika:
„Tak, popraw 4–6”.

- Koszyk dostał remote na GitHubie (2026-11-05, `b4b7b04` wypchnięty), ale nie ma `.github/workflows`
  (ticket 24). Krok 10 bramki 2 czytałby więc stronę Actions do 10 razy co 2 minuty i kończył
  `CI not checked`.
- `ux-cosmetics-1` w Koszyku jest scalony, a jego ticket 04 dostał po triage'u `ready-for-agent`.
  `/ship ux-cosmetics-1` kończy na „scalony”, więc ticket 04 powstawał ręcznie, w innej sesji, na `main`,
  bez weryfikatora i recenzenta. Ani skill, ani przewodnik nie mówiły, którędy taki ticket idzie.
- `CLAUDE_NTFY_URL` nie jest ustawiona, więc `notify.ps1` kończy się kodem 0 z `"sent":false`. Raport `/ship`
  wymieniał tylko kod 1, więc wyłączony fallback wyglądał jak działający.

- [x] AC1: Krok 10 bramki 2 daje `no CI: no workflow` bez otwierania strony Actions, gdy
      `git ls-tree --name-only origin/main .github/workflows/` nic nie wypisuje.
- [x] AC2: `/ship <feature>` na scalonym ficzerze wymienia w raporcie każdy ticket, który nie jest `done` ani
      `wontfix`, z jego statusem i drogą: `needs-triage` do `/triage`, `ready-for-agent` do `/implement` albo
      do nowego ficzera przez `/plan-feature`.
- [x] AC3: `"sent":false` z `notify.ps1` trafia do raportu końcowego raz na przebieg, razem z powodem.
- [x] AC4: Strona `docs/engineering/ship.md` forka i przewodnik `docs/workflow.md` opisują te trzy zachowania.
- [x] Wersja forka podbita.

## Comments

**2026-11-05, wynik:** fork `998fedd`, `8e599f0`, `9ccb947`, `762c1f6`, wersja `c6cef67` (plugin
`1.3.1-dotnet.30`, zainstalowany, cache ma nowy tekst). Przewodnik `docs/workflow.md` w tym samym commicie co
ten ticket: bramka 2 (brak workflow), „Po ficzerze” (droga ticketu po scaleniu), „Powiadomienia” (raport
bez tematu).

- **Poza punktami 4–6, znalezione przy pracy** (`9ccb947`): Git Bash przepisuje argument
  `feat/<feature>:.scratch/<feature>/brief.md` na `feat\<feature>;.scratch\<feature>\brief.md`, więc
  `git show` w kroku 3 startu (gałąź istnieje, `main` ją zawiera, decyzja `merge: OK`) kończył się kodem 128.
  Wznowienie po przerwanej bramce 2 mogło przez to nie zobaczyć zapisanego `OK`. Teraz ta komenda idzie przez
  narzędzie PowerShell. Rewizja bez ukośnika (`main:.scratch/...`) przechodzi bez zmian, a nowe sprawdzenie
  workflow podaje rewizję i ścieżkę jako osobne argumenty.
- **Sprawdzone:** `git ls-tree --name-only origin/main .github/workflows/` w template'cie wypisuje
  `.github/workflows/build.yml`, a w Koszyku nic (kod 0 w obu, w Git Bash i PowerShell). `claude plugin
  validate .` przechodzi. Z `--strict` przegrywa na dwóch ostrzeżeniach (`CLAUDE.md` w katalogu głównym, brak
  ikony), takich samych przy `9dceb0e`.
- **Niesprawdzone w przebiegu:** wszystkie trzy zachowania sprawdzi pierwszy przebieg `/ship`, który na nie
  trafi. Najbliżej jest bramka 2 następnego ficzera Koszyka przed ticketem 24.
- **Review Spec:** nie było. Komendy są sprawdzone na prawdziwych repozytoriach, nowy tekst skilla tylko
  przeczytany.
