# 19: Resztki pilota 10: wznowienie bez użytkownika i push przy pytaniu o zgodę

**What to build:** Dwa kryteria pilota 10, których pilot nie mógł sprawdzić, zostają sprawdzone przy okazji
następnego prawdziwego ficzera puszczonego przez `/ship`, bez osobnego przebiegu ustawionego tylko pod nie.

**Blocked by:** None

**Seams:** obserwacja w kolejnym przebiegu `/ship` (statusy ticketów, `metrics.md`, log sesji w
`~/.claude/projects/<projekt>/*.jsonl`, telefon). Bez zmian w kodzie, dopóki obserwacja nie pokaże błędu.

**Status:** needs-triage

**Skąd:** pilot 10 (2026-10-29/30), przeniesione decyzją użytkownika z 2026-11-01 przy zamknięciu ticketu 10.

- Scenariusz (c): w przebiegu 2 pauza na progu 49% zadziałała (`ship: pause, 5-hour limit`, wiersz pauzy,
  `wait-for-reset.ps1` w tle). Wznowienia nie było: o 23:47 Windows Update zrestartował komputer (ticket 16).
  Rano przebieg ruszył dopiero po wiadomości użytkownika w starej sesji. W przebiegu 1 limit nie padł.
- Push przy pytaniu o zgodę: w trzech przebiegach pilota (todo-comments, todo-comment-count część 1–3)
  nie padło ani jedno pytanie o zgodę, więc nie było czego wysłać.

- [ ] Pauza na limit 5-godzinny kończy się wznowieniem bez udziału użytkownika: `wait-for-reset.ps1` wraca,
      a pętla sama sprawdza limit i środowisko (ticket 17) i rusza z kolejnym ticketem (historie 40, 41).
- [ ] Pytanie o zgodę w sesji `/ship` w tle daje push na telefon. Jeśli push nie dociera jako powiadomienie,
      wynik trafia do ticketu 12 (fallback: hook `Notification` i ntfy) (historia 46).
