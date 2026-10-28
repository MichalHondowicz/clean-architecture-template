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
