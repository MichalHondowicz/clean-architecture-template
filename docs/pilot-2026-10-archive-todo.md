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

Lista kontrolna z handoffu (odhaczyć po grillu; nie rozstrzygać zawczasu):

- [ ] Kto może archiwizować (owner, filtr `UserId`, cudzy todo item to 404)
- [ ] Czy da się przywrócić (unarchive)
- [ ] Czy zarchiwizowane znikają z `GET todos`; czy `GET todos/{id}` nadal je zwraca
- [ ] Relacja z Completion (archiwizacja nieukończonego, ukończenie zarchiwizowanego)
- [ ] Cache: unieważnienie `GET todos/{id}` w `HybridCache`
- [ ] Zdarzenie domenowe `TodoItemArchived`, id nadane przed `Raise(...)` (ADR 0006)

Pytania spoza listy: _do uzupełnienia_

Zmiany w `GLOSSARY.md` i ADR-y: _do uzupełnienia_

## Krok 3: `/to-spec`, `/to-tickets`

_do uzupełnienia_ (podział na tickety przed utworzeniem plików, bramka u użytkownika)

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
