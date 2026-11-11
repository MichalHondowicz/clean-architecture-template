# 29: Template: `add-screen` i konwencje mobilne w `AGENTS.md`

**What to build:** Template dostaje skill projektu `add-screen` (przeniesiony z Koszyka i dopasowany do warstwy
z ticketu 28) i sekcję konwencji mobilnych w `AGENTS.md`, obok backendowych.

**Blocked by:** 28

**Seams:** tekst skilla i `AGENTS.md`; wzorce zgodne z kodem template'u (`git grep`); obserwacja w następnym
ficzerze z ekranem.

**Status:** wontfix

**Skąd:** ticket 11, decyzje 7 i 9 z bramki 2026-11-04 (historie 52, 55).

- [ ] AC1: `.claude/skills/add-screen/SKILL.md` opisuje ViewModel, stronę z `x:DataType`, klient z DTO
      z `Contracts`, rejestrację w `MauiProgram` i testy `Mobile.UnitTests`, z przykładami z ekranu todo.
- [ ] AC2: `AGENTS.md` ma sekcję mobilną: warstwy i zależności, skompilowane bindingi, testy `Mobile.Core`,
      pułapki głowy MAUI, które testy nie łapią. Tabela „Project skills” wskazuje `add-screen`.
- [ ] AC3: `ca-review` sprawdza konwencje mobilne (checklista w `.claude/skills/ca-review/SKILL.md`).
- [ ] AC4: `docs/workflow.md` opisuje warstwę mobilną w „Nowy projekt z template'u”.

## Comments

**2026-11-11, decyzja użytkownika (przegląd workflow):** `wontfix` na teraz. Warstwa MAUI w template'cie nie ma dziś
drugiego odbiorcy, a Koszyk ma już `add-screen`, DoD mobilne i konwencje. Wraca jako nowy plan (`/plan-feature`),
gdy ruszy drugi projekt MAUI.
