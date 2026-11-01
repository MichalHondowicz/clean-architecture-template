# 15: Propozycja konwencji na fałszywym fakcie o kodzie

**What to build:** Propozycja konwencji w briefie opiera twierdzenia o istniejącym kodzie na sprawdzeniu
i mówi, które pliki, także na gałęzi ficzera, łamią proponowaną regułę. Użytkownik na bramce 2 nie przyjmuje
reguły, którą scalany kod sam łamie, nie wiedząc o tym.

**Blocked by:** None

**Seams:** sandbox `D:\Projects\ship-sandbox` (cztery walidatory, build i testy); `agents/standards-reviewer.md`
i „Brief” w `ship/SKILL.md` w forku; obserwacja w kolejnym przebiegu z propozycjami.

**Status:** ready-for-agent

**Skąd:** pilot 10, przebieg 1 (`todo-comments`, 2026-10-29). Propozycja 4 („Validators are `internal
sealed`, like handlers.”) uzasadniała regułę tym, że „wszystkie dotychczasowe [walidatory są] `internal
sealed`”. W sandboksie `CreateTodoCommandValidator` i `CopyTodoCommandValidator` są `public class`.
Propozycja wymieniała nowe walidatory `AddTodoCommentCommandValidator` i `DeleteTodoCommentCommandValidator`
jako `public class`, ale oba weszły do `main` w tej postaci. Użytkownik przyjął regułę („OK, 1-4 tak”), więc
`main` sandboksu (`b38e40e`) łamie regułę przyjętą na tej samej bramce: cztery walidatory są `public class`.

- [ ] Sandbox: konwencja 4 zostaje, a `CreateTodoCommandValidator`, `CopyTodoCommandValidator`,
      `AddTodoCommentCommandValidator` i `DeleteTodoCommentCommandValidator` są `internal sealed`. Build
      i pełny zestaw testów zielone, commit na `main` sandboksa.
- [ ] `standards-reviewer` popiera każde twierdzenie o istniejącym kodzie w propozycji wynikiem sprawdzenia
      (liczba albo lista plików) i wymienia pliki, które łamią proponowaną regułę, także na gałęzi ficzera.
- [ ] Brief pokazuje przy propozycji, że scalany kod ją łamie, zanim użytkownik na nią odpowie.
- [ ] Wersja forka podbita.

## Comments

**2026-11-01, triage (decyzja użytkownika):** `ready-for-agent`, szósty w kolejce. W template'cie 5 z 7
walidatorów jest `internal sealed`, a `public` są tylko `Create` i `Copy`. Konwencja 4 zgadza się więc
z większością kodu, a zmienia się kod sandboksu, nie reguła. Template'u ten ticket nie zmienia.
