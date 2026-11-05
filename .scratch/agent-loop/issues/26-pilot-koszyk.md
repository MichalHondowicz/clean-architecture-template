# 26: Pilot 2: mały ficzer Koszyka przez pętlę

**What to build:** Mały ficzer full-stack z ekranem przechodzi w Koszyku `/plan-feature`, bramkę 1, `/ship`
i bramkę 2 aż do `main` na GitHubie. To pilot 2 z pomiarem nr 2 i pierwsze sprawdzenie pętli na MAUI.

**Blocked by:** 20, 23, 24, 25

**Seams:** seam 1 ze speca: statusy ticketów, `## Comments`, `metrics.md`, brief, historia gałęzi,
powiadomienia, CI po scaleniu.

**Status:** ready-for-human

**Skąd:** ticket 11, decyzje 1 i 8 z bramki 2026-11-04 (historie 54, 58, 63). Zakres pilota: dwie lub trzy
pozycje „kosmetyki UX” z modułu 15 (`D:\Projects\Koszyk\docs\tasks\state-15-mobile-polish-hardening.md`),
np. przecinek w polu `Numeric` i podwójny alert „Brak dostępu”.

- [ ] AC1: Ficzer dochodzi do `main` bez udziału użytkownika poza bramkami. Każde pytanie o zgodę trafia do
      allowlisty Koszyka.
- [ ] AC2: Brief wymienia zmienione ekrany i pliki do sprawdzenia na emulatorze (`screenPaths`), a użytkownik
      sprawdza je na emulatorze przed `OK`.
- [ ] AC3: Tabela pomiaru jest pełna i zapisana jako pomiar nr 2 (historia 47), obok pomiaru nr 1 z ticketu 10.
- [ ] AC4: Obserwacje z ticketu 19 (wznowienie bez użytkownika, push przy pytaniu o zgodę) są zapisane, jeśli
      przebieg na nie trafi.
- [ ] AC5: Rozjazdy pętli i dokumentów Koszyka są zapisane jako tickety `needs-triage`.

**2026-11-05, przebieg 1 (`ux-cosmetics-1`), zapis agenta z repozytorium Koszyka, bez raportu sesji `/ship`:**

- Plan: commit `e78d1f6` w Koszyku. `/ship` 2026-11-04 22:02–22:33: tickety 01–03 `done` (rundy 1, 2, 1), runda
  gałęzi 1 (nazwy testów ticketu 03), brief, decyzja „OK, 1 tak” (`b4b7b04`; konwencja `System.TimeProvider`
  w `AGENTS.md`). Testy 99/5/130/611, 0 ostrzeżeń. Nowe `needs-triage`: 04 (e-mail „Ty” po synchronizacji, DTO
  i API), 05 (nazwy testów `SuggestionsViewModelTests`).
- `metrics.md` w Koszyku (`.scratch/ux-cosmetics-1/metrics.md`): 26 minut, 5h +19 pp (44% → 63%), tydzień +2 pp
  (58% → 60%).
- Po „OK” `main` nie został przesunięty: Koszyk nie ma remote (ticket 20), więc krok scalenia z `git fetch origin`
  i `git push origin main` nie mógł przejść. 2026-11-05 na słowo użytkownika w tej sesji: `main` przesunięty
  fast-forwardem na `b4b7b04`, worktree wyrejestrowany i gałąź `feat/ux-cosmetics-1` usunięta. Pusty folder
  `.claude/worktrees/ux-cosmetics-1` zostaje do zamknięcia sesji `/ship` (blokada katalogu). Bez pushu.
- Nie sprawdzone: ekrany `ListsPage` i `MembersPage` na emulatorze (AC2), pytania o zgodę i wznowienie (AC4).
  Ticket zostaje `ready-for-human`: AC1 wymaga przebiegu z remote, AC3 wpisu pomiaru nr 2.
