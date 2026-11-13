# 24: Koszyk: CI na GitHub Actions z głową Androida

**What to build:** Workflow CI Koszyka buduje obie solucje (z głową `net10.0-android`) i uruchamia testy przy
pushu na `main` i przy PR. Plik, który nie trafił do commita, psuje CI.

**Blocked by:** 20

**Seams:** przebieg workflow na GitHubie po pushu (czytany przez przeglądarkę, jak bramka 2 krok 10).

**Status:** done

**Skąd:** ticket 11, decyzje 2 i 9 z bramki 2026-11-04 (historia 56). Koszyk dziś nie ma CI.

- [x] AC1: `.github/workflows/build.yml` na `ubuntu-latest`: SDK z `global.json`, `dotnet workload install
      maui-android`, build `Koszyk.sln` i `Koszyk.Mobile.sln` z `-warnaserror`, testy backendu, architektury
      i `Koszyk.Mobile.Tests`.
- [x] AC2: Pierwszy przebieg na `main` jest zielony. Czas przebiegu zapisany w `## Comments`.
- [x] AC3: Testy wymagające Dockera (jeśli są, ticket 23 AC2) działają na runnerze albo są świadomie pominięte
      z opisem w `## Comments`.

## Comments

**2026-11-13, zamknięcie.** AC2: pierwszy przebieg na `main` (push commita `b2d0ea8`) jest zielony, job `build`
trwał 4 min 50 s. Wynik odczytał użytkownik na stronie Actions (zrzut ekranu w sesji); build głowy
`net10.0-android` na runnerze przeszedł więc bez zmian w workflow.

**2026-11-13, workflow napisany, przed pierwszym przebiegiem.** Zmiany w Koszyku (commit `b2d0ea8`):
`.github/workflows/build.yml`, `tests/Koszyk.Mobile.Tests/Platform/WindowsOnlyFactAttribute.cs`, atrybut
na jednym teście w `MobileSmokeScriptTests.cs`, akapit **CI** w `AGENTS.md`.

- **AC1.** Jeden job na `ubuntu-latest`: checkout, SDK z `global.json`, JDK 17 (jak na hoście), `dotnet workload
  install maui-android`, a potem cztery komendy z `docs/agents/dod.json` Koszyka bez zmian (build obu solucji z
  `-warnaserror --no-incremental`, `dotnet test Koszyk.sln --no-build`, `dotnet test tests/Koszyk.Mobile.Tests
  --no-build`). Wyzwalacze: push na `main`, PR do `main`, `workflow_dispatch`.
- **AC3.** Żaden test nie wymaga Dockera: komendy testów przeszły w kontenerze `mcr.microsoft.com/dotnet/sdk:10.0`
  (SDK 10.0.404, bez Dockera w środku): 99, 5, 136 i 14 zielonych w `Koszyk.sln`, 646 zielonych i 1 pominięty w
  `Koszyk.Mobile.Tests`.
- **Odstępstwo: jeden test pominięty na Linuksie.** `MobileSmokeScriptTests.Screenshot_NearUniformFrame_IsNotRendered`
  używa `System.Drawing` w `pwsh`, które działa tylko na Windows, więc dostał `[WindowsOnlyFact]`. Na Windows nadal
  działa (14 z 14 testów klasy zielonych). Skrypt smoke i tak jest narzędziem hosta z emulatorem.
- **Niesprawdzone lokalnie:** build głowy `net10.0-android` na Linuksie (workload, Android SDK i JDK runnera).
  Pokaże to dopiero pierwszy przebieg.
- **AC2 czeka** na commit i push w Koszyku oraz na odczytanie przebiegu. Strona Actions prywatnego repo nie jest
  czytelna z przeglądarki panelu bez logowania, więc wynik i czas przebiegu odczytuje użytkownik.
