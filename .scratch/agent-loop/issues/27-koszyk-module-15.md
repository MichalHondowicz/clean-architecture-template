# 27: Moduł 15 Koszyka jako ficzery pętli

**What to build:** Reszta modułu 15 („Polish & hardening”) idzie przez pętlę jako kilka ficzerów, każdy przez
`/plan-feature` i `/ship`. Decyzje użytkownika z `state-15` zostają wejściem do planowania.

**Blocked by:** 26

**Seams:** każdy ficzer według seam 1 ze speca; ten ticket tylko pilnuje podziału i kolejności.

**Status:** ready-for-human

**Skąd:** ticket 11, decyzja 8 z bramki 2026-11-04 (historia 63). Zakres i decyzje:
`D:\Projects\Koszyk\docs\tasks\state-15-mobile-polish-hardening.md` (2026-11-04), `handoff-15-polish-after-e2e.md`
§ 4–5 i `handoff-15-e2e-and-polish.md` § 4–5.

- [ ] AC1: Podział na ficzery z kolejnością, ustalony przy pierwszym `/plan-feature`: niezawodność sync i auth,
      motyw i branding, reszta kosmetyki UX (bez pozycji z pilota), backend i schemat, wystawienie API po TLS
      dla emulatora z E2E Release (moduły 13 § 1.3–1.4 i 14 § 14).
- [ ] AC2: Każdy ficzer doszedł do `main`. Ficzer, który nie zbiegł, ma brief `needs-info` i decyzję użytkownika.
- [ ] AC3: Po E2E Release roadmapa w `AGENTS.md` Koszyka ma 13, 14 i 15 jako ✅ (decyzja 1 ze `state-15`).

## Comments

**2026-11-15, z zamknięcia ticketu 26:**

- Ticket 26 jest `done`, więc ten ticket nie jest już zablokowany.
- Do sprawdzenia przy pierwszym przebiegu `/ship` (`mobile-smoke-hardening`, plan w Koszyku `3f2f371`): seam
  „CI po scaleniu”. Żaden przebieg pilota 2 go nie sprawdził, bo workflow CI Koszyka powstał 2026-11-13.
- AC1: pozycja „wystawienie API po TLS dla emulatora z E2E Release” jest zrobiona (`emulator-tls-release-e2e`
  scalony 2026-11-11, E2E Release zaliczone 2026-11-15). Doszedł ficzer `mobile-smoke-hardening`. Reszta zakresu:
  sync i auth, motyw i branding, `ux-cosmetics-2`, backend i schemat (handoff Koszyka
  `docs/tasks/handoff-15-smoke-hardening-and-next-feature.md`, sekcja 4, pytanie 4).
- AC3: moduły 13 i 14 mają ✅ w roadmapie Koszyka od 2026-11-15 (`a0b236c`); 15 jeszcze nie.

**2026-11-15 (noc), przebieg `mobile-smoke-hardening` z historii Koszyka** (zapis agenta z repozytorium Koszyka,
`main` `9db2b11`, bez raportu sesji `/ship`; źródła: `git log`, reflog `origin/main`,
`.scratch/mobile-smoke-hardening/metrics.md`, `brief.md` i statusy ticketów):

- Plan `3f2f371` 19:11, `/ship` 20:52–22:08: tickety 01–05 `done` (01 po 2 rundach, reszta po 1), review gałęzi
  (standardy i spec na Opusie) z 1 rundą poprawek, brief 22:08. Bramka 2 „OK, 1 tak, 2 tak” 22:14 (`9db2b11`),
  `origin/main` przesunięty pushem o 22:14:45, czyli 23 s po commicie bramki: scalił i wypchnął krok `/ship`.
  Worktree i gałąź `feat/mobile-smoke-hardening` są usunięte.
- Testy z briefu: Unit 99, Architecture 5, AppHost 15, Integration 136, Koszyk.Mobile.Tests 661; 0 ostrzeżeń.
  Smoke `mobile-login` zielony (Pixel_6a_B, Release). Diff poza `.scratch`: 8 plików, +403/−29.
- Nowe tickety Koszyka `needs-triage`: 06 (strażnik konwencji nie widzi wywołań przez zmienną), 07 (`taskkill`
  w `mobile-smoke.ps1` bez limitu czasu).
- Pauzy na limit nie było: historia nie ma commita `ship: pause`, okno 5h skończyło na 66%.

| Etap | Rundy | Minuty | Okno 5h | Tydzień |
|---|---|---|---|---|
| Tickety 01–05 | 6 | 55 | 4% → 50% (+46) | 30% → 36% (+6) |
| Review gałęzi | 1 | 17 | 50% → 66% (+16) | 36% → 38% (+2) |
| Razem | 7 | 72 | +62 | +8 |

Bez planu ficzer kosztował 8 pp tygodnia: więcej niż przebieg 2 pilota (4 pp) i mniej niż próg z historii 48.
Wiersze nie obejmują pauzy, więc mierzą koszt.

Co ten przebieg mówi o kryteriach:

- **Seam „CI po scaleniu”.** To pierwsze scalenie przez `/ship` przy istniejącym workflow CI Koszyka (`build.yml`).
  Review gałęzi wyłapał test, który byłby czerwony na ubuntu (`cmd /c start /b` z `[Fact]`), i zmienił go na
  `[WindowsOnlyFact]` przed scaleniem (`c227c1a`). Wyniku CI dla `9db2b11` z repozytorium nie widać; seam zostaje
  otwarty do odpowiedzi użytkownika.
- **AC1.** Bez zmian: `mobile-smoke-hardening` doszedł jako ficzer spoza pierwotnej listy; następnego ficzera
  modułu 15 Koszyk jeszcze nie zaplanował (brak nowego commita `plan:`).
- **AC2.** `mobile-smoke-hardening` doszedł do `main` bez briefu `needs-info`. Z zakresu modułu 15 w `main` są:
  `ux-cosmetics-1`, `emulator-tls-release-e2e`, `mobile-smoke-emulator`, `mobile-smoke-hardening`. Zostają: sync
  i auth, motyw i branding, `ux-cosmetics-2`, backend i schemat.
- **AC3.** Bez zmian: 15 nie ma ✅.

Ticket zostaje `ready-for-human`.

**2026-11-15 (noc), odpowiedź użytkownika:** CI Koszyka po scaleniu `mobile-smoke-hardening` (`9db2b11`) jest
zielone. Seam „CI po scaleniu” jest sprawdzony: pierwszy przebieg `/ship` z workflow CI Koszyka doszedł do `main`
z zielonym CI na ubuntu.
