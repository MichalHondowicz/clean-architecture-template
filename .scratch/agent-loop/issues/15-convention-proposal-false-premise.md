# 15: Propozycja konwencji na fałszywym fakcie o kodzie

**What to build:** Propozycja konwencji w briefie opiera twierdzenia o istniejącym kodzie na sprawdzeniu
i mówi, które pliki, także na gałęzi ficzera, łamią proponowaną regułę. Użytkownik na bramce 2 nie przyjmuje
reguły, którą scalany kod sam łamie, nie wiedząc o tym.

**Blocked by:** None

**Seams:** do ustalenia przy triage (zmiana w forku: `agents/standards-reviewer.md`, może też „Brief”
w `ship/SKILL.md`; obserwacja w kolejnym przebiegu z propozycjami).

**Status:** needs-triage

**Skąd:** pilot 10, przebieg 1 (`todo-comments`, 2026-10-29). Propozycja 4 („Validators are `internal
sealed`, like handlers.”) uzasadniała regułę tym, że „wszystkie dotychczasowe [walidatory są] `internal
sealed`”. W sandboksie `CreateTodoCommandValidator` i `CopyTodoCommandValidator` są `public class`.
Propozycja wymieniała nowe walidatory `AddTodoCommentCommandValidator` i `DeleteTodoCommentCommandValidator`
jako `public class`, ale oba weszły do `main` w tej postaci. Użytkownik przyjął regułę („OK, 1-4 tak”), więc
`main` sandboksu (`b38e40e`) łamie regułę przyjętą na tej samej bramce: cztery walidatory są `public class`.

- [ ] Decyzja użytkownika dla sandboksu: korekta konwencji 4 (ścieżka z `docs/workflow.md`, „Korekta decyzji
      z bramki 2 po scaleniu”) albo zmiana czterech walidatorów na `internal sealed`.
- [ ] `standards-reviewer` popiera każde twierdzenie o istniejącym kodzie w propozycji wynikiem sprawdzenia
      (liczba albo lista plików) i wymienia pliki, które łamią proponowaną regułę, także na gałęzi ficzera.
- [ ] Brief pokazuje przy propozycji, że scalany kod ją łamie, zanim użytkownik na nią odpowie.
