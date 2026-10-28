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
| tylko dla pętli: `docs/agents/dod.json` | DoD projektu: sprawdzenia środowiska, komendy buildu i testów, chronione ścieżki, nazwa roli implementera, progi limitu, skill standardów |
| tylko dla pętli: `docs/agents/implementer-contract.md` | wejście, wyjście i zakazy implementera, niezależne od modelu |
| tylko dla pętli: `.claude/hooks/` i ich rejestracja w `.claude/settings.json` | `run-dod.ps1`, `check-diff.ps1`, `wait-for-reset.ps1`, `dod-gate.ps1` (`SubagentStop`), `guard-protected-paths.ps1` (`PreToolUse`) i wspólny `dod-common.ps1`, który cztery z nich wczytują; hooki rejestruje projekt, bo Claude Code ignoruje hooki we frontmatterze agentów z pluginu |
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
   potem `.scratch/<feature>/brief.md` i push na telefon.
5. **Bramka 2**: jedno pytanie w sesji `/ship`, z komputera albo przez Remote Control z telefonu. Odpowiadasz
   jedną wiadomością: „OK” scala, „nie” zostawia gałąź, a każda propozycja konwencji dostaje `tak` albo `nie`
   (np. `OK, 1 tak, 2 nie`):
   - pytanie, uwaga albo odpowiedź niejednoznaczna niczego nie rozstrzyga, także propozycji: agent odpowiada
     i pyta jeszcze raz;
   - propozycja, o której nie wspomnisz, zostaje otwarta i wraca na następnej bramce 2;
   - decyzję agent zapisuje pod `## Decision` w briefie i commituje razem z przyjętymi konwencjami na gałęzi
     ficzera, także przy „nie” (wtedy nic nie idzie do `main`);
   - po „OK” robi fast-forward `main`, `git push origin main`, usuwa worktree i gałąź i sprawdza CI
     w przeglądarce aplikacji. Zatrzymuje się, gdy zdalny `main` poszedł dalej niż baza gałęzi: rebase należy
     do Ciebie.
6. **Po ficzerze**: `/triage` dla nowych ticketów `needs-triage` (decyzja dotycząca wielu use case'ów idzie do
   ADR-a), a po większej porcji pracy `/retro`.

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

- **Push na telefon** z aplikacji Claude przy pięciu zdarzeniach: ficzer gotowy do bramki 2, ticket
  `needs-info`, awaria środowiska (Docker, build bazowy), pauza do resetu tygodniowego i czerwone CI po
  scaleniu (bez cofania czegokolwiek). Treść to jedno zdanie i wskazanie briefu; pełny brief leży
  w `.scratch/<feature>/`.
- Push dociera tylko z sesji z Remote Control `on` i przy włączonych powiadomieniach mobilnych w ustawieniach
  Claude Code (`/config` w terminalowym `claude`). Odpowiedź na bramce 2 przez Remote Control z telefonu
  działa.
- **Znany problem:** od 2026-10-27 pushe z sesji z Remote Control `on` nie dochodzą na telefon, choć narzędzie
  odpowiada „Mobile push requested”. Fallback (hook `Notification` i ntfy) czeka jako ticket 12
  (`needs-triage`); do tego czasu zaglądaj do aplikacji.
- **Pytanie o zgodę w sesji w tle**: aplikacja oznacza sesję jako wymagającą uwagi. Czy to dociera na telefon,
  sprawdzi pilot (ticket 10). Każde takie pytanie to kandydat do allowlisty.

## Limity

- **Progi** są w `docs/agents/dod.json` (`quota`): tydzień 80%, okno 5-godzinne 90%. Pętla nie zaczyna
  ticketu, review gałęzi ani rundy poprawek gałęzi na progu lub powyżej, żeby zostało Ci na planowanie
  następnego ficzera. W pilocie limit symulujesz, obniżając próg tuż pod bieżące zużycie.
- **Okno 5-godzinne**: pętla zapisuje pauzę w `metrics.md` i czeka w tej samej sesji. Skrypt
  `wait-for-reset.ps1` trzyma komputer w czuwaniu i kończy się 2 minuty po resecie, a sesja rusza dalej
  sama, bez pusha. Wyjątek: gdy dwa czekania z rzędu nie zwolniły okna, trzeciego nie ma. To awaria
  środowiska (`5-hour window`) z pushem.
- **Limit tygodniowy**: pauza w `metrics.md`, jeden push z godziną resetu i czyste zakończenie sesji. Po
  resecie wpisujesz `/ship <feature>` sam. To samo po restarcie aplikacji w trakcie czekania na okno
  5-godzinne: czekająca sesja nie przeżywa restartu.
- Wznawianie przez zadania zaplanowane aplikacji nie działa: zadanie nie rozwija komendy tylko dla
  człowieka, startuje na modelu sesji, która je utworzyła, i nie trzyma komputera w czuwaniu.
- **Pomiar**: `.scratch/<feature>/metrics.md`, wiersz na ticket (zużycie obu okien na starcie i końcu, rundy,
  model w każdej roli, czas, wynik), wiersz na pauzę i na review gałęzi, a na końcu suma. Start i wznowienie
  to ta sama komenda: stan to statusy ticketów i ich `## Comments`.

## Wznawianie i stany pośrednie

- **Gdzie jest stan.** Do scalenia tickety, ich `## Comments`, `brief.md`, `metrics.md` i pliki findings leżą
  w commitach gałęzi `feat/<feature>`, w worktree `.claude/worktrees/<feature>`. W głównym checkoucie
  `.scratch/<feature>/` ich nie ma: przy komputerze zaglądasz do worktree.
- **Przerwany przebieg** (zamknięty laptop, restart aplikacji, limit tygodniowy): `/ship <feature>` w głównym
  checkoucie. Ticket `in-progress` idzie pierwszy, od następnej rundy (rundy liczą pliki findings), na bazie
  z commita `ship: start <NN>`. Niezacommitowane resztki w worktree bez ticketu `in-progress` idą do stasha
  `ship: leftovers`, nazwanego w raporcie.
- **Po `needs-info`**: gdy choć jeden ticket ficzera nie jest `done` (nie liczą się `needs-triage`
  i `wontfix`), przebieg kończy się bez review gałęzi, briefu i bramki 2. Ticket zachowuje swoje cofnięte
  commity w historii i stash nazwany w briefie. Odblokowanie (decyzja, przywrócenie `ready-for-agent`, commit
  na gałęzi w worktree, bo niezacommitowaną zmianę `/ship` zestashuje) nie jest jeszcze opisane w skillu.
  Ticket po trzech rundach ma trzy pliki findings, więc `/ship` uzna go od razu znowu za `stuck`. Drogę ustali
  pilot (ticket 10) albo ticket 13.
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
- **Zmiana konwencji**: zawsze przez Twoją decyzję (bramka 2, `/retro`), nigdy „przy okazji” innej pracy.

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
