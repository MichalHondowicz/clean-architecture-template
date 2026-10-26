# 08: `/plan-feature`: planowanie z jednym zatwierdzeniem

**What to build:** Jedną komendą przechodzę grill, spec i tickety w jednej sesji na Opusie. Na końcu
zatwierdzam jeden pakiet, a tickety mają format, który `/ship` wykonuje bez pytań. Po zatwierdzeniu agent
proponuje start pętli.

**Blocked by:** 03

**Seams:** pilot (ticket 10): planowanie ficzera pilota.

**Status:** ready-for-agent

- [ ] `/plan-feature` jest tylko dla człowieka i łączy w jednej sesji grill (z zapisem do `GLOSSARY.md`
      i ADR-ów), spec i tickety (historia 1).
- [ ] Planowanie działa na Opusie (historia 6).
- [ ] Każdy ticket ma sekcję `Seams:` wyprowadzoną z ADR 0004 i kryteria `AC1…n` z planowaną nazwą testu
      (historie 3, 4).
- [ ] Jedna decyzja zatwierdza cały pakiet (spec, tickety, seamy, kryteria, DoD), a po niej tickety mają
      `ready-for-agent` (historia 2).
- [ ] Po zatwierdzeniu agent proponuje uruchomienie `/ship <feature>` (historia 7).
- [ ] `grill-with-docs`, `to-spec` i `to-tickets` dalej działają osobno.
- [ ] Wersja forka podbita.
