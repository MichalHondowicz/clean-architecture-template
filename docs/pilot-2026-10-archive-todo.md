# Dziennik pilota: archiwizacja todo itemu (Faza 5)

Start: 2026-10-12. Gałąź `pilot/archive-todo` od `main` (`111f2d4`). Plan: `docs/plan-2026-10-12-agent-setup.md`,
Faza 5. Handoff: `docs/handoff-2026-10-12-phase5-pilot.md`. Spec i tickety: `.scratch/archive-todo/`.

Agent prowadzący pilot jest jednocześnie autorem tego dziennika. Każda korekta, którą musiał zrobić
użytkownik, trafia do sekcji „Korekty użytkownika”, także jeśli wydaje się drobna.

## Kryteria sukcesu (plan, sekcja „Cel i efekt końcowy”)

| Kryterium | Wynik |
|---|---|
| Build: 0 błędów, 0 ostrzeżeń, bez suppressions | _do uzupełnienia_ |
| Testy zielone: unit, architecture, integration (z Dockerem) | _do uzupełnienia_ |
| `ca-review` bez uwag | _do uzupełnienia_ |
| `GLOSSARY.md` zaktualizowany (co najmniej „Archive”) | _do uzupełnienia_ |
| Użytkownik nie poprawiał agenta w kwestii konwencji | _do uzupełnienia_ |

## Preflight (2026-10-12, własny przebieg)

- `git status`: czyste drzewo na `main`, HEAD `111f2d4`.
- `docker info`: serwer 29.8.1, Docker Desktop.
- Fork Pococka: `installed_plugins.json` ma `mattpocock-skills@mattpocock` w wersji `1.3.1-dotnet.1`
  (`gitCommitSha` `2f5489b`); `skills/engineering/tdd/tests.md` w cache tej wersji zawiera przykłady `csharp`.
- superpowers: `"superpowers@claude-plugins-official": false` w `.claude/settings.json`; na starcie sesji nie
  pojawił się wstrzyknięty „using-superpowers”, a lista skilli sesji nie zawiera skilli superpowers.
- `dotnet build CleanArchitecture.slnx`: 0 błędów, 0 ostrzeżeń.
- `dotnet test CleanArchitecture.slnx`: UnitTests 22/22, ArchitectureTests 6/6, IntegrationTests 8/8.

## Decyzje przed pilotem

- **P1 (a):** gałąź pilota zostaje jako dowód; do `main` trafia tylko dziennik i poprawki warstwy agentowej
  (`AGENTS.md`, skille, ADR-y). Template zostaje generyczny.
- **P2 (a):** najpierw pilot; ticket 403 (`.scratch/ownership-forbidden/`) później jako osobny przebieg
  samego `/implement`.

## Zastrzeżenie metodyczne

Przed grillem agent przeczytał handoff, w tym listę kontrolną pytań dla grilla z rekomendacjami (sekcja 4
handoffu), a także `GLOSSARY.md`, ADR-y 0004–0006 i `add-feature`. Pomiar „czy `/grill-with-docs` sam
wydobędzie pytania” jest więc zanieczyszczony: agent zna listę. Odhaczenie pytania mówi, że padło w grillu,
ale nie rozstrzyga, czy padłoby bez handoffu.

## Krok 2: `/grill-with-docs archiwizacja todo itemu`

Przebieg: skill wywołał `grilling` i `domain-modeling`. 11 pytań w 3 rundach, wszystkie rekomendacje przyjęte
(runda 1 odpowiedzią „kontynuuj”, zinterpretowaną jako akceptacja; rundy 2 i 3 jawnym „akceptuję wszystkie
rekomendacje”). Fakty z kodu agent zebrał sam (`find`, `cat` na `Domain/Todos`, `Application/Todos`,
`Endpoints/Todos`), a nie przez sub-agenta, jak każe `grilling`: odstępstwo od skilla.

Lista kontrolna z handoffu:

- [x] Kto może archiwizować: Q2 (runda 1), tylko owner, cudzy todo item to 404.
- [x] Czy da się przywrócić: Q3 (runda 1) tak, osobny use case; nazwa Unarchive w Q4.
- [x] Widoczność: Q5 (runda 2), `GET todos` pomija zarchiwizowane, `GET todos/{id}` je zwraca.
- [x] Relacja z Completion: Q1 (archiwizować można nieukończony) i Q6 (zarchiwizowany można ukończyć,
  edytować, skopiować, usunąć; niezależne osie).
- [x] Cache: Q11 (runda 3), ale jako założenie z konwencji do potwierdzenia, nie jako otwarta decyzja.
- [x] Zdarzenie domenowe: Q11, `TodoItemArchivedDomainEvent` i `TodoItemUnarchivedDomainEvent`; również jako
  założenie z konwencji.

Pytania spoza listy:

- Q4: nazwa cofnięcia (Unarchive, nie Restore/Reopen) i trasy `PUT todos/{id}/archive`, `.../unarchive` (204).
- Q6 (część): Copy zarchiwizowanego daje kopię niezarchiwizowaną.
- Q7: ponowna archiwizacja i Unarchive niezarchiwizowanego to błąd 400 (`TodoItems.AlreadyArchived`,
  `TodoItems.NotArchived`), spójnie z Completion.
- Q8: moment archiwizacji jako jedno pole `ArchivedAt` (null = niezarchiwizowany), kasowane przez Unarchive.
- Q9: jak owner znajduje zarchiwizowane, skoro Unarchive wymaga id. Wyszło ze scenariusza; dodało use case
  `GET todos/archived` (owner z `IUserContext`, bez `userId`, sortowanie od ostatnio zarchiwizowanego, bez
  cache).
- Q10: kolizja nazwy „Active” (w aplikacjach todo zwykle „nieukończony”); bez nowego terminu.

Zakres urósł w grillu: z jednego use case'u do trzech nowych (Archive, Unarchive, GetArchived) i zmiany dwóch
istniejących zapytań (`GET todos` filtruje, `GET todos/{id}` zwraca `ArchivedAt`).

Zmiany w `GLOSSARY.md`: nowe hasła **Archive** i **Unarchive**, w **Copy** dopisane „niezarchiwizowany”.

ADR-y: żaden. Kandydaci odrzuceni wg trzech warunków `domain-modeling`: niezależne osie archive/completion
i `GET todos/archived` bez `userId` są łatwe do odwrócenia (template bez zewnętrznych klientów), więc nie
spełniają warunku „trudne do odwrócenia”.

Bramka: użytkownik potwierdził wspólne rozumienie i zmiany w słowniku.

## Krok 3: `/to-spec`, `/to-tickets`

Tarcie: pierwsza próba to wiadomość „potwierdzam, /to-spec”. Komenda w środku zdania nie uruchamia skilla,
a agent nie może jej wywołać sam (`disable-model-invocation: true`). Agent poprosił o osobną wiadomość.

`/to-spec`: agent przedstawił seamy (HTTP główny, handler dla porażek i czasu, walidator; bez nowych),
użytkownik je przyjął. Spec: `.scratch/archive-todo/spec.md`, `Status: ready-for-agent`, 22 historyjki.
Spec sam z siebie wymienia migrację EF Core dla `ArchivedAt` (zastrzeżenie: agent znał tę pułapkę
z handoffu, sekcja 6) i wskazuje test integracyjny, który pilnuje unieważnienia cache.

`/to-tickets`: _do uzupełnienia_ (podział na tickety przed utworzeniem plików, bramka u użytkownika)

## Krok 4: `/implement`

| Ticket | `add-entity` / `add-feature` | Pierwszy test czerwony (komunikat) | Migracja (sam?) | Build bez suppressions | Red proof | Commit |
|---|---|---|---|---|---|---|

## Krok 5: `code-review` przeciw `main`, potem `ca-review`

_do uzupełnienia_

## Krok 6: wynik i `/retro`

_do uzupełnienia_

## Korekty użytkownika

Format: krok, co zrobił agent, co poprawił użytkownik, czy korekta dotyczy konwencji projektu.

_brak_

## Obserwacje procesu

_do uzupełnienia_
