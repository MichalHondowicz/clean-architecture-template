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
