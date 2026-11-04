# 21: Koszyk: warstwa procesu Pococka

**What to build:** Koszyk dostaje to, z czego czytają skille procesu: tracker, etykiety, układ dokumentów
domeny, sekcję procesu w `AGENTS.md` i `CLAUDE.md` bez starego workflow modułów. Konwencje Koszyka zostają
jego własne. Według `docs/workflow.md` template'u, „Istniejący projekt”, kroki 1–4.

**Blocked by:** None

**Seams:** pliki w `D:\Projects\Koszyk` (`docs/agents/`, `AGENTS.md`, `CLAUDE.md`, `GLOSSARY.md`); obserwacja
w pilocie (ticket 26).

**Status:** done

**Skąd:** ticket 11, decyzja 3 z bramki 2026-11-04 (historie 55, 63).

- [x] AC1: `/setup-matt-pocock-skills` w Koszyku: tracker `.scratch/<feature>/` (`docs/agents/issue-tracker.md`),
      pięć etykiet (`docs/agents/triage-labels.md`), jeden kontekst domeny (`docs/agents/domain.md`).
- [x] AC2: `AGENTS.md` Koszyka ma sekcję „Process skills in this project” z komendami Koszyka: build obu solucji,
      test jednego pliku i pełne testy (`dotnet test Koszyk.sln`, `dotnet test tests/Koszyk.Mobile.Tests`),
      review ticketu tylko na osi Spec, `/plan-feature` i `/ship`. Konwencje backendu i mobilne zostają bez zmian.
- [x] AC3: `CLAUDE.md` zamiast sekcji „Workflow modułów: plugin dotnet-workflow” wskazuje `/plan-feature`
      i `/ship`. `docs/tasks/` zostaje jako historia modułów 0–14, a `AGENTS.md`, „Praca w modułach”, mówi,
      że nowe ficzery idą przez tracker.
- [x] AC4: `GLOSSARY.md` z pojęciami z `Koszyk.Core` (przez `domain-modeling`) i `docs/adr/` z decyzjami,
      które już zapadły w specach `docs/superpowers/specs/`, zapisanymi jako ADR-y.
- [x] AC5: Build obu solucji 0 ostrzeżeń i testy zielone po zmianach (zmieniają się tylko dokumenty).

## Comments

**2026-11-04, wynik:** Koszyk, gałąź `chore/agent-loop` (bez remote, ticket 20), commity `173735c` i `0613390`.

- `docs/agents/`: `triage-labels.md` i `domain.md` skopiowane z template'u, `issue-tracker.md` dopasowany
  (seamy Koszyka: `WebApplicationFactory`, testy jednostkowe, `Koszyk.Mobile.Tests` ze strażnikami głowy MAUI).
  `/setup-matt-pocock-skills` nie dało się wywołać z sesji (skill tylko dla użytkownika), więc pliki powstały
  ręcznie w tym samym układzie. Review Spec uznał to za spełnione AC1.
- `AGENTS.md`: sekcja „Agent skills” z „Process skills in this project” (komendy Koszyka, Standards według
  `AGENTS.md`, bo Koszyk nie ma skilla standardów). Intro i „Praca w modułach” mówią, że `docs/tasks/` to
  historia, a nowe ficzery idą przez `.scratch/`. Sekcja zaznacza, że `dod.json` i kontrakt implementera
  jeszcze nie istnieją (ticket 23).
- `CLAUDE.md`: „Workflow ficzerów: `/plan-feature` i `/ship`” zamiast workflow `dotnet-workflow`.
- `GLOSSARY.md`: 19 pojęć (listy i pozycje, współdzielenie, katalog i sugestie, synchronizacja), sprawdzonych
  w `Koszyk.Core`, `ListAccessService`, `UserProvisioningMiddleware` i specach. `docs/adr/0001–0005`: wyjątki
  domenowe, delta sync, offline-first z Outboxem, SignalR jako sygnał, 404 dla nie-członka i 403 dla za
  niskiej roli. „Considered options” tylko z tego, co zapisały spec i zadania modułów.
- Build obu solucji 0/0, testy 99/130/587 bez zmian.
- Review Spec (Sonnet), runda 1 `fail`: odwołania do plików, których jeszcze nie ma; „rozważane opcje” bez
  źródła w ADR-ach; zły cytat w ADR 0005; różnica LWW „pole” i „pozycja” między specami. Poprawione
  w `0613390`, re-review `pass`.
