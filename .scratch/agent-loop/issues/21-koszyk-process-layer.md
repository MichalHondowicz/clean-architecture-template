# 21: Koszyk: warstwa procesu Pococka

**What to build:** Koszyk dostaje to, z czego czytają skille procesu: tracker, etykiety, układ dokumentów
domeny, sekcję procesu w `AGENTS.md` i `CLAUDE.md` bez starego workflow modułów. Konwencje Koszyka zostają
jego własne. Według `docs/workflow.md` template'u, „Istniejący projekt”, kroki 1–4.

**Blocked by:** None

**Seams:** pliki w `D:\Projects\Koszyk` (`docs/agents/`, `AGENTS.md`, `CLAUDE.md`, `GLOSSARY.md`); obserwacja
w pilocie (ticket 26).

**Status:** ready-for-agent

**Skąd:** ticket 11, decyzja 3 z bramki 2026-11-04 (historie 55, 63).

- [ ] AC1: `/setup-matt-pocock-skills` w Koszyku: tracker `.scratch/<feature>/` (`docs/agents/issue-tracker.md`),
      pięć etykiet (`docs/agents/triage-labels.md`), jeden kontekst domeny (`docs/agents/domain.md`).
- [ ] AC2: `AGENTS.md` Koszyka ma sekcję „Process skills in this project” z komendami Koszyka: build obu solucji,
      test jednego pliku i pełne testy (`dotnet test Koszyk.sln`, `dotnet test tests/Koszyk.Mobile.Tests`),
      review ticketu tylko na osi Spec, `/plan-feature` i `/ship`. Konwencje backendu i mobilne zostają bez zmian.
- [ ] AC3: `CLAUDE.md` zamiast sekcji „Workflow modułów: plugin dotnet-workflow” wskazuje `/plan-feature`
      i `/ship`. `docs/tasks/` zostaje jako historia modułów 0–14, a `AGENTS.md`, „Praca w modułach”, mówi,
      że nowe ficzery idą przez tracker.
- [ ] AC4: `GLOSSARY.md` z pojęciami z `Koszyk.Core` (przez `domain-modeling`) i `docs/adr/` z decyzjami,
      które już zapadły w specach `docs/superpowers/specs/`, zapisanymi jako ADR-y.
- [ ] AC5: Build obu solucji 0 ostrzeżeń i testy zielone po zmianach (zmieniają się tylko dokumenty).
