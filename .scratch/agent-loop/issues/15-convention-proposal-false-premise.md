# 15: Propozycja konwencji na fałszywym fakcie o kodzie

**What to build:** Propozycja konwencji w briefie opiera twierdzenia o istniejącym kodzie na sprawdzeniu
i mówi, które pliki, także na gałęzi ficzera, łamią proponowaną regułę. Użytkownik na bramce 2 nie przyjmuje
reguły, którą scalany kod sam łamie, nie wiedząc o tym.

**Blocked by:** None

**Seams:** sandbox `D:\Projects\ship-sandbox` (cztery walidatory, build i testy); `agents/standards-reviewer.md`
i „Brief” w `ship/SKILL.md` w forku; obserwacja w kolejnym przebiegu z propozycjami.

**Status:** done

**Skąd:** pilot 10, przebieg 1 (`todo-comments`, 2026-10-29). Propozycja 4 („Validators are `internal
sealed`, like handlers.”) uzasadniała regułę tym, że „wszystkie dotychczasowe [walidatory są] `internal
sealed`”. W sandboksie `CreateTodoCommandValidator` i `CopyTodoCommandValidator` są `public class`.
Propozycja wymieniała nowe walidatory `AddTodoCommentCommandValidator` i `DeleteTodoCommentCommandValidator`
jako `public class`, ale oba weszły do `main` w tej postaci. Użytkownik przyjął regułę („OK, 1-4 tak”), więc
`main` sandboksu (`b38e40e`) łamie regułę przyjętą na tej samej bramce: cztery walidatory są `public class`.

- [x] Sandbox: konwencja 4 zostaje, a `CreateTodoCommandValidator`, `CopyTodoCommandValidator`,
      `AddTodoCommentCommandValidator` i `DeleteTodoCommentCommandValidator` są `internal sealed`. Build
      i pełny zestaw testów zielone, commit na `main` sandboksa.
- [x] `standards-reviewer` popiera każde twierdzenie o istniejącym kodzie w propozycji wynikiem sprawdzenia
      (liczba albo lista plików) i wymienia pliki, które łamią proponowaną regułę, także na gałęzi ficzera.
- [x] Brief pokazuje przy propozycji, że scalany kod ją łamie, zanim użytkownik na nią odpowie.
- [x] Wersja forka podbita.

## Comments

**2026-11-01, triage (decyzja użytkownika):** `ready-for-agent`, szósty w kolejce. W template'cie 5 z 7
walidatorów jest `internal sealed`, a `public` są tylko `Create` i `Copy`. Konwencja 4 zgadza się więc
z większością kodu, a zmienia się kod sandboksu, nie reguła. Template'u ten ticket nie zmienia.

**2026-11-01, wynik:**

- **Sandbox** (`main`, bez pusha do jego origin): `4713c65` zmienia cztery walidatory na `internal sealed`.
  Build 0 błędów, testy 6/91/34/83. `git grep` pokazuje 9 walidatorów, wszystkie `internal sealed`.
  `96ca937` poprawia przykład w `.claude/skills/add-feature/references/command-slice.md`: pokazywał
  `public class CreateTodoCommandValidator`, więc agent, który za nim szedł, odtwarzałby łamanie konwencji 4.
  To prawdopodobna przyczyna dwóch nowych walidatorów `public class` w przebiegu 1.
- **Fork** (`90c8727`, plugin `1.3.1-dotnet.26`, zainstalowany): `standards-reviewer` sprawdza każdą
  proponowaną regułę `git grep` na `HEAD` gałęzi, razem z plikami ficzera. Każde twierdzenie o istniejącym
  kodzie podaje jako liczbę albo listę plików, a propozycja wymienia pliki, które już ją łamią (albo `none`).
  Brief niesie te pliki przy propozycji, a propozycja łamana przez scalany kod mówi to w pierwszej linii,
  przed pytaniem bramki 2.
- **Review Spec** (Sonnet): `pass` bez findings.
- **Poza ticketem:** template ma ten sam przykład `public class` w `add-feature`, a 2 z 7 walidatorów
  (`Create`, `Copy`) są `public`. Template nie ma reguły o walidatorach, więc to decyzja użytkownika, nie
  część tego ticketu.
