# 01: Plik DoD i blokada zakończenia implementera

**What to build:** Projekt ma jeden plik DoD. Z niego hooki i weryfikator biorą komendy (build, pełne testy),
wymaganie Dockera, nazwę roli implementera, listę chronionych ścieżek i progi limitu. Gdy sub-agent
implementera kończy pracę, a którakolwiek komenda DoD się nie powiodła, zakończenie jest zablokowane
i implementer dostaje treść porażki. Inne role i główna sesja kończą bez blokady. To pierwszy kawałek pętli,
który działa samodzielnie: nawet ręcznie wywołany implementer nie ogłosi sukcesu na czerwonym DoD.

**Blocked by:** None (can start immediately)

**Seams:** skrypt hooka jako proces. Wejście: JSON zdarzenia hooka na stdin i plik DoD w repozytorium
przygotowanym w katalogu tymczasowym (komendy DoD w teście mogą być trywialnymi komendami z ustalonym kodem
wyjścia). Wyjście: kod wyjścia i decyzja według protokołu hooków Claude Code. Testy xUnit w nowym projekcie
testów hooków.

**Status:** done

- [x] Plik DoD projektu zawiera komendę buildu, komendę pełnych testów, wymaganie Dockera, nazwę roli
      implementera, listę chronionych ścieżek i progi limitu (tygodniowy 80%, okno 5-godzinne 90%). Fork
      skilli nie zawiera żadnej z tych komend (historie 42, 59).
- [x] Hook `SubagentStop`, zarejestrowany w ustawieniach projektu, blokuje zakończenie implementera przy
      niezerowym kodzie wyjścia którejkolwiek komendy DoD i podaje treść porażki. W tym projekcie obejmuje to
      błąd buildu, ostrzeżenie buildu (analizatory są błędami) i czerwony test (historie 21, 23).
- [x] Ten sam hook przepuszcza zakończenie implementera, gdy wszystkie komendy DoD są zielone (historia 21).
- [x] Hook blokuje także przy powtórnej próbie (`stop_hook_active`). Nieskończoną pętlę ogranicza `maxTurns`
      roli implementera, nie hook.
- [x] Hook przepuszcza zakończenie, gdy `agent_type` nie jest rolą implementera z pliku DoD (weryfikator,
      review, inne sub-agenty).
- [x] Gdy Docker jest wymagany i nie odpowiada, hook nie blokuje, tylko zwraca informację o awarii
      środowiska. Rozstrzyga orkiestrator (ticket 03).
- [x] Hook czyta komendy i nazwę roli z pliku DoD: zmiana w pliku zmienia działanie hooka bez zmiany skryptu.
- [ ] Testy hooków są częścią `dotnet test CleanArchitecture.slnx` i przechodzą lokalnie oraz w CI na
      Ubuntu (`pwsh`). **Lokalnie tak; CI jeszcze nie uruchomione (patrz Comments).**
- [x] Build: 0 błędów, 0 ostrzeżeń, bez wyciszeń.

## Comments

**2026-10-26, `/implement`:**

- Pliki: `docs/agents/dod.json` (plik DoD), `.claude/hooks/dod-gate.ps1` (hook), rejestracja `SubagentStop`
  w `.claude/settings.json` (bez matchera, timeout 1800 s; skrypt sam filtruje po `agent_type`), projekt
  `tests/HookTests` w solucji.
- Format pliku DoD: `implementerAgent`, `environmentChecks` (wymaganie Dockera to wpis `docker` z
  `docker info`), `commands` (po kolei; pierwsza porażka kończy, więc `--no-build` nigdy nie idzie po
  nieudanym buildzie), `protectedPaths`, `quota`. Plik DoD jest szukany w górę od `cwd` sub-agenta, a komendy
  idą z katalogu, w którym leży (działa też w worktree).
- Build 0/0. Testy: Architecture 6, Unit 54, Hook 9, Integration 14, wszystkie zielone (Docker uruchomiony
  przez agenta).
- Red w cyklu: `Block_WhenDodCommandFails`, `Pass_WhenAgentIsNotTheImplementer`,
  `ReportEnvironmentFailureWithoutBlocking_WhenEnvironmentCheckFails`,
  `RunCommandsFromRepositoryRoot_WhenCwdIsSubdirectory`, `Pass_WhenEveryDodCommandSucceeds` (prawdziwy błąd,
  niżej), `QuoteOnlyTheTailOfTheOutput_WhenFailingCommandIsVerbose`.
- Red proof (hash skryptu `a8ac3017…` przed i po każdym): `Block_WhenStopHookIsAlreadyActive` (dodany skrót
  przy `stop_hook_active`), `Block_WhenAgentIsTheImplementerNamedInDodFile` (rola wpisana na sztywno),
  `Pass_WhenRepositoryHasNoDodFile` (usunięty warunek braku pliku). Każdy test padł, plik przywrócony.
- Znaleziony błąd: `pwsh` jest tu narzędziem dotnet, więc ścieżka bieżącego procesu to `dotnet.exe`, a komendy
  DoD szły przez `dotnet -NoProfile …` i zawsze padały. Testy 1, 3 i 4 i tak przechodziły, bo oczekiwany tekst
  był w cytowanej treści komendy. Poprawione: tekst powstaje dopiero przy wykonaniu, testy zrobiły się
  czerwone, naprawa to `pwsh` z `PATH`.
- Próby dymne na prawdziwym pliku DoD: Docker wyłączony daje `systemMessage` o awarii środowiska bez blokady;
  Docker działa: przejście bez komunikatu (21 s); tymczasowy CS0169 daje blokadę z przyczyną w 27 liniach.
- Review Spec (Sonnet), poprawione: przyczyna przycięta do 100 ostatnich linii wyjścia (każda blokada trafia
  do kontekstu implementera, czyli kosztuje limit); `.github/**` usunięte z chronionych ścieżek (poza listą
  speca, zablokowałoby ticket CI w etapie 2); `Link` w csproj z ukośnikami `/`. Ponad listę speca zostają
  `docs/agents/**` (sam plik DoD), `CLAUDE.md` i `tests/HookTests/**` jako konfiguracja agenta.
- Świadomie zostawione, do ticketu 03:
  1. Przy awarii środowiska bramka nie sprawdza też buildu. Ficzer i tak staje na awarii środowiska.
  2. Nieczytelny `dod.json` wyłącza bramkę (hook kończy się błędem nieblokującym), więc pre-flight `/ship`
     musi walidować plik DoD.
  3. Uruchomienie Docker Desktop rozpoznaje wpis `docker` w `environmentChecks`.
  4. `systemMessage` widzi użytkownik, nie orkiestrator. Orkiestrator sprawdza środowisko sam.
- Do ticketu 11: `tests/ArchitectureTests/**` jest chronione, więc regułę „`Mobile.Core` nie zależy od MAUI”
  musi dodać człowiek albo orkiestrator, nie implementer pętli.
- CI na Ubuntu: niezweryfikowane. Workflow startuje przy pushu do `main` i PR-ach do `main`; sprawdzi się
  przy scaleniu `chore/agent-loop` albo ręcznym `workflow_dispatch` na tej gałęzi.
- Fork bez zmian. Istniejące `dotnet build` i `dotnet test` w skillu `implement` to ogólna wskazówka sprzed
  pętli, nie komendy DoD.
