# 09: Porządki i przewodnik

**What to build:** Repozytorium i mój przewodnik opisują nowy przepływ, a nic nie wprowadza agenta w błąd:
znika stara kopia skilli, `docs/workflow.md` opisuje dwie bramki, `AGENTS.md` wskazuje `/plan-feature`
i `/ship`, a każda decyzja, o którą agent dotąd pytał w trakcie, jest regułą albo częścią bramki 1.

**Blocked by:** 04, 05, 06, 07, 08

**Seams:** brak testów automatycznych. Sprawdzenie w review Spec i w pilocie (ticket 10).

**Status:** ready-for-agent

- [ ] Nieśledzony katalog `.agents/skills/` jest usunięty (historia 60).
- [ ] `docs/workflow.md` opisuje przepływ `/plan-feature` → bramka 1 → `/ship` → push → bramka 2, tabelę
      „Ty / agent”, powiadomienia, limity i regułę decyzji po pilotach (historie 48, 61).
- [ ] Sekcja procesu w `AGENTS.md` wskazuje `/plan-feature`, `/ship`, role i modele, plik DoD i kontrakt
      implementera, bez powtarzania treści skilli.
- [ ] Każda decyzja z `docs/workflow.md` i z pilota archive-todo, którą dawniej podejmowałem w trakcie
      implementacji, jest regułą w `AGENTS.md` albo w skillu, albo częścią bramki 1 (historia 5).
- [ ] Build: 0 błędów, 0 ostrzeżeń; pełne testy zielone.

## Comments

**2026-10-26, przeniesione z bramki ticketu 05** (do przewodnika `docs/workflow.md`):

- sesję `/ship` uruchamia się na Sonnecie;
- po resecie tygodniowym i po restarcie aplikacji w trakcie czekania na okno 5-godzinne `/ship <feature>`
  wpisuje użytkownik;
- tryb auto nie pozwala agentowi edytować `.claude/settings.json`, więc wpisy allowlisty dodaje użytkownik;
- `claude -p` jest nieużywalne, bo samodzielne CLI nie jest zalogowane.
