# Workflow: praca z agentem w tym template'cie

Template Clean Architecture na .NET 10 (baza: template Milana Jovanovića), przygotowany do pracy z Claude Code.
Jest na własny użytek i nie będzie publikowany. Ten dokument opisuje, jak z niego korzystać: dla Ciebie, nie
dla agenta. Agent ma swoje zasady w `AGENTS.md`.

## Trzy warstwy i gdzie leżą

| Warstwa | Pytanie | Pliki |
|---|---|---|
| Konwencje | co jest dobrym kodem w tym projekcie | `AGENTS.md` (konstytucja), `.claude/skills/` (`add-feature`, `add-entity`, `add-tests`, `ca-review`), `.claude/settings.json` (allowlista komend) |
| Domena | jak mówimy o projekcie i co już postanowiliśmy | `GLOSSARY.md`, `docs/adr/` |
| Proces | jak pracujemy z agentem | skille Matta Pococka z forka `D:\Projects\skills` (gałąź `dotnet`), opisane w `AGENTS.md`, sekcja „Agent skills”, i w `docs/agents/` |

Skille procesu są globalne: plugin `mattpocock-skills` wskazuje na fork i działa w każdym Twoim projekcie.
Projekt mówi im o sobie przez `AGENTS.md` (np. typecheck to `dotnet build`, jeden ticket to jeden use case) i
`docs/agents/` (tracker w `.scratch/`, statusy ticketów, słownik i ADR-y).

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
     (robi to `/grill-with-docs`).

## Istniejący projekt

Warstwa procesu jest globalna (plugin z forka Pococka, wyłączony `superpowers`), więc działa w każdym
projekcie od razu. Per projekt trzeba dołożyć to, z czego skille procesu czytają kontekst:

| Element | Po co |
|---|---|
| `docs/agents/` (przez `/setup-matt-pocock-skills`) | gdzie leżą tickety, jakie są statusy, gdzie słownik i ADR-y |
| sekcja „Process skills in this project” w `AGENTS.md` | komendy build i testów, review ticketu tylko na osi Spec, pełna nazwa `mattpocock-skills:code-review` |
| `GLOSSARY.md` i `docs/adr/` | słownik domeny i decyzje, których agent nie podważa |
| `CLAUDE.md` = `@AGENTS.md` plus zdanie, że reguły idą do `AGENTS.md` | jedno źródło reguł |
| `.claude/settings.json` z allowlistą komend | mniej pytań o zgodę przy buildzie i testach |

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
5. **Pilot**: jedna mała funkcja przez cały przepływ z sekcji niżej, potem `/retro`. Tak wychodzą rozjazdy
   między `AGENTS.md` a rzeczywistością projektu.

`AGENTS.md` z template'u nie kopiuj w całości: każdy projekt ma własną konstytucję o swojej architekturze,
a z template'u przenosisz tylko proces. Długi `AGENTS.md` przytnij przy okazji: agent czyta go w całości na
starcie każdej sesji, więc każda zbędna linia kosztuje.

## Funkcja od pomysłu do `main`

Komendy z ukośnikiem wpisujesz Ty, jako osobną wiadomość zaczynającą się od `/` (to skille „user-only”, agent
nie może ich wywołać sam).

1. **`/grill-with-docs <temat>`**: agent przepytuje Cię z pomysłu, aż wszystko jest jasne, i od razu zapisuje
   pojęcia w `GLOSSARY.md` oraz decyzje jako ADR.
2. **`/to-spec`**: spec z rozmowy, w `.scratch/<feature>/spec.md`.
3. **`/to-tickets`**: tickety w `.scratch/<feature>/issues/NN-<slug>.md`, jeden use case na ticket, każda
   historia speca w kryteriach jakiegoś ticketu.
4. **`/triage`** (dla ticketów `needs-triage`, np. z review): decydujesz o statusie (`ready-for-agent`,
   `needs-info`, `wontfix` i inne z `docs/agents/triage-labels.md`). Decyzja dotycząca wielu use case'ów idzie
   do ADR-a.
5. **Gałąź** `fix/<slug>` albo `feat/<slug>` od `main`; agent może ją utworzyć.
6. **`/implement <ścieżka ticketu>`**:
   - agent proponuje **seamy** testów (gdzie test obserwuje zachowanie), a Ty je zatwierdzasz; bez tego nie
     napisze testu;
   - cykle red → green (`tdd`), zwykle od testu HTTP; red to padający test, nie błąd kompilacji;
   - testy, które nie były czerwone w cyklu, dostają **red proof**: agent psuje jeden mechanizm, patrzy, jak
     test pada, i przywraca plik (hash przed i po);
   - review ticketu tylko na osi Spec (czy zrobione to, o co prosił ticket), skillem
     `mattpocock-skills:code-review`. Wbudowany w Claude Code `/code-review` to inny skill: szuka błędów
     w diffie i nie ma osi Spec. Sam review Pococka wywołasz jako `/mattpocock-skills:code-review`.
7. **`ca-review` całej gałęzi** (oś Standards: konwencje z `AGENTS.md` i checklista skilla), z `base <sha>`
   początku gałęzi.
8. **Zamknięcie ticketu**: odhaczone kryteria, wynik w `## Comments`, `**Status:** done`.
9. **Scalenie**: agent robi fast-forward do `main` za Twoją zgodą i mówi, kiedy skończył.
10. **Ty**: `git push origin main` i `git branch -d <gałąź>`, dopiero po komunikacie o scaleniu. Potem CI
    (`.github/workflows/build.yml`, build Release i testy) powinno być zielone.

Po większej porcji pracy: **`/retro`** (wnioski z sesji do konwencji, przez bramkę), a na koniec sesji
„kończymy” albo „zapisz stan”: skill `handoff` zapisuje w repo stan i prompt startowy dla następnej sesji.

## Co robisz Ty, a co agent

| Ty | Agent |
|---|---|
| komendy z ukośnikiem z listy wyżej | `tdd`, `code-review`, `ca-review`, `domain-modeling`, `add-*` sam |
| decyzje na bramkach: seamy, statusy ticketów, nowe konwencje, ADR-y | propozycje z rekomendacją, zapis decyzji |
| `git push`, `git branch -d` (tryb auto blokuje je agentowi) | commity, gałęzie, scalenie fast-forward za zgodą |
| uruchomienie Docker Desktop, gdy agent o to prosi | sprawdzenie `docker info` przed testami integracyjnymi |
| aktualizacja pluginu Pococka i restart sesji | zmiany w forku, commit na gałęzi `dotnet` |

## Weryfikacja

```bash
dotnet build CleanArchitecture.slnx && dotnet test CleanArchitecture.slnx --no-build
```

Analizatory są błędami (każde ostrzeżenie psuje build) i nigdy się ich nie wycisza. Testy: `UnitTests`
(handlery na EF InMemory), `ArchitectureTests` (kierunek zależności warstw), `IntegrationTests` (HTTP na
PostgreSQL). Liczby testów licz lokalnie: CI na GitHubie pokazuje tylko status.

## Utrzymanie

- **Dependabot** otwiera PR-y z aktualizacjami NuGet codziennie (grupy `microsoft-dotnet`, `opentelemetry`). Przy
  kilku PR-ach naraz agent łączy je w worktree i testuje całość. Przy zmianie wersji głównej czyta informacje
  o wydaniu i porównuje je z użyciem w kodzie. Przy podbiciu frameworka testów porównuje liczby testów.
- **Aktualizacja skilli od Matta**: w `D:\Projects\skills` `git fetch`, rebase gałęzi `dotnet` na `origin/main`,
  podbicie wersji w `.claude-plugin/plugin.json`, potem
  `claude plugin marketplace update mattpocock`, `claude plugin update mattpocock-skills@mattpocock` i restart
  sesji. Fork musi zostać na gałęzi `dotnet`, bo plugin czyta jego katalog roboczy. Kopia zapasowa: remote
  `backup`.
- **Zmiana konwencji**: zawsze przez Twoją decyzję (bramka), nigdy „przy okazji” innej pracy.

## Pułapki, które już kosztowały czas

- Docker Desktop staje między sesjami; bez niego testy integracyjne nie są zielone, tylko niezweryfikowane.
- Nieudany build i `dotnet test --no-build` w jednej linii bez `&&` dają fałszywie zielone testy na starych
  binariach.
- Nowa niepusta kolumna `DateTime` w istniejącej tabeli: stare wiersze dostają `'-infinity'`, co kończy się 500
  przy zapisie (`add-entity`, „Existing rows”).
- Push wpisany zaraz po podsumowaniu agenta, zanim skończy scalanie, nic nie wysyła („Everything up-to-date”).
- Dwa skille `code-review`: wbudowany (szuka błędów) i Pococka (`mattpocock-skills:code-review`, dwie osie).
  Workflow używa tego drugiego; fork od wersji `1.3.1-dotnet.3` woła go pełną nazwą.
- Plugin `superpowers` dubluje skille Pococka (TDD, debugowanie, review). Jest wyłączony globalnie i dodatkowo
  w `.claude/settings.json` template'u; nie włączaj go obok Pococka.
- W Git Bash ścieżki piszesz ukośnikami zwykłymi (`cd /d/Projects/skills`); `\` znika jako znak ucieczki.
