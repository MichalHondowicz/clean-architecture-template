# 23: Koszyk: pętla bez nadzoru (DoD, kontrakt, hooki)

**What to build:** Koszyk ma wszystko, czego `/plan-feature` i `/ship` potrzebują na starcie: `docs/agents/dod.json`
z komendami Koszyka, kontrakt implementera, kopie hooków z template'u i ich rejestrację. Według `docs/workflow.md`
template'u, „Istniejący projekt”, krok 5.

**Blocked by:** 21, 22

**Seams:** `run-dod.ps1 -Check environment` i `-Check commands` w Koszyku jako proces; rejestracja hooków
sprawdzana w pilocie (ticket 26).

**Status:** ready-for-agent

**Skąd:** ticket 11, decyzje 4 i 6 z bramki 2026-11-04 (historie 53, 54, 63).

- [ ] AC1: `docs/agents/dod.json`: `commands` budują `Koszyk.sln` i `Koszyk.Mobile.sln` z `-warnaserror` (głowa
      `net10.0-android` ze skompilowanymi bindingami) i uruchamiają testy backendu, architektury i
      `Koszyk.Mobile.Tests`. `run-dod.ps1 -Check commands` jest zielony na obecnym `main`.
- [ ] AC2: `environmentChecks`: Docker tylko wtedy, gdy testy go wymagają. Ustalenie sprawdzone uruchomieniem
      testów przy zatrzymanym Dockerze i zapisane w `## Comments`.
- [ ] AC3: `protectedPaths`: `AGENTS.md`, `CLAUDE.md`, `docs/adr/**`, `docs/agents/**`, `.claude/**`,
      `tests/Koszyk.Tests.Architecture/**`. `quota` jak w template'cie (80 i 90).
- [ ] AC4: `screenPaths`: XAML w `src/Koszyk.Mobile/**`, `MauiProgram.cs`, `Koszyk.Mobile.csproj` i
      `Platforms/Android/AndroidManifest.xml`, żeby brief wymieniał pliki do sprawdzenia na emulatorze.
- [ ] AC5: `docs/agents/implementer-contract.md` dostosowany do Koszyka (komendy, wyjątki zamiast `Result`,
      dowód czerwieni), bez reguł template'u, które Koszyka nie dotyczą.
- [ ] AC6: `.claude/hooks/` to kopie skryptów z template'u (`dod-common`, `dod-gate`, `guard-protected-paths`,
      `run-dod`, `check-diff`, `wait-for-reset`, `check-reboot`, `notify`) z commitem template'u, z którego
      pochodzą, zapisanym w `## Comments`. `docs/workflow.md` template'u, „Utrzymanie”, opisuje synchronizację.
- [ ] AC7: Rejestracja hooków (`PreToolUse`, `SubagentStop`, `Notification`) i allowlista `/ship` w
      `.claude/settings.json` Koszyka: dopisuje użytkownik (tryb auto blokuje edycję), według gotowego wzoru
      w `## Comments` tego ticketu.
