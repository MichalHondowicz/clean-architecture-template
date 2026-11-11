# 33: Poprawki po przeglądzie workflow: smoke, wyjątek od zakazu wyciszeń, sprawdzenie gałęzi, synchronizacja hooków

**What to build:** Sześć poprawek z przeglądu całego workflow (2026-11-11), przyjętych przez użytkownika w jednej
rundzie pytań: pętla raz przed briefem uruchamia zbudowaną aplikację, zakaz wyciszeń ma drogę decyzji użytkownika,
cała gałąź przechodzi przez `check-diff.ps1`, hooki kopiuje skrypt, a pętla jest zamrożona.

**Blocked by:** None

**Seams:** skrypty z `.claude/hooks/` jako procesy (`tests/HookTests`); `ship/SKILL.md`, `agents/implementer.md`
i strona `docs/engineering/ship.md` w forku; obserwacja przy następnym przebiegu `/ship`.

**Status:** done

**Skąd:** przegląd stanu i workflow 2026-11-11. Powody: brief `emulator-tls-release-e2e` w Koszyku mówi, że nic
nie uruchomiono na emulatorze ani w Dockerze; runda 2 jego review zamieniła `#pragma` dla eksperymentalnego API
Aspire na refleksję po nazwie typu; hooki chronią tylko implementera, a `check-diff.ps1` widział tylko zakresy
ticketów i rund; hooki kopiowane ręcznie do Koszyka i sandboksu.

- [x] AC1: `run-dod.ps1 -Check smoke` puszcza komendy `smoke` z `dod.json`, staje na pierwszej czerwonej, a bez
      listy przechodzi i nic nie uruchamia (testy: `RunDodTests.RunDod_Should_StopAtTheFirstFailingSmokeCommand_WhenCheckingSmoke`,
      `RunDodTests.RunDod_Should_RunNeitherCommandsNorSmoke_WhenCheckingSmokeWithoutASmokeList`).
- [x] AC2: `check-diff.ps1` przepuszcza `#pragma warning disable` tylko z identyfikatorami z `allowedSuppressions`
      wersji bazowej `dod.json`; inne formy, obcy identyfikator, pragma bez identyfikatora i lista dopisana
      w sprawdzanym zakresie dalej są naruszeniem (testy: `CheckDiffTests.CheckDiff_Should_AllowPragma_WhenBaseDodListsEveryDisabledId`,
      `CheckDiffTests.CheckDiff_Should_ReportSuppression_WhenItGoesBeyondTheAllowedPragmaIds`,
      `CheckDiffTests.CheckDiff_Should_ReportSuppression_WhenRangeItselfAllowsTheId`).
- [x] AC3: `sync-hooks.ps1 -Target <projekt>` kopiuje skrypty, zapisuje commit źródłowy w `.claude/hooks/SOURCE`
      i odmawia przy niezacommitowanych zmianach (testy: `SyncHooksTests`, trzy metody).
- [x] AC4: `/ship` przed briefem puszcza smoke i `check-diff.ps1` na całej gałęzi; brief ma sekcje **Smoke**,
      **Branch diff** i **Check before OK**. Ticket, który wymaga wyciszenia, kończy się `needs-info` z dwiema
      drogami dla użytkownika. Implementer nie obchodzi analizatora refleksją. Wersja forka podbita.
- [x] AC5: `AGENTS.md`, kontrakt implementera, `ca-review`, `issue-tracker.md` i przewodnik opisują wyjątek,
      smoke, synchronizację hooków, zamrożenie pętli i zasadę „krok deterministyczny do skryptu”.
- [x] AC6: Tickety 28–30 `wontfix` do drugiego projektu MAUI.

## Comments

**2026-11-11, wykonanie (sesja przeglądu, bez `/ship`):**

- Dowód czerwieni: 7 z 12 nowych przypadków padło przed implementacją (oba `AllowPragma`, oba `Smoke`, trzy
  `SyncHooks`); 5 przypadków „dalej naruszenie” było zielonych od początku i zostaje jako ochrona przed
  zbyt szerokim wyjątkiem.
- Po implementacji `HookTests` 104/104, build 0 ostrzeżeń.
- Fork: `527c8e3`, plugin `1.3.1-dotnet.33`.
- Poza zakresem, do zrobienia przez użytkownika albo po bramce 2 Koszyka:
  - Koszyk i sandbox nie dostały nowych hooków: w Koszyku ficzer czeka na bramkę 2, a commit na jego `main`
    zatrzymałby scalenie. Po scaleniu: `sync-hooks.ps1 -Target D:\Projects\Koszyk`.
  - Template nie ma listy `smoke`: jego testy integracyjne już uruchamiają API po HTTP na prawdziwym
    PostgreSQL. Pierwszym odbiorcą `smoke` jest Koszyk (start AppHost i sonda zdrowia), jako jego własny ticket.
  - Żaden przebieg `/ship` nie sprawdził jeszcze nowych sekcji briefu: pokaże to następny ficzer.
