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
