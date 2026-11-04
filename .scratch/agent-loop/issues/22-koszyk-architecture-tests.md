# 22: Koszyk: testy architektury

**What to build:** Projekt `tests/Koszyk.Tests.Architecture` (NetArchTest) pilnuje reguły zależności z
`AGENTS.md` Koszyka i reguły, że `Koszyk.Mobile.Core` nie zależy od MAUI. Pętla chroni go jako ścieżkę, więc
implementer nie osłabi reguł.

**Blocked by:** None

**Seams:** testy architektury jako proces (`dotnet test tests/Koszyk.Tests.Architecture`).

**Status:** done

**Skąd:** ticket 11, decyzja 5 z bramki 2026-11-04 (historia 50). Koszyk dziś nie ma testów architektury.

- [x] AC1: `Koszyk.Core` nie zależy od żadnego innego projektu Koszyka
      (test: `LayerTests.Core_Should_NotDependOnOtherLayers`).
- [x] AC2: `Koszyk.Application` zależy tylko od `Core`, `Infrastructure` nie zależy od `Api`
      (testy: `LayerTests.Application_Should_NotDependOnInfrastructureOrApi`,
      `LayerTests.Infrastructure_Should_NotDependOnApi`).
- [x] AC3: `Koszyk.Mobile.Core` nie zależy od `Microsoft.Maui` ani od projektów backendu
      (test: `MobileTests.MobileCore_Should_NotDependOnMaui`, `MobileTests.MobileCore_Should_NotDependOnBackend`).
- [x] AC4: Każda reguła ma dowód czerwieni (tymczasowa zależność, test czerwony, przywrócenie).
- [x] AC5: Projekt jest w solucji, którą obejmują komendy DoD (ticket 23), i przechodzi na obecnym kodzie.

## Comments

**2026-11-04, wynik:** Koszyk, gałąź `chore/agent-loop`, commity `1864e05` i `5c347fe`.

- `tests/Koszyk.Tests.Architecture` w `Koszyk.sln` (bez `Koszyk.Mobile.Core` w tej solucji: referencja projektu
  wystarcza, a `AGENTS.md` trzyma projekty mobilne w `Koszyk.Mobile.sln`). Testy: `LayerTests` (3) i
  `MobileTests` (2), nazwy jak w AC.
- `ServiceDefaults` ma rozszerzenia w `Microsoft.Extensions.Hosting`, a `AppHost` nie ma przestrzeni nazw,
  więc testy warstw sprawdzają je przez referencje assembly, a nie przez przestrzeń nazw.
- Dowody czerwieni (skrypt, każda zmiana cofnięta): referencja Core, Application i Infrastructure →
  `Koszyk.Mobile.Core` z typem z niej czerwieni trzy testy warstw. Typ w przestrzeni `Microsoft.Maui.RedProof`
  używany w `Mobile.Core` czerwieni test MAUI. Referencja `Mobile.Core` → `Koszyk.Core` czerwieni test
  backendu. Referencja `Application` → `ServiceDefaults` czerwieni test Application. Zależności typu
  Core → Application nie da się zasymulować, bo graf projektów zrobiłby się cykliczny, więc pilnuje ich już
  kompilator.
- Build obu solucji 0/0; testy 99, 5, 130, 587.
- Review Spec (Sonnet), runda 1: jedna uwaga (Application i Infrastructure bez `ServiceDefaults` i `AppHost`),
  poprawiona w `5c347fe`, re-review `pass`.
