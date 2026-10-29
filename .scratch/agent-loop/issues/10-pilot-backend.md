# 10: Pilot 1: ficzer backendowy przez całą pętlę

**What to build:** Prawdziwy mały ficzer backendowy przechodzi `/plan-feature`, bramkę 1, `/ship` i bramkę 2
bez mojego udziału poza bramkami. Pilot sprawdza też ścieżki awaryjne i daje pierwszy pomiar zużycia limitu.

**Blocked by:** 09

**Seams:** seam 1 ze speca. Obserwacja przez statusy ticketów, `## Comments`, tabelę pomiaru, brief, historię
gałęzi i otrzymane powiadomienia.

**Status:** ready-for-human

- [ ] Wybrany ficzer backendowy (najlepiej 3 tickety) przechodzi od `/plan-feature` do scalenia w `main`
      (historie 1–8, 57).
- [ ] Scenariusz (a): między bramkami nie pada żadne pytanie o zgodę. Każde pytanie, które się pojawiło,
      zostaje dopisane do allowlisty (historia 12).
- [ ] Scenariusz (b): Docker zatrzymany przed startem, pętla uruchamia go sama (historia 11).
- [ ] Scenariusz (c): próg w pliku DoD tuż nad bieżącym zużyciem daje pauzę i wznowienie bez mojego udziału
      (historie 40, 41).
- [ ] Scenariusz (d): ticket niemożliwy do zrobienia kończy się `needs-info`, briefem i pushem, a niezależne
      tickety idą dalej (historie 28, 29).
- [ ] Push przy pytaniu o zgodę w sesji w tle dociera na telefon. Jeśli nie dociera, powstaje ticket na
      fallback (hook `Notification` i ntfy) (historia 46).
- [ ] Tabela pomiaru jest pełna, a zużycie ficzera zapisane jako pomiar nr 1 (historia 47).
- [ ] Propozycje konwencji z briefu są rozstrzygnięte, a rozjazdy pętli zapisane jako tickety `needs-triage`.

## Comments

**2026-10-27, z ticketu 06:** pushe proaktywne z sesji z Remote Control `on` już nie docierają. Narzędzie
odpowiada „Mobile push requested”, ale na telefon nie przyszedł żaden z czterech testów, także po wyłączeniu i
włączeniu Remote Control (szczegóły w Comments ticketu 06). Ticket na fallback (hook `Notification` i ntfy)
założony przed pilotem jako 12 (`needs-triage`).

**2026-10-28, z ticketu 08: pierwszy pełny przebieg `/plan-feature` → `/ship` → bramka 2** (sandbox, ficzer
zabawkowy `todo-open-count`, jeden ticket). To nie jest pilot, tylko dowód, że łańcuch działa bez interwencji
poza bramkami:

- plan: grill na Opusie, jeden commit `plan: todo-open-count`, sesja `/ship` z karty na Sonnecie;
- `/ship`: ticket 01 w jednej rundzie, review gałęzi na Opusie bez uwag w zakresie, uwaga spoza zakresu jako
  ticket 02 (`needs-triage`), brief, odpowiedź na bramce 2, fast-forward i push do gołego origin, gałąź
  i worktree usunięte;
- pomiar: ticket 5 minut i +10% okna 5-godzinnego, review gałęzi 2 minuty i +3%, tydzień +2% łącznie;
- odpowiedź „OK, 1 tak, 2 tak” była pomyłką (zamierzone „2 nie”); konwencję 2 cofnięto ręcznie osobnym
  commitem `b493a0f`. Skill nie ma ścieżki korekty decyzji po scaleniu; do rozważenia w przewodniku (09).

Pilot nadal musi przejść scenariusze (b)–(d) ze spec i ficzer z kilkoma ticketami.

**2026-10-29, decyzje przed pilotem** (bramka w sesji po handoffie
`docs/handoff-2026-10-28-agent-loop-10.md`; wszystkie rekomendacje przyjęte):

**Fakty sprawdzone przed bramką:**

- **Limit na starcie sesji:** okno 5-godzinne 2% (reset 2026-10-29 23:49), tydzień 57% (reset 2026-10-30
  07:00). Przy progu 80% na pilota zostaje 23 punkty tygodnia; ficzer z 08 zużył +2%.
- **Tagi dublują istniejący kod.** `TodoItem` ma już `List<string> Labels`, a `toy-quota` dodał `HasLabel`.
- **`needs-info` zatrzymuje ficzer przed bramką 2.** `ship/SKILL.md`, koniec „Work the tickets”: review gałęzi
  rusza tylko wtedy, gdy każdy ticket jest `done` (`needs-triage` i `wontfix` się nie liczą). Ticket ze
  scenariusza (d) kończy więc przebieg bez briefu i scalenia.
- **`dod.json` czyta worktree.** `run-dod.ps1` szuka pliku od bieżącego katalogu (`Find-DodRoot`), a worktree
  powstaje z HEAD. Zmiana progu musi być zacommitowana na `main` przed `/ship`. Commit przywracający próg na
  `main` po utworzeniu gałęzi rozjechałby historię i fast-forward na bramce 2 by się nie udał.
- **Docker** był zatrzymany (`docker info` bez połączenia z silnikiem), a ścieżka startu z `environmentChecks`
  istnieje. `/plan-feature` Dockera nie potrzebuje.
- **`.agents/`** w template'cie pojawił się znowu 2026-10-29 o 20:46, choć usunięto go w 09. To kopia
  `.claude/skills` bez `README.md`. W sandboksie leży od 2026-10-26.
- **Sandbox:** zarejestrowane worktree `toy-overdue` i `toy-quota` były czyste, a ich gałęzie równe kopiom
  na gołym origin. Puste foldery `todo-open-count` i `toy-brief` zostały po przebiegach.

**Decyzje:**

1. **Miejsce: sandbox.** Krok CI po scaleniu zostaje niesprawdzony; sprawdzi go pierwszy ficzer w template'cie
   po scaleniu `chore/agent-loop`.
2. **Ficzer: komentarze do todo itemu.** Nowa encja `TodoComment` z migracją, trzy tickety: dodanie
   komentarza, lista komentarzy itemu, usunięcie komentarza. Własność przez item: cudzy item odpowiada 404.
3. **Scenariusze (b)–(d) w jednym przebiegu, wieczorem 2026-10-29:**
   - **(b):** Docker zostaje wyłączony do startu `/ship`.
   - **(c):** po bramce 1, tuż przed `/ship`, commit na `main` sandboksu z `fiveHourPercent` równym bieżącemu
     zużyciu plus 4. Ticket 01 rusza, pauza wypada przed następnym. Próg wraca do 90% commitem na gałęzi
     `feat/<feature>` w worktree w trakcie pierwszego czekania, przed następnym `ship: start`, żeby nie
     wszedł w diff żadnego ticketu.
   - **(d):** ticket niemożliwy pisany ręcznie po bramce 1, nie przez grill, żeby `/plan-feature` nie
     „naprawił” go zmianą `protectedPaths`. Wymaga nowej reguły w `tests/ArchitectureTests`, jest niezależny
     i wchodzi jako 02, a tickety planu 02–03 przechodzą na 03–04. Oczekiwane: `STATUS: stuck` w rundzie 1,
     `needs-info`, revert, push, tickety 03–04 idą dalej. Potem użytkownik oznacza go `wontfix` commitem na
     gałęzi i wpisuje `/ship` ponownie: review gałęzi, brief i bramka 2, przy okazji test wznawiania.
   - Pomiar liczy się mimo pauz.
4. **Ticket 13 po pilocie.** `wontfix` omija lukę, a ponowne `ready-for-agent` to droga ticketu 13.
5. **Sprzątanie sandboksu:** usunięte worktree `toy-overdue` i `toy-quota`, lokalne gałęzie `feat/toy-*`
   i puste foldery `todo-open-count` i `toy-brief`. Gałęzie na gołym origin zostają. Archiwizacja starych
   sesji przebiegów tylko na osobne słowo użytkownika.
6. **`.agents/` zostaje nieśledzony.** Nie wpływa na pilota: nowy worktree go nie ma, a bramka 2 używa
   `--untracked-files=no`.
