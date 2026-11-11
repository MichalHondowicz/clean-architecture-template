# 28: Template: warstwa MAUI

**What to build:** Template jest monorepo z warstwą mobilną: `Mobile` (MAUI, `net10.0-android`, tylko XAML i kod
platform), `Mobile.Core` (`net10.0`, CommunityToolkit.Mvvm, typowany klient HTTP), `Contracts` (DTO współdzielone
z Web.Api) i `tests/Mobile.UnitTests`, wszystko w `CleanArchitecture.slnx`. Przykładowa aplikacja: logowanie
i lista todo. Układ i wzorce bierze z Koszyka po pilocie 2.

**Blocked by:** 26

**Seams:** build `CleanArchitecture.slnx` (z głową Androida) i testy `Mobile.UnitTests` jako proces; testy
architektury; integracja `Contracts` przez istniejące testy integracyjne Web.Api.

**Status:** wontfix

**Skąd:** ticket 11, decyzja 9 z bramki 2026-11-04 (historie 49, 50, 51). Koszyk nie ma projektu `Contracts`,
więc ten element jest nowy w template'cie i do Koszyka nie wraca.

- [ ] AC1: Projekty `Mobile`, `Mobile.Core`, `Contracts`, `tests/Mobile.UnitTests` są w `CleanArchitecture.slnx`,
      a `dotnet build CleanArchitecture.slnx` buduje głowę `net10.0-android` z `x:DataType` na każdej stronie.
- [ ] AC2: `Contracts` trzyma DTO todo i logowania, używane przez Web.Api i `Mobile.Core`. Testy integracyjne
      Web.Api przechodzą bez zmian zachowania.
- [ ] AC3: `Mobile.Core` ma ViewModele logowania i listy todo z typowanym klientem i obsługą tokenu, z testami
      w `Mobile.UnitTests` (xUnit, Shouldly, NSubstitute; klient HTTP podstawiony).
- [ ] AC4: Testy architektury: `Mobile.Core` nie zależy od MAUI ani od warstw backendu, a `Contracts` od niczego
      (test: `LayerTests.MobileCore_Should_NotDependOnMaui` i pozostałe nazwy w tym stylu). Z dowodem czerwieni.
- [ ] AC5: Nowe projekty mają te same reguły analizatorów co backend (`AnalysisMode=All`, ostrzeżenia jako
      błędy, Sonar) bez żadnego wyciszenia. Reguła, której głowa MAUI nie może spełnić, wraca do użytkownika
      jako decyzja, nie jako `NoWarn`.
- [ ] AC6: Build i pełne testy zielone, Central Package Management dla nowych pakietów.

## Comments

**2026-11-11, decyzja użytkownika (przegląd workflow):** `wontfix` na teraz. Warstwa MAUI w template'cie nie ma dziś
drugiego odbiorcy, a Koszyk ma już `add-screen`, DoD mobilne i konwencje. Wraca jako nowy plan (`/plan-feature`),
gdy ruszy drugi projekt MAUI.
