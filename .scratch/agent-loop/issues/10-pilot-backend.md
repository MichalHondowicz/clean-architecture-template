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
jest więc prawie pewny. Można go założyć przed pilotem.
