# Plan: template .NET gotowy do pracy z agentem (konwencje projektu + proces Pococka)

Data: 2026-10-12. Język dokumentu: polski. Identyfikatory, ścieżki i commity: angielskie.

## Cel i efekt końcowy

Nowy projekt zakładany z tego template'u od pierwszej sesji ma:

1. **Warstwę konwencji** (CO jest dobrym kodem w tym projekcie): `AGENTS.md`, `CLAUDE.md`, skille projektowe
   `add-feature`, `add-entity`, `add-tests`, `ca-review`, allowlistę uprawnień.
2. **Warstwę domeny** (JAK mówimy o projekcie): `GLOSSARY.md` i ADR-y w `docs/adr/` z decyzjami, które już
   zapadły w template'cie.
3. **Warstwę procesu** (JAK pracujemy z agentem): skille Matta Pococka (`grill-with-docs` → `to-spec` →
   `to-tickets` → `implement` (z `tdd`) → `code-review`), dostosowane do .NET.

Kryterium sukcesu (Faza 5): jedna mała funkcja, np. „archiwizacja zadania”, przechodzi przez cały przepływ.
Build ma 0 ostrzeżeń, wszystkie testy (unit, architecture, integration) są zielone, `ca-review` nie ma uwag,
`GLOSSARY.md` jest zaktualizowany, a użytkownik nie musiał poprawiać agenta w kwestii konwencji.

## Stan zastany (ustalony 2026-10-12)

| Zasób | Stan |
|---|---|
| `D:\Projects\clean-architecture_new` | Świeża kopia template'u Milana Jovanovića (.NET 10). **Nie jest repo git.** Brak `AGENTS.md`, `CLAUDE.md`, `.claude/`, `docs/`. Kod `src/` i `Directory.Build.props` **identyczne** z `D:\Projects\clean-architecture` (sprawdzone `diff -rq`, różnią się tylko `bin/` i `obj/`). |
| `D:\Projects\clean-architecture` | Ta sama baza kodu plus gotowa warstwa konwencji: `AGENTS.md` (84 linie, „konstytucja”), `CLAUDE.md` (`@AGENTS.md`), `.claude/skills/{add-feature,add-entity,add-tests,ca-review}` (z `README.md`), `.claude/settings.json` (allowlista `dotnet`/`git`), `.config/dotnet-tools.json` (dotnet-ef), `docs/tasks/_template.md`. Aktywny branch `e2e/hybrid`, nieśledzone `.agents/skills/` (kopie 4 skilli) i `docs/tasks/research-projects.md`. |
| Plugin `dotnet-workflow` (`D:\Projects\dotnet-workflow`, marketplace `hondo-local`) | v0.4.0, **wyłączony** w `~/.claude/settings.json` (`"dotnet-workflow@hondo-local": false`). Branch `hybrid-clean-architecture` niescalony, E2E nieukończone (patrz `D:\Projects\dotnet-workflow\docs\handoff-2026-10-02-hybrid-e2e-start.md`). Ma własny proces (engineering-manager / architect / implementer / reviewer) i ~22 skille, m.in. `writing-xunit-tests`, `implementing-cqrs-no-mediator`, `implementing-ef-core`. |
| Plugin `mattpocock-skills` 1.3.1 | Włączony (z marketplace `mattpocock`, GitHub). Lokalny klon upstreamu: `D:\Projects\skills` (`main`, bez własnych zmian). |
| Plugin `superpowers` 6.4.1 | Włączony. Hook SessionStart wstrzykuje „using-superpowers” do każdej sesji. Funkcjonalnie dubluje Pococka (tdd, debugowanie, planowanie, code review). |

**Wniosek:** warstwa konwencji już istnieje i jest dopracowana (przeszła kilka rund review). Nie budujemy jej
od nowa, tylko przenosimy i odcinamy od zależności od `dotnet-workflow`. Właściwa nowa praca to warstwa domeny,
dostosowanie Pococka do .NET i rozstrzygnięcie, który proces jest główny.

## Fazy

### Faza 0: bramka decyzyjna (przed jakąkolwiek zmianą)

Zadaj użytkownikowi pytania D1–D6 jednym formularzem (AskUserQuestion, maksymalnie 4 pytania na formularz,
więc dwa formularze). Rekomendacje są w pierwszej opcji.

- **D1. Rola `clean-architecture_new`.**
  (a) Nowy, czysty template „v2”: warstwa konwencji + Pocock, bez `dotnet-workflow`. Stary katalog zostaje
  poligonem `dotnet-workflow`. **Rekomendacja.**
  (b) Porzucić `_new` i dobudować warstwę Pococka w starym `clean-architecture`.
- **D2. Główny proces pracy z agentem.**
  (a) Pocock (mały, składany, łatwy do dopasowania). `dotnet-workflow` zostaje zamrożony, a jego wiedza .NET
  (np. `writing-xunit-tests`) służy jako materiał do Fazy 4. **Rekomendacja.**
  (b) Najpierw dokończyć E2E `dotnet-workflow` (handoff z 2026-10-02), a z Pococka wziąć tylko grilling,
  `GLOSSARY.md` i ADR-y.
  (c) superpowers.
- **D3. superpowers w projektach .NET.**
  (a) Wyłączyć per projekt (`"enabledPlugins": {"superpowers@claude-plugins-official": false}` w
  `.claude/settings.json` template'u), globalnie zostawić. **Rekomendacja.**
  (b) Wyłączyć globalnie. (c) Zostawić.
- **D4. Interfejsy z jedną implementacją** (`IPasswordHasher`, `ITokenProvider`, `IUserContext`,
  `IDateTimeProvider`) kontra reguła Pococka „jeden adapter to hipotetyczny seam”.
  (a) Zostają. ADR: interfejsy w `Application` wynikają z odwrócenia zależności między warstwami, egzekwowanego
  testami architektury. **Rekomendacja.**
  (b) Usuwać, gdzie się da.
- **D5. Issue tracker dla `to-spec`/`to-tickets`.**
  (a) Lokalny markdown w repo (Pocock: `.scratch/<feature>/`). **Rekomendacja na start.**
  (b) GitHub Issues. (c) Inny (np. Azure DevOps, wtedy opis workflow prozą).
- **D6. Gdzie żyją dostosowania Pococka do .NET (Faza 4).**
  (a) Branch `dotnet` w klonie `D:\Projects\skills`, zarejestrowany jako lokalny marketplace zamiast upstreamu.
  Aktualizacje od Matta przez `git fetch` + `rebase`. **Rekomendacja.**
  (b) Kopie skilli w `.claude/skills/` template'u (`npx skills add`, edytowalne, plugin wyłączony per projekt).
  (c) Drugi plugin w marketplace `hondo-local`. Odradzane: kolizja nazw `tdd`/`code-review` z oryginałem.

Zatrzymaj się na odpowiedziach. Dalsze fazy zakładają rekomendacje. Inna odpowiedź oznacza korektę planu
w tym pliku przed Fazą 1.

### Faza 1: bootstrap repo

1. `git init` w `D:\Projects\clean-architecture_new`. Sprawdź, że `.gitignore` wyklucza `bin/`, `obj/`.
2. Commit bazowy: `chore: template baseline (Clean Architecture .NET 10)`.
3. Weryfikacja (wymaga Dockera, najpierw `docker info`):
   `dotnet build CleanArchitecture.slnx`, potem `dotnet test CleanArchitecture.slnx`.
   Zapisz własne liczby testów. Ostatnie znane dla tej bazy kodu (stary katalog, 2026-09-30): build 0/0,
   UnitTests 22/22, ArchitectureTests 6/6, IntegrationTests 8/8 (ta ostatnia liczba niepotwierdzona).

### Faza 2: przeniesienie warstwy konwencji ze starego template'u

Źródło: `D:\Projects\clean-architecture` (branch `main`, nie `e2e/hybrid`; użyj `git show main:<ścieżka>`).

1. Skopiuj: `.claude/skills/` (4 skille + `README.md` + `references/`), `.claude/settings.json`,
   `.config/dotnet-tools.json`, `CLAUDE.md`.
2. `AGENTS.md`: skopiuj i **odetnij zależności od `dotnet-workflow`**:
   - sekcja „Language convention”: usuń akapit o machine-read tokens (`**Skill:**`, `### Phase N:`, `✅`/`⏸`
     itd.), bo to format planów `dotnet-workflow`. Zostaw regułę językową.
   - zdanie „A workflow plan should cut phases along these skills…” zamień na mapowanie na Pococka
     (patrz Faza 3, punkt 5).
   - sekcja „Tasks” (`/dotnet-workflow:implement`, `--auto`, batch): usuń albo zastąp opisem trackera z D5.
3. Przejrzyj 4 skille pod kątem odwołań do `dotnet-workflow`
   (`grep -rn "dotnet-workflow\|Phase\|--auto" .claude/skills`) i usuń je.
4. D3: dopisz do `.claude/settings.json` wyłączenie superpowers per projekt.
5. `dotnet tool restore`, build, commit: `chore: agent convention layer (AGENTS.md, project skills, allowlist)`.

### Faza 3: warstwa domeny i spięcie z Pocockiem

1. Uruchom `/setup-matt-pocock-skills` (tracker z D5, domyślne etykiety, single-context).
   Powstaje `docs/agents/*.md` i blok `## Agent skills` w `CLAUDE.md`.
   **Uwaga:** `CLAUDE.md` to tylko `@AGENTS.md`. Zdecyduj, czy blok trafia do `CLAUDE.md` (skill tak zrobi),
   czy przenieść go do `AGENTS.md`. Rekomendacja: `AGENTS.md`, żeby konstytucja była w jednym pliku.
2. ADR-y w `docs/adr/` (format Pococka: krótko, kontekst → decyzja → konsekwencje). Każdy ADR to decyzja,
   którą skille Pococka inaczej podważą:
   - `0001-interfaces-for-layer-inversion.md` (D4); źródło konfliktu: `codebase-design`,
     `improve-codebase-architecture`.
   - `0002-result-over-exceptions.md` (`Result`/`Error`, `CustomResults.Problem`, mapowanie kodów HTTP).
   - `0003-cqrs-without-mediatr-decorators.md` (własne `ICommandHandler`/`IQueryHandler`, Scrutor, dekoratory
     walidacji i logowania).
   - `0004-test-strategy.md`: logika handlera w testach unit na `TestDbContext` (EF InMemory). Wszystko, co
     zależy od relacyjnej bazy (unikalność, ograniczenia, transakcje, migracje), w testach integracyjnych
     (Testcontainers). Integracja przez HTTP to preferowany „seam” dla `tdd`. Źródło konfliktu: `tdd`
     („test przez publiczny interfejs, bez mockowania własnych klas”).
   - `0005-ownership-returns-failure.md`: `UserErrors.Unauthorized()` to `ErrorType.Failure`, więc 500,
     a nie 401/403 (fakt z `AGENTS.md`, wart osobnej decyzji: utrwalić czy zmienić).
3. `GLOSSARY.md`: zbuduj z kodu (`Domain/`, `SharedKernel/`) przez skill `domain-modeling`, a potem krótki
   `/grill-with-docs` z użytkownikiem, żeby wyostrzyć pojęcia (Todo vs TodoItem, Permission, Refresh token
   rotation, Domain Event).
4. W `AGENTS.md` dodaj sekcję „How the process skills use this project”:
   - jednostka pracy `to-tickets` = jeden use case = jedno wywołanie `add-feature` (encja: `add-entity` przed
     nim);
   - `implement`/`tdd`: „typecheck” = `dotnet build CleanArchitecture.slnx` (analizatory jako błędy);
     pojedynczy test = `dotnet test <projekt> --filter "FullyQualifiedName~<Nazwa>"`; pełny suite na koniec;
   - `code-review`, oś Standards: źródłem standardów jest `AGENTS.md` + `ca-review`.
5. Commit: `docs: glossary, ADRs and process mapping for agent skills`.

### Faza 4: Pocock po .NET (wariant D6a)

W `D:\Projects\skills`, branch `dotnet` od `main`. Zmieniaj możliwie mało plików, żeby rebase na upstream był
tani. Reguła repo: bez em-dashy w prozie.

1. `skills/engineering/tdd/tests.md`, `mocking.md`: przykłady w C# (xUnit, Shouldly, NSubstitute,
   `WebApplicationFactory`, Testcontainers). Dobry test: wzorowany na
   `tests/IntegrationTests/Todos/TodosTests.cs`. Mock tylko na granicy systemu: `IDateTimeProvider` /
   `TimeProvider`, zewnętrzne API. Ostrzeżenie o EF InMemory. Materiał pomocniczy:
   `D:\Projects\dotnet-workflow\plugins\dotnet-workflow\skills\writing-xunit-tests\SKILL.md`, sekcja
   „Tests that pass for the wrong reason”.
2. `skills/engineering/codebase-design/SKILL.md`: przykłady C#; doprecyzuj, że interfejs w sensie Pococka to
   nie słowo kluczowe `interface`, a interfejsy wymuszone odwróceniem zależności między warstwami opisuje
   ADR projektu.
3. `skills/engineering/implement/SKILL.md`: „typechecking” → „build (in .NET: `dotnet build`, analyzers as
   errors)”.
4. `skills/engineering/setup-matt-pocock-skills/SKILL.md`: sygnały monorepo `.sln`/`.slnx` z wieloma
   projektami spoza `tests/`; opcjonalnie szablon `issue-tracker-azure-devops.md`, jeśli D5 = Azure DevOps.
5. `skills/engineering/prototype/SKILL.md`: dopisz file-based app .NET 10 (`dotnet run app.cs`).
6. `skills/engineering/wizard/SKILL.md`: wariant PowerShell na Windows.
7. `claude plugin validate . --strict`. Podbij wersję w `.claude-plugin/plugin.json` (np. `1.3.1-dotnet.1`).
   Przełącz instalację. `.claude-plugin/marketplace.json` klonu ma nazwę `mattpocock`, czyli tę samą co
   zarejestrowany marketplace z GitHuba, więc najpierw usuń stary:
   `claude plugin uninstall mattpocock-skills@mattpocock`, `claude plugin marketplace remove mattpocock`,
   `claude plugin marketplace add D:\Projects\skills`, `claude plugin install mattpocock-skills@mattpocock`,
   restart sesji. Te kroki zmieniają globalną konfigurację: wykonuje je użytkownik albo agent za jego zgodą.

### Faza 5: pilot end-to-end

W `clean-architecture_new`, na branchu `pilot/archive-todo`:

1. `/grill-with-docs` dla „archiwizacja zadania” (kto może, czy przywracanie, czy zarchiwizowane znikają
   z listy, cache).
2. `/to-spec` → `/to-tickets` (tracker z D5).
3. `/implement` dla ticketów. Obserwuj: czy wywołał `add-feature`/`add-entity`, czy pierwszy test był czerwony,
   czy build przeszedł bez suppressions.
4. `/code-review` (przeciw `main`).
5. Kryteria z sekcji „Cel”. Zapisz wynik i lekcje w `docs/pilot-2026-10-archive-todo.md`, potem `/retro`.

### Faza 6: porządki

- Decyzja o `dotnet-workflow`: zamrozić (README: „paused, see clean-architecture_new”) albo wrócić do jego E2E.
- Stary `D:\Projects\clean-architecture`: zostawić jako poligon albo zarchiwizować.
- Czy `clean-architecture_new` publikować jako własny template (`dotnet new` lub GitHub template repo).

## Ryzyka

- Trzy warstwy procesu naraz (Pocock, superpowers, `dotnet-workflow`) dają niedeterministyczny wybór skilli.
  D2/D3 muszą zapaść przed Fazą 5, inaczej pilot nic nie mierzy.
- Fork Pococka (D6a) wymaga ręcznych rebase'ów. Ograniczać zmiany do plików z Fazy 4.
- Testy integracyjne wymagają Dockera. Bez niego Faza 1 i 5 nie mają pełnego dowodu.
