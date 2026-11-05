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
- [x] AC3: Tabela pomiaru jest pełna i zapisana jako pomiar nr 2 (historia 47), obok pomiaru nr 1 z ticketu 10.
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

**2026-11-05, pomiar nr 2** (`D:\Projects\Koszyk\.scratch\ux-cosmetics-1\metrics.md`, na `main` Koszyka `b4b7b04`):

| Etap | Rundy | Minuty | Okno 5h | Tydzień |
|---|---|---|---|---|
| 01 | 1 | 5 | 44% → 48% | 58% → 59% |
| 02 | 2 | 9 | 48% → 53% | 59% → 59% |
| 03 | 1 | 6 | 53% → 57% | 59% → 60% |
| review gałęzi | 1 | 6 | 57% → 63% | 60% → 60% |
| **suma** | 5 | 26 | +19 | +2 |

Modele: `/ship` Sonnet, implementer Sonnet, weryfikator Haiku, recenzent ticketu Sonnet, review gałęzi Opus
(Standards, Spec i re-review). Wynik: 3 `done`, 0 `needs-info`.

Obok pomiaru nr 1 (ticket 10, `todo-comments`, backend w sandboksie: 5 rund, 34 minuty, 5h +22, tydzień +3):
ficzer MAUI z 3 ticketami w Koszyku kosztował mniej, choć jego DoD buduje dwie solucje (czas DoD zmierzony
przed pilotem w tickecie 23: 49,5 s przy 587 testach mobilnych; po pilocie testów mobilnych jest 611, czasu
w pilocie nikt nie mierzył). Różnica mieści się w zaokrągleniu procentów tygodnia do pełnych punktów, więc
oba pomiary mówią to samo: ok. 2–3% tygodnia na ficzer z 3 ticketami. Czego tabela nie obejmuje: planowania
(`/plan-feature` na Opusie; w pomiarze nr 1 zmierzone na ok. 2% tygodnia, tydzień z 57% do 59%; planowania
ficzera MAUI nikt nie zmierzył) i ewentualnej równoległej pracy w innych sesjach w tym samym oknie. Wniosek
do reguły z historii 48 (`docs/workflow.md`, „Reguła decyzji po pilotach”) należy do decyzji z ticketu 11:
przy ok. 5% tygodnia na ficzer razem z planem (plan z pomiaru nr 1) próg 2 ficzerów tygodniowo jest daleko.

**2026-11-05, rozjazdy pętli z przeglądu stanu (AC5):** trzy luki `/ship` (CI bez workflow, otwarte tickety
scalonego ficzera, niewidoczny brak tematu ntfy) i błąd Git Bash przy wznowieniu po scaleniu poprawione od
razu w tickecie 31 (plugin `1.3.1-dotnet.30`). Rozjazdy Koszyka z przebiegu 1 to jego tickety 04 i 05,
po triage'u 04 `ready-for-agent`, 05 `wontfix` (Koszyk `fbc74b3`). AC5 zostaje otwarte do końca pilota.
