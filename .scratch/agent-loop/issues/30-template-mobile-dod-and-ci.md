# 30: Template: DoD mobilny i build Androida w CI

**What to build:** DoD template'u obejmuje głowę `net10.0-android` i testy `Mobile.UnitTests`, brief wymienia
zmienione ekrany, a CI buduje głowę Androida obok backendu.

**Blocked by:** 28

**Seams:** `run-dod.ps1 -Check commands` jako proces; przebieg CI na GitHubie po pushu na `main`.

**Status:** wontfix

**Skąd:** ticket 11, decyzja 9 z bramki 2026-11-04 (historie 53, 54, 56).

- [ ] AC1: `docs/agents/dod.json` buduje `CleanArchitecture.slnx` z głową Androida i uruchamia
      `Mobile.UnitTests` w pełnych testach; `run-dod.ps1 -Check commands` zielony.
- [ ] AC2: `screenPaths` w `dod.json` obejmuje XAML w `src/Mobile/**`, `MauiProgram.cs`, csproj głowy i manifest.
- [ ] AC3: `.github/workflows/build.yml` na `ubuntu-latest` instaluje workload `maui-android` i buduje całą
      solucję z głową Androida. Przebieg na `main` zielony, czas zapisany w `## Comments`.
- [ ] AC4: Sandbox `D:\Projects\ship-sandbox` dostaje ten stan (merge z `main` template'u, kopie hooków
      z ticketów 12, 16, 17), żeby kolejne przebiegi tam miały warstwę mobilną.

## Comments

**2026-11-11, decyzja użytkownika (przegląd workflow):** `wontfix` na teraz. Warstwa MAUI w template'cie nie ma dziś
drugiego odbiorcy, a Koszyk ma już `add-screen`, DoD mobilne i konwencje. Wraca jako nowy plan (`/plan-feature`),
gdy ruszy drugi projekt MAUI.
