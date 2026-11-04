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
