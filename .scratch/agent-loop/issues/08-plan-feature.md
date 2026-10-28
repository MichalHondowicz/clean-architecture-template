# 08: `/plan-feature`: planowanie z jednym zatwierdzeniem

**What to build:** Jedną komendą przechodzę grill, spec i tickety w jednej sesji na Opusie. Na końcu
zatwierdzam jeden pakiet, a tickety mają format, który `/ship` wykonuje bez pytań. Po zatwierdzeniu agent
proponuje start pętli.

**Blocked by:** 03

**Seams:** pilot (ticket 10): planowanie ficzera pilota.

**Status:** ready-for-agent

- [ ] `/plan-feature` jest tylko dla człowieka i łączy w jednej sesji grill (z zapisem do `GLOSSARY.md`
      i ADR-ów), spec i tickety (historia 1).
- [ ] Planowanie działa na Opusie (historia 6).
- [ ] Każdy ticket ma sekcję `Seams:` wyprowadzoną z ADR 0004 i kryteria `AC1…n` z planowaną nazwą testu
      (historie 3, 4).
- [ ] Jedna decyzja zatwierdza cały pakiet (spec, tickety, seamy, kryteria, DoD), a po niej tickety mają
      `ready-for-agent` (historia 2).
- [ ] Po zatwierdzeniu agent proponuje uruchomienie `/ship <feature>` (historia 7).
- [ ] `grill-with-docs`, `to-spec` i `to-tickets` dalej działają osobno.
- [ ] Wersja forka podbita.

## Comments

**2026-10-28, decyzje przed implementacją** (bramka w sesji po handoffie
`docs/handoff-2026-10-28-agent-loop-08.md`; wszystkie rekomendacje przyjęte):

**Fakty sprawdzone przed bramką:**

- **Skill tool a `disable-model-invocation`** (próba): wywołanie `mattpocock-skills:to-spec` przez Skill tool
  kończy się odmową („cannot be used with Skill tool due to disable-model-invocation”), a komunikat każe nie
  odtwarzać workflow skilla innymi sposobami. Dokumentacja (strona Skills) mówi to samo ogólnie. O wywołaniu
  jednego skilla z drugiego milczy.
- **`model:` skilla** (dokumentacja): override „applies for the rest of the current turn”, a w następnej turze
  wraca model sesji. Przy `model: opus` pierwsza tura idzie więc na Opusie, a sprawdzenie modelu w tej turze
  zawsze przejdzie.
- **Model sesji:** `get_session("self")` w aplikacji podaje model sesji (`model`) niezależnie od override'u
  z frontmattera. System prompt podaje model bieżącej tury. Model wybrany w aplikacji albo przez `/model`
  trzyma się między turami (przebiegi 03–07, dokumentacja: „the session model resumes”).
- **Cache pluginu** zachowuje układ `skills/engineering/<skill>/`, więc ścieżka `../to-spec/SKILL.md` działa
  także z zainstalowanego pluginu.
- **`effort:`** we frontmatterze nadpisuje effort sesji tylko wtedy, gdy skill jest aktywny.

**Decyzje:**

1. **Opus przez całe planowanie:**
   - frontmatter `/plan-feature` bez `model:` i bez `effort:`;
   - pierwszy krok sprawdza model sesji: `get_session("self")`, gdy aplikacja ma to narzędzie, a bez niego
     linię modelu w system prompcie;
   - jeśli to nie Opus, skill prosi o przełączenie (`/model opus` albo wybór modelu w aplikacji) i kończy turę;
   - przewodnik (09) zaleca effort `high`.
2. **Składanie skilli:**
   - `/plan-feature` wywołuje Skill toolem `grilling` i `domain-modeling`;
   - `to-spec` i `to-tickets` czyta jako pliki (`../to-spec/SKILL.md`, `../to-tickets/SKILL.md`) i wykonuje je
     z różnicami z decyzji 3;
   - wprost zakazuje wywoływania ich przez Skill tool;
   - pliki `grill-with-docs`, `to-spec` i `to-tickets` zostają bez zmian.
3. **Jedno zatwierdzenie:**
   - rundy grilla zostają;
   - uzgodnienie seamów (krok 2 `to-spec`) i quiz podziału (krok 4 `to-tickets`) przechodzą do bramki pakietu;
   - spec i tickety leżą na dysku niezacommitowane, ze statusem `needs-triage`, więc `/ship` ich nie weźmie;
   - po „OK” statusy dostają `ready-for-agent`, a spec, tickety, `GLOSSARY.md`, nowe ADR-y i ewentualna zmiana
     DoD idą w jednym commicie `plan: <feature>`.
4. **Pakiet i decyzja:**
   - w czacie krótkie podsumowanie, czytelne na telefonie: ścieżka speca, a przy każdym tickecie
     `Blocked by`, seamy i `AC1…n` z nazwami testów;
   - do tego DoD z `dod.json`: polecenia, `protectedPaths` i progi limitu;
   - zmiana DoD, której ficzer wymaga (np. `screenPaths`), jest częścią pakietu;
   - brak `dod.json` zatrzymuje skill;
   - zatwierdza tylko jednoznaczne „OK”. Każda inna odpowiedź to poprawki: skill je nanosi i pokazuje pakiet
     jeszcze raz.
5. **Gałąź i propozycja `/ship`:**
   - przed commitem skill sprawdza, czy checkout stoi na `main`, a na innej gałęzi pyta w ramach bramki;
   - na końcu podaje dokładne `/ship <feature>` i zalecane ustawienia sesji: nowa sesja, Sonnet, effort
     `medium`, tryb auto;
   - gdy agent ma narzędzie do zakładania sesji, proponuje start kartą i nigdy nie uruchamia `/ship` sam.
6. **Seamy z ADR 0004:** bez zmian w forku. `to-tickets` bierze seamy ze strategii testów projektu.
   Sprawdzenie w przebiegu (decyzja 8).
7. **Dokumentacja forka:**
   - nowa strona `docs/engineering/plan-feature.md`;
   - wpisy w `README.md` i `skills/engineering/README.md`;
   - łańcuch w `docs/engineering/ship.md` („Where it fits”) z `/plan-feature` zamiast trzech kroków;
   - jedno zdanie w `ask-matt`;
   - bump do `1.3.1-dotnet.18`.
8. **Żywy przebieg** w sandboksie na `main`, w nowej sesji, na mały ficzer z jedną lub dwiema rundami grilla.
   Odpowiada użytkownik. Do sprawdzenia:
   - bramka modelu: celowy start na Sonnecie, potem przełączenie;
   - model w drugiej turze;
   - format ticketów względem `issue-tracker.md`;
   - jeden commit `plan: <feature>`;
   - propozycja `/ship`.

   Samego `/ship` nie uruchamiać. Przebieg tylko na wyraźne słowa użytkownika.

**2026-10-28, decyzja w trakcie implementacji:** `.agents/invocation.md` forka zabrania odwołań
`../skill/SKILL.md` i mówi, że skill tylko dla człowieka nie sięga do innego takiego. Użytkownik wybrał
2(a) z nazwanym wyjątkiem w `invocation.md` zamiast wspólnego skilla z szablonami albo kopii.

**2026-10-28, implementacja** (fork `D:\Projects\skills`, gałąź `dotnet`, commit `86e03d9`, plugin
`1.3.1-dotnet.18`, zainstalowany):

- **`skills/engineering/plan-feature/SKILL.md`:**
  - user-only, bez `model:` i `effort:`;
  - kroki: sprawdzenie projektu, bramka Opusa (model sesji, ponownie przed specem i przed ponownym
    pokazaniem pakietu), szkice przerwanego przebiegu, grill, spec, tickety, pakiet, commit, propozycja
    `/ship`.
- **Szkic:** katalog `.scratch/<feature>/` bez śledzonych plików. Wznowienie zaczyna od pierwszego
  brakującego elementu i sprawdza, czy każda historia speca trafiła do kryteriów któregoś ticketu.
- **Commit:** `git commit -m "plan: <feature>" -- <ścieżki>`. Próba na repozytorium tymczasowym: commit bierze
  tylko pliki pakietu, a plik wcześniej dodany do indeksu przez użytkownika zostaje w indeksie. Przy błędzie
  `git reset -q -- <ścieżki>` i powrót do `needs-triage`.
- **Dokumentacja:**
  - nowa strona `docs/engineering/plan-feature.md`;
  - wpisy w `README.md` i README kubełka, `plugin.json`, `agents/openai.yaml`;
  - łańcuch `plan-feature → ship → retro` w `ship.md`;
  - zdanie w `ask-matt`;
  - wyjątek w `invocation.md` i odnośnik do niego w README.
- **Pliki `to-spec`, `to-tickets` i `grill-with-docs`:** bez zmian.
- **`claude plugin validate . --strict`:** te same dwa stare ostrzeżenia (`CLAUDE.md` w korzeniu, brak
  ikony), nowych nie ma.
- **Template:** bez zmian w kodzie. Pełny build bez ostrzeżeń, testy zielone (ArchitectureTests 6,
  Application.UnitTests 54, IntegrationTests 14, HookTests 83).
- **Review Spec (Sonnet), dwie rundy:**
  - runda 1: 8 uwag w zakresie (wznawianie, kolizja sluga, commit zgarniający indeks użytkownika,
    sprzeczność przy `OK` poza `main`, format statusu speca, dziury bramki modelu, brak plików
    sąsiednich skilli, lista pozostałości przy porzuceniu);
  - runda 2: 7 uwag w zakresie (sprawdzanie modelu przed commitem gubiło `OK`, za szeroka definicja szkicu,
    pliki poprzedniego przebiegu przy wznowieniu, niepełny zestaw ticketów, nieaktualna gałąź przy
    odpowiedzi, stan po nieudanym commicie, brak ponownej propozycji `/ship`) i drobiazgi w dokumentacji.
  - Wszystkie naniesione. Poza zakresem zostały: stara strona `docs/engineering/ask-matt.md` bez `ship`,
    sesja w worktree nigdy nie stoi na `main` (wtedy zawsze `OK here`), a pytania na stronie dokumentacji
    nie pochodzą z obserwacji.
- **Seam (pilot) jeszcze nie sprawdzony na żywo:**
  - model w drugiej turze;
  - format ticketów w praktyce;
  - jeden commit `plan: <feature>`;
  - karta `/ship`: czy `start_session` rozwija polecenie z ukośnikiem.
