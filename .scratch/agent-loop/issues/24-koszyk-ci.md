# 24: Koszyk: CI na GitHub Actions z głową Androida

**What to build:** Workflow CI Koszyka buduje obie solucje (z głową `net10.0-android`) i uruchamia testy przy
pushu na `main` i przy PR. Plik, który nie trafił do commita, psuje CI.

**Blocked by:** 20

**Seams:** przebieg workflow na GitHubie po pushu (czytany przez przeglądarkę, jak bramka 2 krok 10).

**Status:** ready-for-agent

**Skąd:** ticket 11, decyzje 2 i 9 z bramki 2026-11-04 (historia 56). Koszyk dziś nie ma CI.

- [ ] AC1: `.github/workflows/build.yml` na `ubuntu-latest`: SDK z `global.json`, `dotnet workload install
      maui-android`, build `Koszyk.sln` i `Koszyk.Mobile.sln` z `-warnaserror`, testy backendu, architektury
      i `Koszyk.Mobile.Tests`.
- [ ] AC2: Pierwszy przebieg na `main` jest zielony. Czas przebiegu zapisany w `## Comments`.
- [ ] AC3: Testy wymagające Dockera (jeśli są, ticket 23 AC2) działają na runnerze albo są świadomie pominięte
      z opisem w `## Comments`.
