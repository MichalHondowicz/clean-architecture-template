# 22: Koszyk: testy architektury

**What to build:** Projekt `tests/Koszyk.Tests.Architecture` (NetArchTest) pilnuje reguły zależności z
`AGENTS.md` Koszyka i reguły, że `Koszyk.Mobile.Core` nie zależy od MAUI. Pętla chroni go jako ścieżkę, więc
implementer nie osłabi reguł.

**Blocked by:** None

**Seams:** testy architektury jako proces (`dotnet test tests/Koszyk.Tests.Architecture`).

**Status:** ready-for-agent

**Skąd:** ticket 11, decyzja 5 z bramki 2026-11-04 (historia 50). Koszyk dziś nie ma testów architektury.

- [ ] AC1: `Koszyk.Core` nie zależy od żadnego innego projektu Koszyka
      (test: `LayerTests.Core_Should_NotDependOnOtherLayers`).
- [ ] AC2: `Koszyk.Application` zależy tylko od `Core`, `Infrastructure` nie zależy od `Api`
      (testy: `LayerTests.Application_Should_NotDependOnInfrastructureOrApi`,
      `LayerTests.Infrastructure_Should_NotDependOnApi`).
- [ ] AC3: `Koszyk.Mobile.Core` nie zależy od `Microsoft.Maui` ani od projektów backendu
      (test: `MobileTests.MobileCore_Should_NotDependOnMaui`, `MobileTests.MobileCore_Should_NotDependOnBackend`).
- [ ] AC4: Każda reguła ma dowód czerwieni (tymczasowa zależność, test czerwony, przywrócenie).
- [ ] AC5: Projekt jest w solucji, którą obejmują komendy DoD (ticket 23), i przechodzi na obecnym kodzie.
