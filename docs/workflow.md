# Workflow: praca z agentem w tym template'cie

Template Clean Architecture na .NET 10 (baza: template Milana Jovanovića), przygotowany do pracy z Claude Code.
Jest na własny użytek i nie będzie publikowany. Ten dokument opisuje, jak z niego korzystać: dla Ciebie, nie
dla agenta. Agent ma swoje zasady w `AGENTS.md`.

## Trzy warstwy i gdzie leżą

| Warstwa | Pytanie | Pliki |
|---|---|---|
| Konwencje | co jest dobrym kodem w tym projekcie | `AGENTS.md` (konstytucja), `.claude/skills/` (`add-feature`, `add-entity`, `add-tests`, `ca-review`), `.claude/settings.json` (allowlista komend i rejestracja hooków) |
| Domena | jak mówimy o projekcie i co już postanowiliśmy | `GLOSSARY.md`, `docs/adr/` |
| Proces | jak pracujemy z agentem | skille Matta Pococka z forka `D:\Projects\skills` (gałąź `dotnet`), opisane w `AGENTS.md`, sekcja „Agent skills”; `docs/agents/` (tracker, statusy, DoD w `dod.json`, kontrakt implementera); hooki pętli w `.claude/hooks/` |

Skille procesu są globalne: plugin `mattpocock-skills` wskazuje na fork i działa w każdym Twoim projekcie.
Projekt mówi im o sobie przez `AGENTS.md` (np. typecheck to `dotnet build`, jeden ticket to jeden use case) i
`docs/agents/` (tracker w `.scratch/`, statusy ticketów, słownik i ADR-y, a dla pętli bez nadzoru plik DoD
i kontrakt implementera).

## Nowy projekt z template'u

1. Utwórz repo z template'u. Na GitHubie wygodnie przez „Use this template”, jeśli w ustawieniach repo
   template'u zaznaczysz „Template repository”.
2. Sklonuj, potem `dotnet tool restore`, `dotnet build CleanArchitecture.slnx`, `dotnet test CleanArchitecture.slnx`
   (Docker Desktop musi działać, testy integracyjne startują PostgreSQL w Testcontainers).
3. Porządki w nowym projekcie:
   - zostaw `AGENTS.md`, `.claude/`, `docs/agents/`, `docs/adr/` (decyzje architektury obowiązują dalej);
   - historia budowy template'u nie dotyczy nowego projektu: `docs/plan-*.md`, `docs/pilot-*.md`,
     `docs/handoff-*.md` i zamknięte tickety w `.scratch/` możesz usunąć;
   - przykładowy kod Todo/Users zostaw, dopóki nie masz własnych slice'ów: skille wskazują go jako wzorzec
     (np. `add-entity` „following the `TodoItem` pattern”);
   - `GLOSSARY.md` opisuje domenę przykładu; zastępuj go swoimi pojęciami, gdy Twoja domena się wyostrzy
     (robi to grill w `/plan-feature` albo `/grill-with-docs`);
   - w `docs/agents/dod.json` sprawdź ścieżkę startu Docker Desktop (`environmentChecks`), jeśli masz go
     gdzie indziej.

## Istniejący projekt

Warstwa procesu jest globalna (plugin z forka Pococka, wyłączony `superpowers`), więc działa w każdym
projekcie od razu. Per projekt trzeba dołożyć to, z czego skille procesu czytają kontekst:

| Element | Po co |
|---|---|
| `docs/agents/` (przez `/setup-matt-pocock-skills`) | gdzie leżą tickety, jakie są statusy, gdzie słownik i ADR-y |
| sekcja „Process skills in this project” w `AGENTS.md` | komendy build i testów, review ticketu tylko na osi Spec, pełna nazwa `mattpocock-skills:code-review`, wskazanie `/plan-feature` i `/ship` |
| `GLOSSARY.md` i `docs/adr/` | słownik domeny i decyzje, których agent nie podważa |
| `CLAUDE.md` = `@AGENTS.md` plus zdanie, że reguły idą do `AGENTS.md` | jedno źródło reguł |
| `.claude/settings.json` z allowlistą komend | mniej pytań o zgodę przy buildzie i testach |
| tylko dla pętli: `docs/agents/dod.json` | DoD projektu: sprawdzenia środowiska, komendy buildu i testów, chronione ścieżki, nazwa roli implementera, progi limitu, skill standardów; opcjonalnie `screenPaths` (pliki ekranów do briefu), `smoke` (komendy, które raz przed briefem uruchamiają zbudowaną aplikację i same się kończą) i `allowedSuppressions` (identyfikatory diagnostyk, które wolno wyłączyć przez `#pragma warning disable`) |
| tylko dla pętli: `docs/agents/implementer-contract.md` | wejście, wyjście i zakazy implementera, niezależne od modelu |
| tylko dla pętli: `.claude/hooks/` i ich rejestracja w `.claude/settings.json` | `run-dod.ps1`, `check-diff.ps1`, `wait-for-reset.ps1`, `check-reboot.ps1`, `notify.ps1` (też hook `Notification`), `dod-gate.ps1` (`SubagentStop`), `guard-protected-paths.ps1` (`PreToolUse`) i wspólny `dod-common.ps1`, który cztery z nich wczytują; hooki rejestruje projekt, bo Claude Code ignoruje hooki we frontmatterze agentów z pluginu |
| tylko dla pętli: wpisy allowlisty `/ship` | `git worktree`, `git switch`, `git merge --ff-only`, dokładnie `git push origin main`, a dla bramki 2 także `git fetch origin`, `git fetch . feat/*:main` i `git update-ref -d refs/heads/feat/*`; do tego skrypty z `.claude/hooks/`. Pełny wzór (Bash i PowerShell) w `.claude/settings.json` tego template'u |

Wdrożenie: jedna sesja otwarta w katalogu projektu, na osobnej gałęzi (np. `chore/agent-workflow`).

1. **`/setup-matt-pocock-skills`**: tracker (np. `.scratch/`), etykiety, układ słownika i ADR-ów. Blok
   „Agent skills”, który skill dopisuje, przenieś do `AGENTS.md`.
2. **`AGENTS.md`**: dopisz sekcję procesu z tego template'u („Process skills in this project”) i podmień
   komendy na komendy projektu. Opisz tylko to, co w projekcie naprawdę obowiązuje (np. analizatory jako błędy
   tylko wtedy, gdy build tak działa).
3. **Domena**: `domain-modeling` buduje pierwszy `GLOSSARY.md` z kodu, a decyzje, które już zapadły, zapisuje
   jako ADR-y; potem krótki `/grill-with-docs`, żeby wyostrzyć pojęcia.
4. **Skille projektu tylko przy tej samej architekturze.** `add-feature`, `add-entity`, `add-tests` i
   `ca-review` opisują ten template (`Result`, własne handlery, Scrutor, Testcontainers). W projekcie o innej
   budowie agent pisałby według cudzych wzorców. Tam zacznij bez nich, a skille napisz po kilku funkcjach,
   gdy widać powtarzalne wzorce (`writing-for-agents`, `skill-creator`).
5. **Pętla bez nadzoru** (opcjonalnie): `dod.json` z komendami projektu, kontrakt implementera, hooki
   i wpisy allowlisty z tabeli wyżej. Bez `dod.json` `/plan-feature` i `/ship` zatrzymują się na starcie.
6. **Pilot**: jedna mała funkcja przez cały przepływ z sekcji niżej, potem `/retro`. Tak wychodzą rozjazdy
   między `AGENTS.md` a rzeczywistością projektu.

`AGENTS.md` z template'u nie kopiuj w całości: każdy projekt ma własną konstytucję o swojej architekturze,
a z template'u przenosisz tylko proces. Długi `AGENTS.md` przytnij przy okazji: agent czyta go w całości na
starcie każdej sesji, więc każda zbędna linia kosztuje.

## Funkcja od pomysłu do `main`

Ficzer ma dwie bramki i nic pomiędzy nimi. Komendy z ukośnikiem wpisujesz Ty, jako osobną wiadomość
zaczynającą się od `/` (to skille „user-only”, agent nie może ich wywołać sam). Jak działają skille w środku,
opisują strony forka: `D:\Projects\skills\docs\engineering\plan-feature.md` i
`D:\Projects\skills\docs\engineering\ship.md`.

1. **`/plan-feature <pomysł>`** w głównym checkoucie, na `main`, w sesji na Opusie z effortem `high`
   (ustawienia w sekcji „Ustawienia sesji”). Agent przepytuje Cię z pomysłu, zapisuje pojęcia w `GLOSSARY.md`
   i decyzje jako ADR, potem pisze spec i tickety z sekcją `Seams:` i kryteriami `AC1…n`, każde z planowaną
   nazwą testu.
2. **Bramka 1**: pakiet planu na jednym ekranie (spec, tickety, seamy, kryteria, DoD). Jednoznaczne „OK”
   zatwierdza całość: tickety dostają `ready-for-agent` i idą w jeden commit `plan: <feature>`. Każda inna
   odpowiedź to poprawka, po której pakiet wraca. Poza `main` skill commituje tylko na „OK here”.
3. **`/ship <feature>`** w nowej sesji: z karty, którą proponuje `/plan-feature`, albo ręcznie
   (`/mattpocock-skills:ship <feature>`). Sesja na Sonnecie, effort `medium`, tryb auto, uruchomiona
   w głównym checkoucie, nie w worktree. Potem odchodzisz od komputera.
4. **Pętla** (bez Ciebie): worktree `.claude/worktrees/<feature>` na gałęzi `feat/<feature>`; tickety po kolei,
   każdy w rundach implementer → weryfikator DoD → review Spec, najwyżej 3 rundy. Ticket, który nie zbiega,
   dostaje `needs-info` i brief w `## Comments`, a niezależne idą dalej. Uwaga spoza zakresu ticketu staje się
   nowym ticketem `needs-triage`. Na końcu review całej gałęzi na Opusie (Standards przez `ca-review` i Spec),
   potem `.scratch/<feature>/brief.md` i push na telefon. Przed briefem pętla puszcza komendy `smoke` z `dod.json`
   (jeśli są) i `check-diff.ps1` na całej gałęzi. Brief ma z tego sekcje **Smoke**, **Branch diff** (to, czego
   nie widziała żadna runda, np. Twój ręczny commit na gałęzi) i **Check before OK**: lista rzeczy, których
   pętla nie mogła uruchomić i które oglądasz sam przed odpowiedzią (ekrany na emulatorze, zachowania nigdy
   nieuruchomione).
5. **Bramka 2**: jedno pytanie w sesji `/ship`, z komputera albo przez Remote Control z telefonu. Odpowiadasz
   jedną wiadomością: „OK” scala, „nie” zostawia gałąź, a każda propozycja konwencji dostaje `tak` albo `nie`
   (np. `OK, 1 tak, 2 nie`):
   - pytanie, uwaga albo odpowiedź niejednoznaczna niczego nie rozstrzyga, także propozycji: agent odpowiada
     i pyta jeszcze raz;
   - propozycja, o której nie wspomnisz, zostaje otwarta i wraca na następnej bramce 2;
   - decyzję agent zapisuje pod `## Decision` w briefie i commituje razem z przyjętymi konwencjami na gałęzi
     ficzera, także przy „nie” (wtedy nic nie idzie do `main`);
   - po „OK” robi fast-forward `main`, `git push origin main`, usuwa worktree i gałąź i sprawdza CI
     w przeglądarce aplikacji. Repozytorium bez plików w `.github/workflows` dostaje od razu wynik
     `no CI: no workflow`, bez czekania na przebieg. Przy prywatnym repozytorium panel przeglądarki aplikacji
     musi być zalogowany do GitHuba, a logujesz się w nim sam. Wylogowany panel dostaje od GitHuba
     „Page not found”, więc pętla od razu zapisuje `CI not checked` i w raporcie prosi o zalogowanie.
     Projekt bez remote `origin` (np. Koszyk przed ticketem 20)
     dostaje tylko fast-forward lokalnego `main` i sprzątanie, bez pushu i CI. Zatrzymuje się, gdy `main`
     (zdalny, a bez remote lokalny) poszedł dalej niż baza gałęzi: rebase należy do Ciebie.
6. **Po ficzerze**: `/triage` dla nowych ticketów `needs-triage` (decyzja dotycząca wielu use case'ów idzie do
   ADR-a), a po większej porcji pracy `/retro`. Scalony ficzer jest dla `/ship` zamknięty: raport po scaleniu
   i każdy późniejszy `/ship <feature>` tylko wymieniają jego otwarte tickety (statusy z `main`) i drogę dla
   każdego. Ticket, który po triage'u dostał `ready-for-agent`, zrób przy komputerze przez `/implement`
   (sekcja „Przy komputerze”), a większą porcję zaplanuj jako nowy ficzer przez `/plan-feature`.

Na koniec sesji „kończymy” albo „zapisz stan”: skill `handoff` zapisuje w repo stan i prompt startowy dla
następnej sesji.

## Przy komputerze

Stary przepływ zostaje do pracy, przy której jesteś: chcesz zatwierdzać seamy i podział po kolei, sterować
ticketem w trakcie albo tracker nie jest lokalnym markdownem. `implement` i `implement-spec` działają bez
zmian.

1. **`/grill-with-docs <temat>`**, potem **`/to-spec`** i **`/to-tickets`** (każdy osobną wiadomością).
2. **`/triage`** dla ticketów `needs-triage`; decyzja dotycząca wielu use case'ów idzie do ADR-a.
3. **Gałąź** `fix/<slug>` albo `feat/<slug>` od `main`; agent może ją utworzyć.
4. **`/implement <ścieżka ticketu>`** dla każdego ticketu (albo `/implement-spec` dla tych, które mogą iść
   równolegle):
   - bez `Seams:` w tickecie agent proponuje seamy i czeka na Twoje zatwierdzenie;
   - cykle red → green (`tdd`), zwykle od testu HTTP; testy, które nie były czerwone, dostają red proof;
   - review ticketu tylko na osi Spec, skillem `mattpocock-skills:code-review`.
5. **`ca-review` całej gałęzi** z `base <sha>` początku gałęzi, potem zamknięcie ticketów (`**Status:** done`).
6. **Scalenie**: agent robi fast-forward do `main` za Twoją zgodą; `git push origin main` i
   `git branch -d <gałąź>` robisz Ty, dopiero po komunikacie o scaleniu.

## Co robisz Ty, a co agent

| Ty | Agent |
|---|---|
| `/plan-feature`, odpowiedzi w grillu i decyzja na bramce 1 | pytania z rekomendacją, spec, tickety z seamami i `ACn`, commit `plan: <feature>`, propozycja sesji `/ship` |
| start sesji `/ship` (model, effort, tryb) | worktree i gałąź, Docker, implementacja, weryfikacja DoD, review, briefy `needs-info`, tabela pomiaru |
| decyzja na bramce 2: scalenie i każda propozycja konwencji | brief, push na telefon, commit konwencji, fast-forward, `git push origin main`, sprzątanie, sprawdzenie CI |
| `/ship <feature>` po resecie tygodniowym i po restarcie aplikacji | czekanie na reset okna 5-godzinnego w tej samej sesji i wznowienie |
| `/triage` nowych ticketów `needs-triage` | zapis każdej uwagi spoza zakresu jako ticketu |
| wpisy allowlisty w `.claude/settings.json` (tryb auto blokuje agentowi edycję) | sprawdzenie pliku na dysku po Twojej zmianie |
| korekta pomyłki z bramki 2 (sekcja niżej) | brak: `/ship` nie ma ścieżki korekty |
| aktualizacja pluginu Pococka i restart sesji | zmiany w forku, commit na gałęzi `dotnet`, podbicie wersji |
| decyzja o planie po pilotach | liczby w `metrics.md` |

## Ustawienia sesji

| Sesja | Model | Effort | Tryb |
|---|---|---|---|
| `/plan-feature` | Opus | `high` | dowolny |
| `/ship` | Sonnet | `medium` | auto, nigdy `bypassPermissions` |

- **Dlaczego model sesji, a nie skilla.** Pole `model:` skilla obowiązuje tylko w turze, w której skill
  wystartował, a `effort:` tylko wtedy, gdy skill jest aktywny. Każda odpowiedź w grillu i każde
  powiadomienie o końcu pracy sub-agenta w `/ship` to nowa tura na modelu i effortcie sesji. `/plan-feature`
  sprawdza model sam i prosi o przełączenie, gdy to nie Opus.
- **Modele ról w pętli** ustawiają definicje ról w forku: implementer Sonnet (runda 3 Opus), weryfikator Haiku
  ze skryptem, review Spec ticketu Sonnet, review gałęzi Opus. Źródłem są pola `model:` w
  `D:\Projects\skills\agents\*.md`; rundę 3 implementera i review Spec gałęzi przełącza na Opusa `/ship`.
- **Przełączenie modelu**: `/model opus` w sesji albo wybór modelu w aplikacji. Zostaje na stałe dla tej sesji.
  Zaraz po przełączeniu raz wystąpił `ECONNRESET`: wystarczyło „Try again”.
- **Remote Control**: ustawienie aplikacji „Connect new sessions to Remote Control” jest włączone
  (od 2026-10-27), więc każda nowa sesja, także `/ship`, łączy się sama. `/ship` nie może go włączyć sobie
  sam (FAQ na stronie `ship.md` forka) i tylko podaje swój stan w raporcie i w briefie.
- **`/ship` startuj w głównym checkoucie.** Skill sam przechodzi do worktree ficzera, a bramka 2 scala
  z głównego checkoutu; sesja uruchomiona w worktree kończy się przed pytaniem.

## Powiadomienia

- **Push na telefon** z aplikacji Claude przy sześciu zdarzeniach: ficzer gotowy do bramki 2, ticket
  `needs-info`, awaria środowiska (Docker, build bazowy), pauza do resetu tygodniowego, oczekujący restart
  Windows i czerwone CI po scaleniu (bez cofania czegokolwiek). Treść to jedno zdanie i wskazanie briefu;
  pełny brief leży w `.scratch/<feature>/`. Każdy taki push idzie też przez ntfy (niżej), jeśli go ustawisz.
- Push dociera tylko z sesji z Remote Control `on` i przy włączonych powiadomieniach mobilnych w ustawieniach
  Claude Code (`/config` w terminalowym `claude`). Odpowiedź na bramce 2 przez Remote Control z telefonu
  działa.
- **Znany problem:** od 2026-10-27 pushe z sesji z Remote Control `on` nie przychodzą jako powiadomienie
  na telefonie, choć narzędzie odpowiada „Mobile push requested”. Wiadomość widać dopiero po otwarciu aplikacji
  Claude (pilot 10, 2026-10-29). Dlatego jest fallback przez ntfy.
- **Fallback ntfy** (`.claude/hooks/notify.ps1`): `/ship` wysyła tę samą linię do Twojego tematu ntfy, a hook
  `Notification` wysyła tam pytanie o zgodę z sesji. Ustawienie, raz na komputer:
  1. Wymyśl losowy temat o długości co najmniej 24 znaków. Na publicznym ntfy.sh temat jest sekretem tylko
     przez swoją nazwę: kto ją zna, czyta treść. Dlatego treść to jedno zdanie bez kodu.
  2. Zapisz pełny adres w zmiennej użytkownika (poza repozytorium), np.
     `[Environment]::SetEnvironmentVariable('CLAUDE_NTFY_URL', 'https://ntfy.sh/<temat>', 'User')`, i uruchom
     aplikację Claude od nowa, żeby sesje ją widziały. Bez zmiennej skrypt nic nie wysyła i nie zatrzymuje
     pętli, a raport `/ship` raz mówi, że temat nie jest ustawiony.
  3. W aplikacji ntfy na telefonie zasubskrybuj ten temat na serwerze `ntfy.sh`.
  4. Sprawdź: `pwsh -NoProfile -File .claude/hooks/notify.ps1 -Message "test"` powinien dać `{"sent":true}`
     i powiadomienie na telefonie.
  5. W `.claude/settings.json` dopisz sam (tryb auto blokuje agentowi edycję tego pliku) hook i allowlistę,
     tak jak w tickecie 12 `.scratch/agent-loop/issues/12-push-fallback-ntfy.md`.
- **Pytanie o zgodę w sesji w tle**: aplikacja oznacza sesję jako wymagającą uwagi, a hook `Notification`
  z matcherem `permission_prompt` wysyła je przez ntfy po około 6 sekundach (aplikacja desktopowa od Claude
  Code v2.1.233). Czy dociera, sprawdzi następny przebieg (ticket 19). Każde takie pytanie to kandydat do
  allowlisty.

## Limity

- **Progi** są w `docs/agents/dod.json` (`quota`): tydzień 80%, okno 5-godzinne 90%. Pętla nie zaczyna
  ticketu, review gałęzi ani rundy poprawek gałęzi na progu lub powyżej, żeby zostało Ci na planowanie
  następnego ficzera. W pilocie limit symulujesz, obniżając próg tuż pod bieżące zużycie.
- **Okno 5-godzinne**: pętla zapisuje pauzę w `metrics.md` i czeka w tej samej sesji. Skrypt
  `wait-for-reset.ps1` trzyma komputer w czuwaniu i kończy się 2 minuty po resecie, a sesja rusza dalej
  sama, bez pusha. Wyjątek: gdy dwa czekania z rzędu nie zwolniły okna, trzeciego nie ma. To awaria
  środowiska (`5-hour window`) z pushem.
- **Limit tygodniowy**: pauza w `metrics.md`, jeden push z godziną resetu i czyste zakończenie sesji. Po
  resecie wpisujesz `/ship <feature>` sam.
- **Restart w trakcie czekania** (aplikacji albo systemu): czekanie na okno 5-godzinne nie przeżywa restartu.
  Jak wznowić, opisuje „Wznawianie i stany pośrednie”.
- Wznawianie przez zadania zaplanowane aplikacji nie działa: zadanie nie rozwija komendy tylko dla
  człowieka, startuje na modelu sesji, która je utworzyła, i nie trzyma komputera w czuwaniu.
- **Pomiar**: `.scratch/<feature>/metrics.md`, wiersz na ticket (zużycie obu okien na starcie i końcu, rundy,
  model w każdej roli, czas, wynik), wiersz na pauzę i na review gałęzi, a na końcu suma. Start i wznowienie
  to ta sama komenda: stan to statusy ticketów i ich `## Comments`.

## Wznawianie i stany pośrednie

- **Gdzie jest stan.** Do scalenia tickety, ich `## Comments`, `brief.md`, `metrics.md` i pliki findings leżą
  w commitach gałęzi `feat/<feature>`, w worktree `.claude/worktrees/<feature>`. W głównym checkoucie
  `.scratch/<feature>/` ich nie ma: przy komputerze zaglądasz do worktree.
- **Przerwany przebieg** (zamknięty laptop, restart aplikacji lub systemu, limit tygodniowy): `/ship <feature>` w głównym
  checkoucie. Ticket `in-progress` idzie pierwszy, od następnej rundy (rundy liczą pliki findings), na bazie
  z commita `ship: start <NN>`. Niezacommitowane resztki w worktree bez ticketu `in-progress` idą do stasha
  `ship: leftovers`, nazwanego w raporcie.
- **Restart systemu w nocy** (aktualizacja Windows). Utrzymywanie komputera w czuwaniu nie chroni przed
  restartem po aktualizacji, a godziny aktywne też nie: w pilocie 10 restart „Upgrade (Planned)” przyszedł
  o 23:47, w godzinach aktywnych 18:00–0:00. Dlatego `/ship` na starcie i przed czekaniem na okno uruchamia
  `check-reboot.ps1`, a gdy Windows czeka na restart, wysyła push z ostrzeżeniem i pracuje dalej. Po takim
  pushu przed dłuższym przebiegiem lepiej zrestartować komputer samemu.
  - **Jak to rozpoznać rano:** sesja `/ship` ma komunikat „Background shell command didn't finish before the
    previous session ended”, ostatni wiersz `metrics.md` w worktree to pauza bez kolejnego ticketu, a log
    systemu pokazuje restart: `Get-WinEvent -FilterHashtable @{LogName='System'; Id=1074} -MaxEvents 3`.
  - **Jak wznowić:** napisz dowolną wiadomość w tej samej sesji `/ship` (np. „jak status?”). Pętla sprawdzi
    limit i środowisko i pójdzie dalej (pilot 10). Albo wpisz `/ship <feature>` w nowej sesji w głównym
    checkoucie. Obie drogi wznawiają z trackera. Od wersji `1.3.1-dotnet.35` skill każe sesji wznowić samej
    po takim komunikacie. Czy aplikacja daje jej wtedy turę, pokaże następny restart: 2026-11-11 w Koszyku
    (restart 20:25, reset okna 20:40) komunikat przyszedł, a sesja nie zrobiła nic, jeszcze na starej wersji.
- **Po `needs-info`**: gdy choć jeden ticket ficzera nie jest `done` (nie liczą się `needs-triage`
  i `wontfix`), przebieg kończy się bez review gałęzi, briefu i bramki 2. Ticket zachowuje swoje cofnięte
  commity w historii i stash nazwany w briefie. Odblokowanie, dwie równoważne drogi:
  - **Odpowiedź w tej samej sesji `/ship`**, dopóki ją masz: pętla wpisuje Twoją odpowiedź do `## Comments`
    ticketu jako `**Decision:**`, ustawia `ready-for-agent`, commituje i rusza od nowa.
  - **Ręcznie, w worktree ficzera:** dopisz w `## Comments` `**Decision:** …`, ustaw
    `**Status:** ready-for-agent`, zacommituj na `feat/<feature>` (niezacommitowaną zmianę `/ship` zestashuje)
    i wpisz `/ship <feature>`.

  W obu przypadkach ticket dostaje nową próbę z trzema rundami od rundy 1. Stare pliki findings trafiają do
  `findings/<NN>-attempt-<k>/`, więc nie liczą się jako zużyte rundy. Ticket na chronionej ścieżce ma swoje
  dwie drogi (punkt niżej).
- **Ticket na chronionej ścieżce** (`protectedPaths` w `dod.json`, np. `tests/ArchitectureTests/**`): pętla
  takiej zmiany nie zrobi, a zdjęcie ochrony na czas ticketu nie działa, bo `HookTests` przypinają ścieżki
  do guarda, a `check-diff` i tak oznaczy zmianę (pilot 10, ticket 18). Brief `needs-info` daje dwie drogi,
  obie jako commit w worktree ficzera, na `feat/<feature>`: tam leży tracker, a niezacommitowaną zmianę
  statusu kolejny `/ship` zestashuje.
  - Zrób zmianę ręcznie (planowana zmiana jest w briefie) i zacommituj ją razem z ticketem ustawionym
    na `done`, potem `/ship <feature>` jeszcze raz.
  - Albo zacommituj ticket ustawiony na `wontfix` i zrób zmianę poza ficzerem, przed `/ship` albo po
    scaleniu. Ticket, który ma go w `Blocked by`, nie ruszy, dopóki w tym samym commicie nie usuniesz tej
    zależności; brief wymienia takie tickety.
- **Ticket, który wymaga wyciszenia analizatora** (np. diagnostyka eksperymentalnego API): implementer kończy
  `STATUS: stuck` z identyfikatorem i powodem, zamiast obchodzić analizator refleksją. Decyzja jest Twoja,
  w commicie na `feat/<feature>` razem z `**Decision:**` i ticketem ustawionym na `ready-for-agent`:
  - dopisujesz identyfikator do `allowedSuppressions` w `docs/agents/dod.json` (plik chroniony, więc ręcznie);
    `check-diff.ps1` przepuszcza wtedy `#pragma warning disable` tylko z identyfikatorami z tej listy;
  - albo wskazujesz w `**Decision:**` inną drogę.

  Brief pokaże Twój commit w **Branch diff**: scalenie go potwierdza.
- **Ponowne review gałęzi**: usuń w commicie na gałęzi `brief.md` i `findings/branch-*.md`, potem `/ship`.
- **Przerwana bramka 2**: decyzja jest zapisana w briefie. Dopóki `main` nie zawiera gałęzi, kolejny `/ship`
  pyta jeszcze raz („OK” liczy się tylko w sesji, która scala). Gdy `main` już ją zawiera, `/ship <feature>`
  tylko dokańcza push, sprzątanie i sprawdzenie CI.
- **Przerwane planowanie**: niezacommitowany szkic wraca przez `/plan-feature <feature>`. Zacommitowanego
  planu skill nie rusza. Poprawki ticketów po `plan: <feature>` zacommituj przed `/ship`: z niezacommitowanym
  `.scratch/<feature>/` `/ship` nie utworzy worktree.

## Reguła decyzji po pilotach

Po dwóch pilotach (ficzer backendowy, potem MAUI) decyzja o planie jest mechaniczna i wynika z sum
w `metrics.md`: jeśli w tygodniowym limicie Pro mieszczą się mniej niż 2 ficzery po 3 tickety, to Max 5x albo
zewnętrzny implementer. W przeciwnym razie zostaje Pro. Kontrakt implementera
(`docs/agents/implementer-contract.md`) jest przygotowany pod podmianę modelu: zewnętrzny implementer dostaje
ten sam kontrakt i te same hooki.

## Korekta decyzji z bramki 2 po scaleniu

`/ship` nie ma ścieżki korekty. Gdy odpowiedź na bramce 2 była pomyłką, a ficzer jest już w `main`,
poprawiasz ją ręcznie (sam albo przez agenta w zwykłej sesji):

1. Na `main` jeden osobny commit, np. `docs: reject convention <n> of <feature> gate 2`:
   - zmiana reguły w pliku, który wskazała propozycja (usunięcie przyjętej albo dopisanie odrzuconej);
   - nowy wpis pod `## Decision` w `.scratch/<feature>/brief.md`, z datą i dopiskiem „(korekta)”, nazwą
     propozycji z nowym wynikiem i powodem. Wcześniejszego wpisu nie zmieniasz.
2. `git push origin main`.

Commit przyjętych konwencji z bramki 2 obejmuje wszystkie naraz, więc `git revert` cofnąłby też te dobre.
Ścieżka korekty w skillu powstanie, jeśli pilot pokaże, że pomyłki są częste.

## Weryfikacja

```bash
dotnet build CleanArchitecture.slnx && dotnet test CleanArchitecture.slnx --no-build
```

Analizatory są błędami (każde ostrzeżenie psuje build) i nigdy się ich nie wycisza. Testy: `UnitTests`
(handlery na EF InMemory), `ArchitectureTests` (kierunek zależności warstw), `IntegrationTests` (HTTP na
PostgreSQL), `HookTests` (skrypty z `.claude/hooks/` jako procesy). Liczby testów licz lokalnie: CI na
GitHubie pokazuje tylko status.

## Utrzymanie

- **Dependabot** otwiera PR-y z aktualizacjami NuGet codziennie (grupy `microsoft-dotnet`, `opentelemetry`). Przy
  kilku PR-ach naraz agent łączy je w worktree i testuje całość. Przy zmianie wersji głównej czyta informacje
  o wydaniu i porównuje je z użyciem w kodzie. Przy podbiciu frameworka testów porównuje liczby testów.
- **Aktualizacja skilli od Matta**: w `D:\Projects\skills` `git fetch`, rebase gałęzi `dotnet` na `origin/main`,
  podbicie wersji w `.claude-plugin/plugin.json`, potem
  `claude plugin marketplace update mattpocock`, `claude plugin update mattpocock-skills@mattpocock` i restart
  sesji (bieżąca sesja nowej wersji nie widzi). Fork musi zostać na gałęzi `dotnet`, bo plugin czyta jego
  katalog roboczy. Kopia zapasowa: remote `backup`.
- **Kopie hooków w innych projektach** (Koszyk `D:\Projects\Koszyk`, sandbox `D:\Projects\ship-sandbox`): źródłem
  jest `.claude/hooks/` tego template'u, jedyne miejsce z testami (`tests/HookTests`). Zmiana hooka idzie tutaj,
  z testem i commitem. Potem `pwsh -NoProfile -File .claude/hooks/sync-hooks.ps1 -Target <katalog projektu>`
  kopiuje wszystkie skrypty do `.claude/hooks/` projektu i zapisuje commit template'u w pliku
  `.claude/hooks/SOURCE` (odmawia, gdy hooki template'u mają niezacommitowane zmiany). Kopię commitujesz
  w projekcie i sprawdzasz przebiegiem `run-dod.ps1 -Check commands`. Nie synchronizuj projektu, w którym
  ficzer czeka na bramkę 2: commit na jego `main` zatrzyma scalenie. Nowy skrypt albo nowy hook wymaga też wpisu w `.claude/settings.json` projektu (rejestracja albo
  allowlista), który dopisujesz sam. Koszyk ma kopie z `7629132`.
- **Zmiana konwencji**: zawsze przez Twoją decyzję (bramka 2, `/retro`), nigdy „przy okazji” innej pracy.
- **Pętla jest zamrożona** (decyzja 2026-11-11): skille i hooki zmieniasz tylko wtedy, gdy prawdziwy ficzer się
  na czymś wyłoży. Bez pilotów i ticketów na zapas.
- **Błąd w kroku deterministycznym `/ship`** (wybór ticketu, liczenie rund, wiersz pomiaru, kroki git bramki 2)
  naprawiasz przeniesieniem tego kroku do skryptu w `.claude/hooks/` z testem w `tests/HookTests`, a skill
  tylko go woła. Kolejne zdanie w skillu jest poprawką tylko dla kroków, które wymagają oceny.
- **Warstwa MAUI w template'cie** (tickety 28–30) jest odłożona do drugiego projektu MAUI. Do tego czasu wzorcem
  jest Koszyk (`add-screen`, DoD mobilne, `screenPaths`).

## Pułapki, które już kosztowały czas

- Docker Desktop staje między sesjami; bez niego testy integracyjne nie są zielone, tylko niezweryfikowane.
  `/ship` uruchamia go sam (`environmentChecks` w `dod.json`).
- Nieudany build i `dotnet test --no-build` w jednej linii bez `&&` dają fałszywie zielone testy na starych
  binariach.
- Nowa niepusta kolumna `DateTime` w istniejącej tabeli: stare wiersze dostają `'-infinity'`, co kończy się 500
  przy zapisie (`add-entity`, „Existing rows”).
- Push wpisany zaraz po podsumowaniu agenta, zanim skończy scalanie, nic nie wysyła („Everything up-to-date”).
- Dwa skille `code-review`: wbudowany (szuka błędów) i Pococka (`mattpocock-skills:code-review`, dwie osie).
  Workflow używa tego drugiego; fork od wersji `1.3.1-dotnet.3` woła go pełną nazwą.
- Plugin `superpowers` dubluje skille Pococka (TDD, debugowanie, review). Jest wyłączony globalnie i dodatkowo
  w `.claude/settings.json` template'u; nie włączaj go obok Pococka.
- Tryb auto nie pozwala agentowi edytować `.claude/settings.json`: wpisy allowlisty dodajesz sam.
- `claude -p` jest nieużywalne: samodzielne CLI nie jest zalogowane.
- W Git Bash ścieżki piszesz ukośnikami zwykłymi (`cd /d/Projects/skills`); `\` znika jako znak ucieczki.
  `git show <rev>:<ścieżka>` poprzedź `MSYS_NO_PATHCONV=1`, inaczej Git Bash przerobi argument na ścieżkę
  Windows.
