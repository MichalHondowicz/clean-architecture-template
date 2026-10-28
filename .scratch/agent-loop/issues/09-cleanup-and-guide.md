# 09: Porządki i przewodnik

**What to build:** Repozytorium i mój przewodnik opisują nowy przepływ, a nic nie wprowadza agenta w błąd:
znika stara kopia skilli, `docs/workflow.md` opisuje dwie bramki, `AGENTS.md` wskazuje `/plan-feature`
i `/ship`, a każda decyzja, o którą agent dotąd pytał w trakcie, jest regułą albo częścią bramki 1.

**Blocked by:** 04, 05, 06, 07, 08

**Seams:** brak testów automatycznych. Sprawdzenie w review Spec i w pilocie (ticket 10).

**Status:** done

- [x] Nieśledzony katalog `.agents/skills/` jest usunięty (historia 60).
- [x] `docs/workflow.md` opisuje przepływ `/plan-feature` → bramka 1 → `/ship` → push → bramka 2, tabelę
      „Ty / agent”, powiadomienia, limity i regułę decyzji po pilotach (historie 48, 61).
- [x] Sekcja procesu w `AGENTS.md` wskazuje `/plan-feature`, `/ship`, role i modele, plik DoD i kontrakt
      implementera, bez powtarzania treści skilli.
- [x] Każda decyzja z `docs/workflow.md` i z pilota archive-todo, którą dawniej podejmowałem w trakcie
      implementacji, jest regułą w `AGENTS.md` albo w skillu, albo częścią bramki 1 (historia 5).
- [x] Build: 0 błędów, 0 ostrzeżeń; pełne testy zielone.

## Comments

**2026-10-26, przeniesione z bramki ticketu 05** (do przewodnika `docs/workflow.md`):

- sesję `/ship` uruchamia się na Sonnecie;
- po resecie tygodniowym i po restarcie aplikacji w trakcie czekania na okno 5-godzinne `/ship <feature>`
  wpisuje użytkownik;
- tryb auto nie pozwala agentowi edytować `.claude/settings.json`, więc wpisy allowlisty dodaje użytkownik;
- `claude -p` jest nieużywalne, bo samodzielne CLI nie jest zalogowane.
- `effort` we frontmatterze skilla `ship` (`medium`) obowiązuje, gdy skill jest aktywny; sesję `/ship` warto
  też uruchamiać z effortem `medium`, bo kolejne tury (powiadomienia o sub-agentach) idą na effortcie sesji.

**2026-10-28, decyzje przed implementacją** (bramka w sesji z handoffu `docs/handoff-2026-10-28-agent-loop-09.md`;
wszystkie rekomendacje przyjęte jedną odpowiedzią „Tak, wszystkie”).

Fakty sprawdzone przed rundą:

- `.agents/` ma tylko `skills/` (stare kopie `add-entity`, `add-feature`, `add-tests`, `ca-review`). `hooks/` i
  `hooks.json` leżą w `.codex/`, nie w `.agents/`.
- `.codex/` nie odwołuje się do `.agents/`, ale jest nieaktualny: jego `run-dod.ps1` nie ma `-Check quota` ani
  wyjścia `passed` (tickety 05, 06), a `wait-for-reset.ps1` w nim brak.
- `ask-matt/SKILL.md` w forku już opisuje `/ship` i `/plan-feature`; nieaktualna jest tylko strona
  `docs/engineering/ask-matt.md`.
- Wpis `to-tickets` w `AGENTS.md` pasuje dalej, bo `/plan-feature` postępuje według procesu `to-tickets`.
  Implementer `/ship` bierze „the commands `AGENTS.md` gives”, a te są dziś przypisane tylko `implement`, `tdd`.

Decyzje:

1. **Usunięcie: cały `.agents/`.** `.codex/` zostaje nieśledzony (decyzja 7 z ticketu 05), mimo że jest
   nieaktualny; przyszły wykonawca Codex dostanie kopię ze skryptu.
2. **`docs/workflow.md`:**
   - główny przepływ: `/plan-feature` → bramka 1 → `/ship` → bramka 2 (scalenie, push, CI);
   - stary przepływ jako krótka sekcja „Przy komputerze” (`implement`, `implement-spec`, historia 62);
   - nowa tabela „Ty / agent”;
   - osobne sekcje: powiadomienia (Remote Control, ustawienie „Connect new sessions to Remote Control”,
     mobile push w `/config`), limity (progi z `dod.json`, czekanie na okno 5-godzinne, po resecie
     tygodniowym i po restarcie aplikacji `/ship <feature>` wpisuje użytkownik), ustawienia sesji
     (`/plan-feature` na Opusie z effortem `high`, `/ship` na Sonnecie z `medium` w trybie auto, `/model` i
     jedno „Try again” po `ECONNRESET`), reguła po pilotach (historia 48);
   - pułapki: tryb auto blokuje edycję `.claude/settings.json` (allowlistę dopisuje użytkownik), `claude -p`
     nieużywalne;
   - „Istniejący projekt”: to, czego wymaga pętla (`dod.json`, `.claude/hooks/`, kontrakt implementera,
     rejestracja hooków w ustawieniach projektu);
   - skille tylko przez odsyłacze do stron forka; ntfy (ticket 12, AC6) nie wchodzi.
3. **Inwentarz decyzji (kryterium 4):** tabela niżej. Żadna pozycja nie wymaga nowej reguły w `AGENTS.md`.
4. **Sekcja procesu w `AGENTS.md`:** krótkie wskazania `/plan-feature` i `/ship`, modele ról w jednym zdaniu
   z odsyłaczem do tabeli w spec, `docs/agents/dod.json` i `docs/agents/implementer-contract.md`, bez treści
   skilli. Wpis `to-tickets` dostaje „also when `/plan-feature` follows it”, a wpis `implement`, `tdd`
   obejmuje też implementera `/ship`.
5. **Korekta decyzji z bramki 2 po scaleniu: wariant (a).** Przewodnik opisuje ręczną korektę: osobny commit
   cofający regułę i dopisek pod `## Decision` w briefie. `/ship` bez zmian, dopóki pilot nie pokaże, że to
   częste.
6. **Fork:** poprawka strony `docs/engineering/ask-matt.md` (główny przepływ zna gałąź bez nadzoru
   `plan-feature → ship → retro`), `SKILL.md` bez zmian, plugin `1.3.1-dotnet.19`.
7. **Sandbox:** worktree `toy-overdue` i `toy-quota` z gałęziami `feat/toy-*` oraz stare sesje przebiegów
   tylko na wyraźne słowo użytkownika; w tym tickecie nic z nimi.

Inwentarz decyzji:

| Decyzja podejmowana dawniej w trakcie | Źródło | Gdzie leży teraz |
|---|---|---|
| Zatwierdzenie seamów przed pierwszym testem | `workflow.md`, krok 6 | bramka 1 (`Seams:` w tickecie), `tdd` |
| Status ticketu z uwagi z review | `workflow.md`, krok 4; pilot, kroki 4–5 („odłożone do 03”) | `/ship`: uwaga spoza zakresu to ticket `needs-triage`, `/triage` po fakcie |
| Decyzja z triage dotycząca wielu use case'ów idzie do ADR-a | `workflow.md`, krok 4 | przewodnik `docs/workflow.md` („Po ficzerze”, „Przy komputerze”): `/triage` należy do użytkownika i nie leży między bramkami (dopisane po review Spec, runda 1) |
| ADR-y i pojęcia słownika | `workflow.md`, krok 1 | bramka 1 (grill w `/plan-feature`) |
| Nowe konwencje (`/retro`, zmiana konwencji przez bramkę) | `workflow.md`, „Utrzymanie”; pilot, `/retro` | bramka 2, decyzja per propozycja |
| Gałąź `feat/<slug>` | `workflow.md`, krok 5 | `/ship` (worktree) |
| Baza `ca-review` (`base <sha>`) | `workflow.md`, krok 7 | `/ship`: `git merge-base main HEAD` |
| Zamknięcie ticketu | `workflow.md`, krok 8; pilot, retro #8 | `/ship` + `docs/agents/issue-tracker.md` |
| Scalenie fast-forward za zgodą | `workflow.md`, krok 9 | bramka 2 („OK”) |
| `git push origin main`, `git branch -d` | `workflow.md`, krok 10 | `/ship` po „OK” + allowlista |
| Uruchomienie Docker Desktop | `workflow.md`, tabela; pilot, ticket 01 | `dod.json` (`environmentChecks.start`) |
| Nagłówek `// <auto-generated />` w migracji | pilot, „Korekty użytkownika” | `AGENTS.md` (Build and verification) + `add-entity` |
| Migracja dla nowego pola (`PendingModelChangesWarning`) | pilot, retro #2 | `add-entity`, `add-feature`, ADR 0004 |
| Review ticketu tylko na osi Spec, Standards raz na gałąź | pilot, retro #7 | `AGENTS.md` (`code-review`) + `/ship` |
| Kod błędu w `ProblemDetails.Title` | pilot, retro #10, bramka T3 | `AGENTS.md` (Tests) |
| Zmiany istniejących use case'ów i historie „bez zmian w kodzie” | pilot, obserwacje 3 i 8 | `AGENTS.md` (`to-tickets`) |
| Test unit dla każdego zmienionego handlera | pilot, obserwacja 9 | ADR 0004 |
| Metody domenowe `Archive`/`Unarchive` | pilot, retro #10 | nie dotyczy: decyzja projektowa tylko na gałęzi pilota; jeśli wróci, rozstrzyga ją grill na bramce 1 |
| P1, P2 (dowód pilota, kolejność przebiegów) | pilot, „Decyzje przed pilotem” | nie dotyczy: jednorazowe |

**2026-10-28, wynik implementacji** (`/implement`, bez `tdd`: ticket nie ma seamów kodu):

- **Kryterium 1:** nieśledzony `.agents/` usunięty w całości (miał tylko `skills/`). `.codex/` został.
- **Kryterium 2:** `docs/workflow.md` przepisany:
  - główny przepływ `/plan-feature` → bramka 1 → `/ship` → bramka 2, stary jako „Przy komputerze”;
  - nowa tabela „Ty / agent”;
  - sekcje „Ustawienia sesji”, „Powiadomienia”, „Limity”, „Wznawianie i stany pośrednie”, „Reguła decyzji po
    pilotach”, „Korekta decyzji z bramki 2 po scaleniu”;
  - „Istniejący projekt” z elementami pętli; nowe pułapki;
  - skille opisane odsyłaczami do `plan-feature.md` i `ship.md` forka.
- **Kryterium 3:** w `AGENTS.md` nowy wpis o `/plan-feature` i `/ship` (DoD, kontrakt, modele) oraz dopiski
  w `to-tickets` i `implement`, `tdd`. Odstępstwo od decyzji 4: zamiast odsyłacza do tabeli w spec jedno
  zdanie o modelach i wskazanie definicji ról w pluginie, bo w projekcie z template'u `.scratch/agent-loop/`
  się usuwa (uwaga review, runda 1).
- **Kryterium 4:** inwentarz wyżej, z wierszem o regule ADR-a z triage dopisanym po review. Nowych reguł
  w `AGENTS.md` nie było trzeba.
- **Kryterium 5:** build 0 błędów, 0 ostrzeżeń; ArchitectureTests 6, Application.UnitTests 54,
  IntegrationTests 14, HookTests 83, wszystkie zielone. Po pomiarze zmieniały się tylko pliki `.md`.
- **Fork** (`D:\Projects\skills`, gałąź `dotnet`): strona `docs/engineering/ask-matt.md` zna `/ship`
  i `/plan-feature` jako trzeci sposób pracy nad ticketami; `ask-matt/SKILL.md` bez zmian; plugin
  `1.3.1-dotnet.19`. `claude plugin validate . --strict`: tylko dwa stare ostrzeżenia.
- **Review Spec** (subagent na Sonnecie):
  - runda 1: 8 uwag w zakresie, wszystkie poprawione: piąty push (czerwone CI), brakujące wpisy allowlisty
    bramki 2, `dod-common.ps1`, reguła ADR-a z triage, wiszący odsyłacz do spec, stany bramki 2, rundy
    poprawek i trzecie czekanie w limitach, „trzy gałęzie” na stronie `ask-matt`;
  - runda 2: zdanie o modelach w `AGENTS.md` i luka wznawiania (miejsce stanu w worktree, droga po
    `needs-info`, wznowienie `in-progress`, przerwana bramka 2, ponowne review gałęzi, przerwane planowanie),
    poprawione nową sekcją; krótka kontrola poprawek dała jeszcze dwie drobne nieścisłości, poprawione;
  - poza zakresem: ticket po trzech rundach ma trzy pliki findings, więc po odblokowaniu `/ship` od razu uzna
    go znowu za `stuck`. Założony ticket 13 (`needs-triage`).
