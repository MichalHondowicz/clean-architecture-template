# 23: Koszyk: pętla bez nadzoru (DoD, kontrakt, hooki)

**What to build:** Koszyk ma wszystko, czego `/plan-feature` i `/ship` potrzebują na starcie: `docs/agents/dod.json`
z komendami Koszyka, kontrakt implementera, kopie hooków z template'u i ich rejestrację. Według `docs/workflow.md`
template'u, „Istniejący projekt”, krok 5.

**Blocked by:** 21, 22

**Seams:** `run-dod.ps1 -Check environment` i `-Check commands` w Koszyku jako proces; rejestracja hooków
sprawdzana w pilocie (ticket 26).

**Status:** done

**Skąd:** ticket 11, decyzje 4 i 6 z bramki 2026-11-04 (historie 53, 54, 63).

- [x] AC1: `docs/agents/dod.json`: `commands` budują `Koszyk.sln` i `Koszyk.Mobile.sln` z `-warnaserror` (głowa
      `net10.0-android` ze skompilowanymi bindingami) i uruchamiają testy backendu, architektury i
      `Koszyk.Mobile.Tests`. `run-dod.ps1 -Check commands` jest zielony na obecnym `main`.
- [x] AC2: `environmentChecks`: Docker tylko wtedy, gdy testy go wymagają. Ustalenie sprawdzone uruchomieniem
      testów przy zatrzymanym Dockerze i zapisane w `## Comments`.
- [x] AC3: `protectedPaths`: `AGENTS.md`, `CLAUDE.md`, `docs/adr/**`, `docs/agents/**`, `.claude/**`,
      `tests/Koszyk.Tests.Architecture/**`. `quota` jak w template'cie (80 i 90).
- [x] AC4: `screenPaths`: XAML w `src/Koszyk.Mobile/**`, `MauiProgram.cs`, `Koszyk.Mobile.csproj` i
      `Platforms/Android/AndroidManifest.xml`, żeby brief wymieniał pliki do sprawdzenia na emulatorze.
- [x] AC5: `docs/agents/implementer-contract.md` dostosowany do Koszyka (komendy, wyjątki zamiast `Result`,
      dowód czerwieni), bez reguł template'u, które Koszyka nie dotyczą.
- [x] AC6: `.claude/hooks/` to kopie skryptów z template'u (`dod-common`, `dod-gate`, `guard-protected-paths`,
      `run-dod`, `check-diff`, `wait-for-reset`, `check-reboot`, `notify`) z commitem template'u, z którego
      pochodzą, zapisanym w `## Comments`. `docs/workflow.md` template'u, „Utrzymanie”, opisuje synchronizację.
- [x] AC7: Rejestracja hooków (`PreToolUse`, `SubagentStop`, `Notification`) i allowlista `/ship` w
      `.claude/settings.json` Koszyka: dopisuje użytkownik (tryb auto blokuje edycję), według gotowego wzoru
      w `## Comments` tego ticketu.

## Comments

**2026-11-04, bramka (decyzja użytkownika: „Tak, 1–6”):** `environmentChecks: []` po przebiegu bez Dockera;
build z `-warnaserror --no-incremental`; kolejność backend, potem mobile; `.claude/settings.json` tworzy
użytkownik według wzoru niżej; kontrakt dostosowany do Koszyka; `screenPaths` z `src/Koszyk.Mobile/*.xaml`
i code-behind `*.xaml.cs` (rozszerzenie AC4).

**2026-11-04, wynik:** Koszyk, gałąź `chore/agent-loop`, commit `0ccbedb`; template: `docs/workflow.md`,
„Utrzymanie”.

- **AC1.** `docs/agents/dod.json`, cztery komendy po kolei: `dotnet build Koszyk.sln -warnaserror --no-incremental`,
  `dotnet test Koszyk.sln --no-build` (Unit 99, Architecture 5, Integration 130),
  `dotnet build Koszyk.Mobile.sln -warnaserror --no-incremental` (głowa `net10.0-android` z bindingami),
  `dotnet test tests/Koszyk.Mobile.Tests --no-build` (587). `run-dod.ps1 -Check commands` w Koszyku:
  `{"ok":true}`, 49,5 s. Gałąź różni się od `main` (`8251296`) w `src/` i `tests/` tylko projektem
  `Koszyk.Tests.Architecture` z ticketu 22, więc kod produktu jest ten sam co na `main`; na samym `main` testów
  architektury jeszcze nie ma.
  - **Dlaczego `--no-incremental`.** Sprawdzone tymczasowym plikiem z `#warning` w `Koszyk.Core` (usunięty):
    zwykły `dotnet build` (1 ostrzeżenie, exit 0), potem `dotnet build -warnaserror` dał exit 0 i 0 ostrzeżeń,
    bo kompilacja była pominięta. Tak samo `-p:TreatWarningsAsErrors=true`. Dopiero `--no-incremental` dał
    exit 1. Bez tego ostrzeżenie zostawione przez wcześniejszy build implementera przeszłoby przez DoD.
  - **Czasy pełnego buildu:** backend 4 s, mobile 41 s. Pierwszy pełny build mobilny zawiesił się na 12 minut
    na żądaniu do serwera kompilatora (VBCSCompiler nie odpowiadał na named pipe), drugi na świeżym serwerze
    przeszedł w 41 s. `dod-gate` ma limit 1800 s, więc taki zawis kończy się błędem bramki, a nie wiszącą pętlą.
- **AC2.** `"environmentChecks": []`. W `tests/` nie ma Testcontainers ani `Aspire.Hosting.Testing`
  (`git grep` pusty); `KoszykWebApplicationFactory` podmienia `KoszykDbContext` na EF InMemory. Przebieg przy
  zatrzymanym Docker Desktop (`docker desktop stop`, `docker info` exit 1 przed i po testach): oba buildy
  0 ostrzeżeń, testy 99, 5, 130 i 587 zielone. Docker włączony z powrotem, kontenery `unless-stopped` wróciły.
  `run-dod.ps1 -Check environment`: `{"ok":true,"failed":[],"started":[]}`. `AGENTS.md` Koszyka mówił
  „integracyjne wymagają Dockera”; poprawione.
- **AC3.** `protectedPaths` jak w AC, `quota` 80 i 90. Sprawdzone hookiem `guard-protected-paths.ps1` w Koszyku
  ze zdarzeniem implementera: `deny` dla `AGENTS.md`, `CLAUDE.md`, `docs/adr/0006-x.md`, `docs/agents/dod.json`,
  `.claude/settings.json`, `tests/Koszyk.Tests.Architecture/LayerTests.cs`; `allow` dla pliku w `src/Koszyk.Core`,
  `tests/Koszyk.Mobile.Tests` i `.scratch/`. Koszyk nie ma `.editorconfig` ani `Directory.Build.props`, więc
  tych ścieżek z template'u nie ma na liście.
- **AC4.** `screenPaths`: `src/Koszyk.Mobile/*.xaml`, `src/Koszyk.Mobile/*.xaml.cs`, `MauiProgram.cs`,
  `Koszyk.Mobile.csproj`, `Platforms/Android/AndroidManifest.xml` (pełne ścieżki w pliku). `/ship` podaje je
  jako pathspec do `git diff --name-only`. Pathspec bez magii `:(glob)` dopasowuje `*` także przez `/`, więc
  `src/Koszyk.Mobile/*.xaml` łapie wszystkie 11 plików XAML, a `src/Koszyk.Mobile/**/*.xaml` tylko 9 (gubi
  `App.xaml` i `AppShell.xaml`). Razem 25 plików (`git ls-files` z tymi wzorcami).
- **AC5.** `docs/agents/implementer-contract.md`: wejście, wyjście i zasady z template'u, do tego komendy
  Koszyka (sekcja „Commands”, z powodem `--no-incremental`), `KoszykException` zamiast typu wyniku (ADR 0001),
  test czerwony najpierw, commity backendu osobno od mobile i docs, zmiany w `screenPaths` wypisane
  w `## Comments` do sprawdzenia na emulatorze, kontrola `git diff` po `dotnet sln add`. Bez Testcontainers,
  `ca-review`, `Result` i skilli template'u. `AGENTS.md` i `docs/agents/issue-tracker.md` Koszyka nie mówią już,
  że tych plików brak; `AGENTS.md` mówi, że hooki to kopie z template'u.
- **AC6.** `.claude/hooks/` Koszyka: wszystkie osiem skryptów z template'u z commitu **`7629132`** (od niego
  `.claude/hooks/` template'u się nie zmieniło). Identyfikatory blobów w `0ccbedb` i w `7629132` są równe dla
  każdego pliku. „Utrzymanie” w `docs/workflow.md` template'u opisuje synchronizację: źródło i testy w template'cie,
  kopia wszystkich skryptów, commit źródłowy w commicie projektu, sprawdzenie blobów i `run-dod.ps1`, wpis
  w `settings.json` dla nowego skryptu.
- **AC7 (dla użytkownika).** Utwórz `D:\Projects\Koszyk\.claude\settings.json` z treścią niżej i zacommituj
  go na `chore/agent-loop`. Plik nie jest ignorowany (ignorowane są tylko `settings.local.json` i
  `scheduled_tasks.lock`). Allowlista to wzór template'u bez wpisów Dockera i `dotnet tool restore` (Koszyk
  ich nie używa), z wpisami `check-reboot.ps1` (ticket 16) i `notify.ps1` (ticket 12). `git push origin main`
  i `git fetch origin` zadziałają dopiero z remote z ticketu 20. Hook `Notification` wysyła coś tylko przy
  ustawionej `CLAUDE_NTFY_URL`. Po zapisaniu sprawdzę plik na dysku.

  ```json
  {
    "permissions": {
      "allow": [
        "Bash(dotnet build*)",
        "Bash(dotnet test*)",
        "Bash(dotnet ef migrations *)",
        "Bash(dotnet restore*)",
        "Bash(dotnet format*)",
        "Bash(dotnet list*)",
        "Bash(dotnet nuget why*)",
        "Bash(sha256sum*)",
        "Bash(git status*)",
        "Bash(git diff*)",
        "Bash(git log*)",
        "Bash(git add *)",
        "Bash(git commit *)",
        "Bash(date*)",
        "Bash(git rev-parse*)",
        "Bash(git merge-base*)",
        "Bash(git ls-files*)",
        "Bash(git show*)",
        "Bash(git worktree *)",
        "Bash(git switch *)",
        "Bash(git merge --ff-only *)",
        "Bash(git push origin main)",
        "Bash(git fetch origin)",
        "Bash(git fetch . feat/*:main)",
        "Bash(git update-ref -d refs/heads/feat/*)",
        "Bash(pwsh -NoProfile -NonInteractive -File .claude/hooks/run-dod.ps1 *)",
        "Bash(pwsh -NoProfile -NonInteractive -File .claude/hooks/check-diff.ps1 *)",
        "Bash(pwsh -NoProfile -NonInteractive -File .claude/hooks/wait-for-reset.ps1 *)",
        "Bash(pwsh -NoProfile -NonInteractive -File .claude/hooks/check-reboot.ps1)",
        "Bash(pwsh -NoProfile -NonInteractive -File .claude/hooks/notify.ps1 *)",
        "PowerShell(dotnet build*)",
        "PowerShell(dotnet test*)",
        "PowerShell(dotnet ef migrations *)",
        "PowerShell(dotnet restore*)",
        "PowerShell(dotnet format*)",
        "PowerShell(dotnet list*)",
        "PowerShell(dotnet nuget why*)",
        "PowerShell(Get-FileHash*)",
        "PowerShell(git status*)",
        "PowerShell(git diff*)",
        "PowerShell(git log*)",
        "PowerShell(git add *)",
        "PowerShell(git commit *)",
        "PowerShell(Get-Date*)",
        "PowerShell(git rev-parse*)",
        "PowerShell(git merge-base*)",
        "PowerShell(git ls-files*)",
        "PowerShell(git show*)",
        "PowerShell(git worktree *)",
        "PowerShell(git switch *)",
        "PowerShell(git merge --ff-only *)",
        "PowerShell(git push origin main)",
        "PowerShell(git fetch origin)",
        "PowerShell(git fetch . feat/*:main)",
        "PowerShell(git update-ref -d refs/heads/feat/*)",
        "PowerShell(pwsh -NoProfile -NonInteractive -File .claude/hooks/run-dod.ps1 *)",
        "PowerShell(pwsh -NoProfile -NonInteractive -File .claude/hooks/check-diff.ps1 *)",
        "PowerShell(pwsh -NoProfile -NonInteractive -File .claude/hooks/wait-for-reset.ps1 *)",
        "PowerShell(pwsh -NoProfile -NonInteractive -File .claude/hooks/check-reboot.ps1)",
        "PowerShell(pwsh -NoProfile -NonInteractive -File .claude/hooks/notify.ps1 *)"
      ],
      "deny": [
        "Bash(git diff *--output*)",
        "Bash(git log *--output*)",
        "Bash(git show *--output*)",
        "PowerShell(git diff *--output*)",
        "PowerShell(git log *--output*)",
        "PowerShell(git show *--output*)"
      ]
    },
    "hooks": {
      "PreToolUse": [
        {
          "matcher": "Edit|Write",
          "hooks": [
            {
              "type": "command",
              "command": "pwsh -NoProfile -NonInteractive -File \"${CLAUDE_PROJECT_DIR}/.claude/hooks/guard-protected-paths.ps1\""
            }
          ]
        }
      ],
      "SubagentStop": [
        {
          "hooks": [
            {
              "type": "command",
              "command": "pwsh -NoProfile -NonInteractive -File \"${CLAUDE_PROJECT_DIR}/.claude/hooks/dod-gate.ps1\"",
              "timeout": 1800
            }
          ]
        }
      ],
      "Notification": [
        {
          "matcher": "permission_prompt",
          "hooks": [
            {
              "type": "command",
              "command": "pwsh -NoProfile -NonInteractive -File \"${CLAUDE_PROJECT_DIR}/.claude/hooks/notify.ps1\""
            }
          ]
        }
      ]
    },
    "enabledPlugins": {
      "superpowers@claude-plugins-official": false
    }
  }
  ```

**2026-11-04, review Spec** (`mattpocock-skills:spec-reviewer`): `pass`, bez findings. Recenzent sprawdził sam
`Get-DodProblems`, dopasowanie `screenPaths` (25 plików, `**/*.xaml` daje 9), bloby hooków względem `7629132`,
twierdzenia kontraktu w kodzie Koszyka, wzór `settings.json` względem template'u i ticketów 12 i 16 oraz różnicę
gałęzi względem `main`. Dwie uwagi poniżej progu bez zmian: kontrakt wymienia wyciszanie przez `.editorconfig`,
którego Koszyk nie ma (reguła zostaje na wypadek, gdyby plik doszedł); typecheck w `AGENTS.md` to zwykły build,
a `--no-incremental` dokłada dopiero DoD, co kontrakt wyjaśnia. Status `ready-for-human` do czasu AC7.

**2026-11-04, AC7:** użytkownik utworzył `D:\Projects\Koszyk\.claude\settings.json` z wzoru wyżej. Sprawdzone na
dysku: poprawny JSON, 60 wpisów `allow`, 6 `deny`, hooki `PreToolUse`, `SubagentStop`, `Notification`; hash równy
przygotowanemu plikowi. Commit w Koszyku `968d9fb`. Potem `main` Koszyka przesunięty fast-forwardem na
`chore/agent-loop` (`968d9fb`, tickety 21–25 i ten plik) i wybrany w checkoucie, bo `/plan-feature` startuje
z `main`. Rejestrację hooków w działaniu sprawdzi pilot (ticket 26).
