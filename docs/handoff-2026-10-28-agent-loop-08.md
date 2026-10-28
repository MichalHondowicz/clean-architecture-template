> Nieaktualny. Wykonaj docs/handoff-2026-10-28-agent-loop-09.md

# Handoff: pętla bez nadzoru, ticket 08 (`/plan-feature`: planowanie z jednym zatwierdzeniem)

## 1. Stan wejściowy

- **Template** `D:\Projects\clean-architecture_new`:
  - lokalna gałąź `chore/agent-loop`, niewypchnięta; HEAD to commit tego handoffu, na `cfcbbee`;
  - drzewo czyste poza `.agents/` i `.codex/`, które celowo zostają nieśledzone.
- **Tickety** w `.scratch/agent-loop/issues/`:
  - 01–07: `done`;
  - 08, 09: `ready-for-agent`;
  - 10, 11: `ready-for-human`;
  - 12: `needs-triage`.
- **Ostatni własny pełny przebieg**, przy implementacji ticketu 07: build bez błędów i ostrzeżeń, testy zielone
  (ArchitectureTests 6, Application.UnitTests 54, IntegrationTests 14, HookTests 83). Od tamtej pory w
  template'cie zmieniły się tylko `.scratch/` i `.claude/settings.json` (allowlista).
- **Fork skilli** `D:\Projects\skills`, gałąź `dotnet`, HEAD `e47e934`, drzewo czyste. Zainstalowany plugin
  `mattpocock-skills` to 1.3.1-dotnet.17.
- **Sandbox** `D:\Projects\ship-sandbox`:
  - checkout główny na `chore/agent-loop` (`36d3411`);
  - `origin` to goły klon `D:\Projects\ship-sandbox-origin.git`, a `main` lokalnie i na origin stoi na `87b0b81`
    (ficzer `toy-brief` scalony w przebiegu D ticketu 07);
  - worktree `toy-overdue` i `toy-quota` są skończone i można je usunąć;
  - zostaje pusty folder `.claude\worktrees\toy-brief`, który trzyma proces sesji przebiegu D. Usuwa go
    użytkownik.
- **Limit przy handoffie:** okno 5-godzinne 17% (reset 28.10 o 11:30), tydzień 46% (reset 30.10 o 07:00).

## 2. Co robimy dalej

Ticket **08: `/plan-feature`: planowanie z jednym zatwierdzeniem**, plik
`.scratch/agent-loop/issues/08-plan-feature.md`. Ticket istnieje i ma status `ready-for-agent`. Seam to pilot
(ticket 10), czyli planowanie ficzera pilota. Najpierw bramka w sesji: pytania z sekcji 4 w jednej rundzie.
Potem użytkownik wpisuje `/mattpocock-skills:implement <ścieżka ticketu>`.

Po 08 zostaje 09 (porządki i przewodnik), a potem pilot 10 i rozpisanie etapu 2 (11), oba należą do
użytkownika. Ticket 12 czeka na triage i niczego nie blokuje.

## 3. Co przeczytać na start

- **`.scratch/agent-loop/issues/08-plan-feature.md`:** kryteria.
- **`.scratch/agent-loop/spec.md`:**
  - historie 1–7 (sekcja „Planowanie”);
  - decyzje „Podział odpowiedzialności” (oba nowe skille tylko dla człowieka), „`/plan-feature`” oraz tabela
    „Role i modele”;
  - „Testing Decisions”, seam 1 (pilot).
- **`## Comments` ticketu 03:** dopisek „Do 05”: `model:` skilla obowiązuje tylko w pierwszej turze. To fakt
  wejściowy do pytania 1.
- **Fork `D:\Projects\skills`:**
  - `skills/engineering/grill-with-docs/SKILL.md`: wywołuje przez Skill tool `grilling` i `domain-modeling`;
  - `skills/engineering/to-spec/SKILL.md`: krok 2 uzgadnia seamy z użytkownikiem, krok 3 nadaje
    `ready-for-agent`;
  - `skills/engineering/to-tickets/SKILL.md`: seamy i `AC1…n` z planowaną nazwą testu już są (szablon ticketu),
    krok 4 iteruje z użytkownikiem do zatwierdzenia podziału, krok 5 publikuje z `ready-for-agent`;
  - `skills/engineering/ship/SKILL.md`, *Start or resume*: `/ship` wymaga zacommitowanych ticketów i zakłada
    worktree z bieżącego HEAD, a bramka 2 wymaga, żeby `origin/main` był przodkiem gałęzi;
  - frontmatter `ship/SKILL.md`: wzór `disable-model-invocation`, `model:`, `effort:`, `argument-hint:`.
- **`docs/agents/issue-tracker.md`**, sekcja „Ticket format for unattended work”: format, który `/ship` wykonuje.
- **`docs/agents/dod.json`:** DoD projektu, część pakietu do zatwierdzenia (historia 2).
- **`docs/adr/0004-*.md`:** strategia testów, z której ticket wyprowadza seamy.

## 4. Otwarte pytania na bramkę

Rozstrzygnij je z użytkownikiem w jednej rundzie, zanim zaczniesz implementację. Fakty oznaczone „sprawdź”
sprawdź przed rundą, nie z pamięci.

1. **Opus przez całe planowanie.** Fakt z ticketu 03: `model:` skilla obowiązuje tylko w pierwszej turze, a grill
   ma wiele tur. Warianty:
   - (a) `model: opus` we frontmatterze, a pierwszy krok skilla sprawdza model sesji. Gdy to nie Opus, prosi
     o przełączenie modelu sesji i kończy turę;
   - (b) skill przełącza model sam narzędziem aplikacji. To narzędzie jest tylko w aplikacji desktop, więc
     generyczny fork go nie zakłada;
   - (c) tylko zdanie w przewodniku (09).

   Rekomendacja: (a). Sprawdź, czy przełączenie modelu sesji w aplikacji trzyma się między turami i czy model
   widzi, na jakim modelu działa (system prompt podaje model).
2. **Jak `/plan-feature` składa istniejące skille.** `grill-with-docs`, `to-spec` i `to-tickets` mają
   `disable-model-invocation: true`. Sprawdź, czy to blokuje ich wywołanie przez Skill tool z innego skilla. Jeśli
   blokuje, warianty:
   - (a) `/plan-feature` wywołuje `grilling` i `domain-modeling` (bez blokady), a `to-spec` i `to-tickets` czyta
     jako pliki (ścieżka względem katalogu skilla) i wykonuje z wypisanymi różnicami (pytanie 3);
   - (b) kopia ich treści w `/plan-feature`, co grozi rozjazdem;
   - (c) zdjęcie blokady, wbrew decyzji spec „nic nie uruchamia się samo”.

   Rekomendacja: (a). Kryterium „`grill-with-docs`, `to-spec` i `to-tickets` działają osobno” wymaga, żeby ich
   pliki zostały bez zmian albo zmiany nie ruszały ich samodzielnej ścieżki.
3. **Jedno zatwierdzenie zamiast trzech.** Dziś `to-spec` uzgadnia seamy, a `to-tickets` iteruje nad podziałem,
   i oba nadają `ready-for-agent`. Rekomendacja:
   - rundy grilla zostają, bo to jest grill;
   - uzgodnienie seamów i podziału przechodzi do bramki pakietu;
   - spec i tickety leżą na dysku jako szkic, niezacommitowane, ze statusem `needs-triage`, więc `/ship` ich nie
     weźmie;
   - po „OK” statusy dostają `ready-for-agent`, a spec, tickety, `GLOSSARY.md` i nowe ADR-y idą w jednym commicie
     `plan: <feature>`.

   Wariant: pakiet tylko w czacie, pliki dopiero po zatwierdzeniu. Gorzej się go czyta na telefonie.
4. **Co zawiera pakiet i jak wygląda decyzja.** Rekomendacja:
   - w czacie krótkie podsumowanie, czytelne na telefonie: ścieżka speca, lista ticketów z `Blocked by`, seamami
     i kryteriami z nazwami testów, oraz DoD z `dod.json` (polecenia, `protectedPaths`, progi limitu);
   - jeśli ficzer wymaga zmiany DoD (np. `screenPaths`), zmiana jest częścią pakietu i wchodzi w commit
     `plan: <feature>`. Brak `dod.json`: stop, bo `/ship` i tak nie ruszy;
   - zatwierdza tylko jednoznaczne „OK”. Każda inna odpowiedź to poprawki: skill nanosi je i pokazuje pakiet
     jeszcze raz.
5. **Gałąź commitu i propozycja `/ship`.** `/ship` zakłada worktree z bieżącego HEAD, a bramka 2 wymaga, żeby
   `origin/main` był przodkiem gałęzi. Commit planu na innej gałęzi niż `main` wniósłby jej commity do ficzera.
   Rekomendacja:
   - przed commitem sprawdzić, że checkout stoi na `main`. Na innej gałęzi zapytać, bo to część bramki;
   - na końcu podać dokładne polecenie `/ship <feature>` i zalecane ustawienia sesji: nowa sesja, Sonnet,
     effort `medium`, tryb auto;
   - gdy agent ma narzędzie do zakładania sesji, zaproponować start jako kartę, nigdy nie uruchamiać samemu.
6. **Seamy z ADR 0004.** `to-tickets` bierze seamy ze strategii testów projektu (ADR-y, `AGENTS.md`), więc
   generyczny fork nie potrzebuje wzmianki o ADR 0004. Rekomendacja: bez zmian w forku. Sprawdzenie przy
   przebiegu (pytanie 8).
7. **Dokumentacja forka.** Rekomendacja:
   - nowa strona `docs/engineering/plan-feature.md`;
   - wpisy w `README.md` i `skills/engineering/README.md`;
   - łańcuch w `docs/engineering/ship.md` („Where it fits”) z `/plan-feature` zamiast trzech kroków;
   - jedno zdanie w `ask-matt`;
   - bump do `1.3.1-dotnet.18`.
8. **Żywy przebieg.** Seam to pilot 10, ale bramka modelu (pytanie 1) i składanie skilli (pytanie 2) wychodzą
   tylko na żywo. Rekomendacja: krótki przebieg w sandboksie na `main`, w nowej sesji na Opusie. Mały ficzer na
   jedną lub dwie rundy grilla, a odpowiada użytkownik. Do sprawdzenia:
   - model w drugiej turze;
   - format ticketów względem `issue-tracker.md`;
   - jeden commit `plan: <feature>`;
   - propozycja `/ship`.

   Samego `/ship` nie uruchamiać, to robi pilot. Uruchomienie tylko na wyraźne słowa użytkownika.

## 5. Dług i ustalenia przeniesione

Nie spłacaj ich tutaj bez decyzji użytkownika.

- **Do pilota (10), niesprawdzone na żywo:**
  - z ticketu 07:
    - scalenie przez `merge --ff-only`, gdy checkout stoi na `main`;
    - krok CI w przeglądarce sesji w tle;
    - wznawianie przerwanej bramki 2;
    - odpowiedź „nie” albo niejasna;
    - czy allowlista przepuszcza sam `git push origin main`, a push innej gałęzi albo z `--force` pyta;
  - z wcześniejszych: rundy 2 i 3 gałęzi, uwagi otwarte w briefie, przywracanie kodu przez `git restore`, pauza
    limitu przed review gałęzi, verifier z narzędziem PowerShell (plugin `.15`).
- **Ticket 12** (fallback powiadomień przez ntfy): `needs-triage`. Pushe na telefon nadal nie dochodzą, ale
  odpowiedź wpisana na telefonie przez Remote Control dociera do czekającej sesji (07).
- **Do ticketu 09 (przewodnik):** wszystko z `## Comments` ticketu 09 oraz:
  - `/ship` na Sonnecie z effortem `medium`;
  - `/plan-feature` na Opusie (zależnie od pytania 1);
  - pusty folder worktree na Windows po bramce 2.
- **Znane ograniczenie:** wznowiony ticket `in-progress` albo runda gałęzi w toku dostaje baseline na swoim
  bieżącym drzewie.
- **Sprzątanie na życzenie użytkownika:**
  - worktree `toy-quota` i `toy-overdue` oraz pusty folder `toy-brief` w sandboksie;
  - zarchiwizowanie sesji przebiegów A, B, B2, C i D oraz sesji testu pushy.

## 6. Środowisko i pułapki, które kosztowały czas

Część z tej sekcji pochodzi ze streszczenia po kompaktowaniu kontekstu sesji ticketu 07 i może być niepełna.

- **Sesja izolowana w worktree:**
  - odmawia złożonych poleceń z `git` (`cd`, `;`, `&&`, potok, `$(...)`);
  - jej narzędzie Bash odmawia każdego polecenia z `pwsh`, więc skrypty uruchamia się narzędziem PowerShell.
- **Checkout główny nie jest izolowany.** Sesja chętnie łączy tam polecenia w łańcuchy, a wynik zależy wtedy od
  kolejności, nie od kodu wyjścia. To wyszło w przebiegu D ticketu 07. Skill musi wprost żądać jednego polecenia na
  wywołanie tam, gdzie decyzja zależy od wyniku.
- **Windows trzyma folder worktree** otwarty przez powłokę sesji: `git worktree remove` daje `Permission denied`,
  choć git już go wyrejestrował.
- **Tryb auto blokuje edycję `.claude/settings.json`.** Linie allowlisty dopisuje użytkownik. Po jego zapewnieniu
  sprawdź plik na dysku: w 07 linii najpierw nie było. Podaj dokładne miejsce wstawienia.
- **Długie wpisy po polsku:** dopisuj przez Edit albo Write, nie przez heredoc w Bash.
- **Plugin w trakcie przebiegu:** sesja w tle używa kopii z cache
  (`~/.claude/plugins/cache/mattpocock/mattpocock-skills/<wersja>`). Źródła forka można poprawiać w trakcie, bump
  robi się po przebiegu.
- **Dowody z przebiegu:** transkrypty subagentów w
  `C:\Users\hondo\.claude\projects\<ścieżka-z-myślnikami>\<sesja>\subagents\agent-*.jsonl`.
- **Start żywego przebiegu:** `start_session` z `initiation: user_asked`, `cwd` sandboksa, `use_worktree: false`,
  `permission_mode: auto` i promptem ze skillem. Dla `/plan-feature` model `claude-opus-5-5`, bo to sesja
  z użytkownikiem.
- **Koszt:** przebieg D (bramka 2 bez review) był tani. Grill na Opusie będzie droższy, więc sprawdź limit przed
  przebiegiem.
- **Pułapki z `docs/handoff-2026-10-26-agent-loop-05.md`, sekcja 6, nadal obowiązują:** `pwsh` jako narzędzie
  dotnet, `set -o pipefail` przed `--no-build`, Docker przez `.claude/hooks/run-dod.ps1 -Check environment`,
  kroki aktualizacji forka.

## 7. Workflow

Bez zmian względem ticketów 05–07:

1. Bramka w tej sesji: pytania w jednej rundzie, decyzje w `## Comments` ticketu jako „Decyzje przed
   implementacją”, commit.
2. Użytkownik wpisuje `/mattpocock-skills:implement .scratch/agent-loop/issues/08-plan-feature.md`.
3. `tdd` na seamach ticketu: tu pilot, więc testy w `tests/HookTests` tylko wtedy, gdy powstanie skrypt.
4. Pełny build i testy łańcuchem: `set -o pipefail`, potem
   `dotnet build CleanArchitecture.slnx && dotnet test CleanArchitecture.slnx --no-build`.
5. Review na osi Spec: subagent na Sonnecie, dwie rundy. Każ mu przejść ścieżkę samodzielnych skilli
   (`to-spec`, `to-tickets` bez `/plan-feature`) i każdy wariant odpowiedzi na bramce pakietu.
6. Commity w obu repozytoriach, bump pluginu i aktualizacja.
7. Żywy przebieg w sandboksie, tylko na wyraźne słowa użytkownika.
8. Zamknięcie ticketu.

Push i scalenie `chore/agent-loop` do `main` należą do użytkownika, dopiero po pilocie.

## 8. Lekcje metodyczne do powtórzenia

1. **Fakty z handoffu sprawdzaj przed rundą.** W 07 rekomendacje 4 i 5 poprzedniego handoffu nie działały:
   `git fetch . x:main` odmawia, gdy `main` jest wyciągnięty, a `git branch -d` odmawia przy checkoucie na innej
   gałęzi. Wyszło to tylko dzięki próbie na repozytorium tymczasowym. Pytania 1 i 2 mają ten sam charakter.
2. **Review Spec na Sonnecie znajduje dziury w każdej rundzie:** w 07 kolejno 7 i 4 uwagi, głównie przy wznawianiu
   i stanach pośrednich. Bez drugiej rundy zostałyby w skillu.
3. **Żywy przebieg znajduje to, czego review nie widzi.** W 07 były to łańcuchy poleceń i folder trzymany przez
   proces. Przy `/plan-feature` najpewniej wyjdzie model w drugiej turze.
4. **Po przebiegu przeszukaj transkrypty, nie tylko raport sesji.** Orkiestrator potrafi obejść błąd i nie
   wspomnieć o nim.
5. **Wynik narzędzia to nie fakt.** „Mobile push requested” nie znaczy, że push doszedł, a zapewnienie
   o dopisanych liniach nie znaczy, że są w pliku. Potwierdza dopiero drugi kanał.

## 9. Pierwszy krok i miejsce zatrzymania

1. Przeczytaj pliki z sekcji 3. Sprawdź fakty z pytań 1 i 2: trwałość przełączonego modelu sesji między turami
   oraz to, czy `disable-model-invocation` blokuje Skill tool. Do dokumentacji Claude Code użyj agenta
   `claude-code-guide`, a co się da, sprawdź próbą.
2. Przedstaw użytkownikowi pytania z sekcji 4 w jednej rundzie: numerowane, z rekomendacją, sformułowane tak,
   żeby „tak” przyjmowało rekomendację.
3. Zapisz decyzje w `## Comments` ticketu 08, zacommituj i poproś użytkownika o
   `/mattpocock-skills:implement .scratch/agent-loop/issues/08-plan-feature.md`.
4. Zatrzymaj się po zamknięciu ticketu 08: commity w obu repozytoriach, zaktualizowany plugin, przebieg
   w sandboksie, jeśli użytkownik go zlecił. Nie zaczynaj ticketu 09 w tej samej sesji, jeśli kontekst przekroczył
   połowę okna albo okno 5-godzinne przekroczyło 70%.
