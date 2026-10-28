> Nieaktualny. Wykonaj docs/handoff-2026-10-28-agent-loop-10.md

# Handoff: pętla bez nadzoru, ticket 09 (porządki i przewodnik)

## 1. Stan wejściowy

- **Template** `D:\Projects\clean-architecture_new`:
  - lokalna gałąź `chore/agent-loop`, niewypchnięta; HEAD to commit tego handoffu;
  - drzewo czyste poza nieśledzonymi `.agents/` i `.codex/`.
- **Tickety** w `.scratch/agent-loop/issues/`:
  - 01–08: `done`;
  - 09: `ready-for-agent`;
  - 10, 11: `ready-for-human`;
  - 12: `needs-triage`.
- **Ostatni własny pełny przebieg**, przy tickecie 08, 2026-10-28: build bez błędów i ostrzeżeń, testy zielone
  (ArchitectureTests 6, Application.UnitTests 54, IntegrationTests 14, HookTests 83). Od tamtej pory w template'cie
  zmieniły się tylko pliki w `.scratch/` i `docs/`.
- **Fork skilli** `D:\Projects\skills`, gałąź `dotnet`, HEAD `86e03d9`, drzewo czyste. Zainstalowany plugin
  `mattpocock-skills` to `1.3.1-dotnet.18`, już z `/plan-feature`.
- **Sandbox** `D:\Projects\ship-sandbox`:
  - checkout główny na `main` (`b493a0f`), `origin/main` tak samo;
  - stoi na nim zakończony ficzer `todo-open-count`, pierwszy pełny przebieg `/plan-feature` → `/ship` →
    bramka 2;
  - zostały stare worktree `toy-overdue` i `toy-quota` z gałęziami `feat/toy-*`.
- **Limit w trakcie przebiegu 08:** okno 5-godzinne do ok. 19%, tydzień 52%. Sprawdź `get_usage` na starcie.

## 2. Co robimy dalej

Ticket **09: porządki i przewodnik**, plik `.scratch/agent-loop/issues/09-cleanup-and-guide.md`. Ticket
istnieje i ma status `ready-for-agent`. Testów automatycznych nie ma: sprawdza go review Spec i pilot (10).

Najpierw bramka w sesji: pytania z sekcji 4 w jednej rundzie. Potem użytkownik wpisuje
`/mattpocock-skills:implement .scratch/agent-loop/issues/09-cleanup-and-guide.md`.

Po 09 zostają pilot 10 i rozpisanie etapu 2 (11). Oba należą do użytkownika. Push i scalenie `chore/agent-loop`
do `main` też należą do niego, dopiero po pilocie.

## 3. Co przeczytać na start

- **`.scratch/agent-loop/issues/09-cleanup-and-guide.md`:** kryteria oraz `## Comments`, czyli rzeczy przeniesione
  do przewodnika z bramki 05.
- **Materiały do przewodnika z innych ticketów** (wyszukasz je przez `grep -n "przewodnik\|(09)"`):
  - `03`: model sesji `/ship`;
  - `05`: start na Sonnecie;
  - `06`, decyzja o Remote Control: ustawienie „Connect new sessions to Remote Control” i FAQ;
  - `08`, decyzja 1: effort `high` dla `/plan-feature`, oraz punkt „Do przewodnika (09)” w żywym przebiegu;
  - `10`, ostatni komentarz: korekta decyzji z bramki 2 po scaleniu;
  - `12`, AC6: ustawienie tematu ntfy. To czeka na triage 12, więc nie wchodzi teraz.
- **`.scratch/agent-loop/spec.md`:**
  - historie 5, 48, 60–62;
  - „Implementation Decisions”: „Podział odpowiedzialności”, „Role i modele”, „Porządki”.
- **`docs/workflow.md`:** cały, 144 linie. Sekcje „Funkcja od pomysłu do `main`” i „Co robisz Ty, a co agent”
  opisują stary przepływ ośmiu ręcznych kroków.
- **`docs/pilot-2026-10-archive-todo.md`:** sekcje „Decyzje przed pilotem”, „Korekty użytkownika” i „Obserwacje
  procesu”. To źródło decyzji, które użytkownik podejmował w trakcie (kryterium 4).
- **`AGENTS.md`:** sekcja „Agent skills” → „Process skills in this project”. Brakuje tam `/plan-feature` i `/ship`.
- **Fork:** `docs/engineering/plan-feature.md` i `docs/engineering/ship.md`. Przewodnik ma do nich odsyłać, nie je
  powtarzać.

## 4. Otwarte pytania na bramkę

Rozstrzygnij je z użytkownikiem w jednej rundzie, zanim zaczniesz. Fakty oznaczone „sprawdź” sprawdź przed rundą,
nie z pamięci.

1. **Co usunąć.** W `.agents/` jest tylko `skills/` ze starymi kopiami `add-entity`, `add-feature`, `add-tests`,
   `ca-review` oraz z `hooks/` i `hooks.json`. Żaden śledzony plik poza `.md` nie odwołuje się do `.agents`
   (`git grep "\.agents" -- ':!*.md'` nic nie zwraca). `.codex/` to kopia hooków dla Codexa z bezwzględnymi
   ścieżkami. Decyzja z 2026-10-26: zostaje nieśledzona i nigdy nie trafia do commita. Warianty:
   - (a) usunąć cały `.agents/`, a `.codex/` zostawić;
   - (b) usunąć oba;
   - (c) tylko `.agents/skills/`, jak w kryterium.

   Rekomendacja: (a). Sprawdź, czy `.codex/` czyta coś z `.agents/`, zanim go zostawisz.
2. **Kształt `docs/workflow.md`.** Historia 62 każe zostawić `implement` i `implement-spec` do pracy przy
   komputerze. Rekomendacja:
   - przepływ bez nadzoru jako główny: `/plan-feature` → bramka 1 → `/ship` → bramka 2;
   - stary przepływ jako krótka sekcja „Przy komputerze”;
   - nowa tabela „Ty / agent”;
   - osobne sekcje: powiadomienia, limity, ustawienia sesji (model, effort, tryb, Remote Control) i reguła
     decyzji po pilotach (Max 5x albo zewnętrzny implementer, ze spec);
   - opis skilli tylko przez odsyłacze do stron forka.
3. **Inwentarz decyzji (kryterium 4).** Rekomendacja:
   - tabela w `## Comments` ticketu 09 z kolumnami: decyzja, źródło (sekcja `workflow.md` albo dziennika pilota),
     gdzie teraz leży (reguła `AGENTS.md` / skill / bramka 1 / bramka 2);
   - nowe reguły do `AGENTS.md` tylko tam, gdzie żadne miejsce jej nie pokrywa, każda pokazana użytkownikowi
     przed zapisem;
   - `AGENTS.md` jest chronione tylko przed implementerem `/ship`, nie przed tą sesją.
4. **Sekcja procesu w `AGENTS.md`.** Rekomendacja: dopisać krótkie wskazania:
   - `/plan-feature` i `/ship`;
   - modele ról w jednym zdaniu albo odsyłacz do tabeli w spec;
   - `docs/agents/dod.json` i `docs/agents/implementer-contract.md`;
   - bez treści skilli.

   Sprawdź, czy wpis o `to-tickets` nadal pasuje, skoro `/plan-feature` go czyta.
5. **Korekta decyzji z bramki 2 po scaleniu.** W sandboksie „2 tak” było pomyłką. Konwencję cofnięto ręcznie
   osobnym commitem i dopisano korektę pod `## Decision` w briefie (ticket 10, ostatni komentarz). Warianty:
   - (a) przewodnik opisuje ręczną korektę;
   - (b) `/ship` dostaje ścieżkę korekty.

   Rekomendacja: (a). Zmiana skilla dopiero wtedy, gdy pilot pokaże, że to częste.
6. **Porządki w forku przy okazji.** Review ticketu 08 znalazło nieaktualną stronę
   `D:\Projects\skills\docs\engineering\ask-matt.md`: nie wspomina ani `ship`, ani `plan-feature`. Rekomendacja:
   poprawić w 09 i podbić plugin do `1.3.1-dotnet.19`. To porządki, a 09 jest ostatnim ticketem przed pilotem.
7. **Sprzątanie sandboksu.** Worktree `toy-overdue` i `toy-quota` z gałęziami. Rekomendacja: usunąć na wyraźne
   słowo użytkownika i w tej sesji tylko zapytać. Zarchiwizowanie starych sesji przebiegów (A, B, B2, C, D,
   „Przebieg plan-feature: toy-open-count”, „/ship todo-open-count”) także tylko na jego słowo.

## 5. Dług i ustalenia przeniesione

Nie spłacaj ich tutaj bez decyzji użytkownika.

- **Do pilota (10), nadal niesprawdzone na żywo:**
  - z ticketu 07: `merge --ff-only`, gdy checkout stoi na `main`; krok CI w przeglądarce sesji w tle (sandbox nie
    ma CI); wznawianie przerwanej bramki 2; odpowiedź „nie” albo niejasna; czy allowlista przepuszcza sam
    `git push origin main`;
  - z wcześniejszych: rundy 2 i 3 gałęzi, scenariusze (b)–(d) ze spec, czyli Docker zatrzymany, pauza limitu i
    ticket niemożliwy.
- **Ticket 12** (ntfy): `needs-triage`. Pushe na telefon nadal nie dochodzą. Odpowiedź przez Remote Control
  dociera.
- **Z review 08, poza zakresem:** sesja w worktree nigdy nie stoi na `main`, więc `/plan-feature` w niej zawsze
  wymaga „OK here”. Pytania na stronie `plan-feature.md` są wymyślone, nie zaobserwowane, a zasada forka woli
  obserwowane.
- **Znane ograniczenie:** wznowiony ticket `in-progress` dostaje baseline na swoim bieżącym drzewie.

## 6. Środowisko i pułapki, które kosztowały czas

- **`git show <rev>:<ścieżka>` w Git Bash** zamienia `:` i `/` na ścieżkę Windows („ambiguous argument
  'feat\x;...'”). Poprzedź polecenie `MSYS_NO_PATHCONV=1`.
- **`/model` w sesji** przełącza model na stałe. Zaraz po przełączeniu raz wystąpił `ECONNRESET`, który wymagał
  „Try again”. Wpisz to do przewodnika.
- **Skill tool odmawia skilla z `disable-model-invocation`** i zabrania odtwarzać jego workflow. Skill tylko dla
  człowieka może przeczytać inny taki skill wyłącznie jako plik. Wyjątek jest opisany w `.agents/invocation.md`
  forka.
- **Tryb auto blokuje edycję `.claude/settings.json`.** Linie allowlisty dopisuje użytkownik, a potem sprawdzasz
  plik na dysku.
- **Długie wpisy po polsku** dopisuj przez Edit albo Write, nie przez heredoc w Bash.
- **Fork:**
  - po zmianie podbij wersję w `.claude-plugin/plugin.json`;
  - potem `claude plugin marketplace update mattpocock` i `claude plugin update mattpocock-skills@mattpocock`;
  - bieżąca sesja nie widzi nowej wersji;
  - `claude plugin validate . --strict` ma dwa stare ostrzeżenia: `CLAUDE.md` w korzeniu i brak ikony.
- **Build i testy** łańcuchem z `set -o pipefail`, bo bez tego `--no-build` za `grep` daje stary zielony wynik.
  Docker sprawdzasz przez `pwsh -NoProfile -NonInteractive -File .claude/hooks/run-dod.ps1 -Check environment`
  (narzędziem PowerShell).
- **Python nie jest zainstalowany.** Skrypty pisz w `node`.

## 7. Workflow

Bez zmian względem ticketów 05–08:

1. Bramka w tej sesji: pytania w jednej rundzie, decyzje w `## Comments` ticketu jako „Decyzje przed
   implementacją”, commit.
2. Użytkownik wpisuje `/mattpocock-skills:implement .scratch/agent-loop/issues/09-cleanup-and-guide.md`.
3. Bez `tdd`: ticket nie ma seamów kodu.
4. Pełny build i testy (kryterium 5).
5. Review na osi Spec: subagent na Sonnecie, dwie rundy. Każ mu sprawdzić przewodnik pod trzema kątami:
   - zgodność z faktycznym zachowaniem skilli w forku;
   - zgodność z `## Comments` ticketów;
   - pokrycie każdej pozycji inwentarza decyzji.
6. Commity w obu repozytoriach. Bump pluginu, jeśli fork się zmienił.
7. Zamknięcie ticketu.

## 8. Lekcje metodyczne do powtórzenia

1. **Fakty sprawdzaj przed rundą.** W 08 próba wywołania Skill toolem pokazała, że rekomendacja „wywołaj skill”
   nie zadziała, zanim powstał plan na niej oparty. W 07 to samo zrobiło repozytorium tymczasowe.
2. **Reguły forka czytaj przed pisaniem, nie po.** W 08 rekomendacja 2(a) łamała `.agents/invocation.md` forka.
   Wyszło to dopiero przy implementacji i wymagało dodatkowego pytania do użytkownika.
3. **Review Spec na Sonnecie znajduje dziury w każdej rundzie:** w 08 kolejno 8 i 7 uwag w zakresie, głównie przy
   wznawianiu i stanach pośrednich. Druga runda zostaje.
4. **Po przebiegu przeszukaj surowy transkrypt** (`~/.claude/projects/<ścieżka>/<sesja>.jsonl`), nie tylko raport.
   W 08 dopiero transkrypt pokazał, który model obsłużył każdą turę i że `to-spec` został przeczytany jako plik.
5. **Dziwny commit sprawdź, zanim go zrelacjonujesz.** W sandboksie para „conventions 1, 2” i „reject convention
   2” wyglądała na błąd skilla, a była poprawką pomyłki użytkownika. Wyjaśnił to opis commita.

## 9. Pierwszy krok i miejsce zatrzymania

1. Przeczytaj pliki z sekcji 3. Sprawdź fakty z pytań 1 i 4: czy `.codex/` odwołuje się do `.agents/` oraz czy
   wpis o `to-tickets` w `AGENTS.md` nadal pasuje. Szkic inwentarza decyzji z pytania 3 przygotuj przed rundą,
   żeby użytkownik oceniał konkretne pozycje.
2. Przedstaw pytania z sekcji 4 w jednej rundzie: numerowane, z rekomendacją, tak żeby „tak” przyjmowało
   rekomendację.
3. Zapisz decyzje w `## Comments` ticketu 09, zacommituj je i poproś o
   `/mattpocock-skills:implement .scratch/agent-loop/issues/09-cleanup-and-guide.md`.
4. Zatrzymaj się po zamknięciu ticketu 09. Pilota (10) nie zaczynaj: należy do użytkownika.
