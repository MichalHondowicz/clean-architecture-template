# Todo

Osobista lista zadań: użytkownik zakłada konto, loguje się i zarządza wyłącznie własnymi todo itemami.
Terminy są po angielsku, tak jak w kodzie; definicje po polsku.

## Language

### Użytkownicy i dostęp

**User**:
Osoba z kontem założonym przez rejestrację (email i hasło); jedyny rodzaj aktora w systemie.
_Avoid_: Account, Member, Customer

**Owner**:
User, do którego należy todo item. Widzi go i zmienia wyłącznie owner.
_Avoid_: Creator, Author

**Access token**:
Krótkotrwały dowód tożsamości usera, wydawany przy logowaniu i odświeżeniu, dołączany do każdego żądania.
_Avoid_: JWT (w prozie), Session token

**Refresh token**:
Długotrwały token wydawany przy logowaniu, wymieniany na nowy access token. Każda wymiana go rotuje:
stara wartość przestaje działać, a termin ważności przesuwa się od chwili wymiany.
_Avoid_: Session, Remember-me token

**Rotation**:
Zastąpienie wartości refresh tokenu nową przy każdej wymianie, tak że skradziona wartość działa najwyżej raz.

**Permission**:
Nazwane prawo (np. `users:access`) wymagane przez endpoint, niezależne od tego, czy user jest ownerem.
_Avoid_: Role, Claim, Scope

### Todo itemy

**Todo item**:
Jedna rzecz do zrobienia należąca do jednego ownera. W nazwach use case'ów i tras skracana do „Todo”
(`ArchiveTodoCommand`, `todos/...`); identyfikator to zawsze `TodoItemId`.
_Avoid_: Task, Todo (jako rzeczownik w prozie), Item

**Priority**:
Ważność todo itemu. `Normal` oznacza brak nadanego priorytetu; nadane tworzą skalę Low < Medium < High < Top.
_Avoid_: Severity, Urgency

**Label**:
Dowolny tekst przypięty do todo itemu; todo item może mieć ich wiele.
_Avoid_: Tag, Category

**Due date**:
Opcjonalny termin, do którego todo item powinien być ukończony.
_Avoid_: Deadline

**Completion**:
Jednorazowe oznaczenie todo itemu jako zrobionego, z zapisanym momentem ukończenia. Nie ma cofnięcia;
ponowne ukończenie to błąd.
_Avoid_: Done, Close, Finish

**Archive**:
Odłożenie todo itemu przez ownera poza bieżącą listę, z zapisanym momentem archiwizacji, bez usuwania go
i bez zmiany jego completion. Ponowna archiwizacja zarchiwizowanego to błąd.
_Avoid_: Hide, Soft delete, Trash; Active dla niezarchiwizowanego (zwykle znaczy „nieukończony”)

**Unarchive**:
Cofnięcie archive: todo item wraca na bieżącą listę, a moment archiwizacji przepada. Unarchive
niezarchiwizowanego to błąd.
_Avoid_: Restore, Reopen

**Copy**:
Nowy todo item tego samego ownera z tym samym opisem, priorytetem, due date i labelami, ale nieukończony
i niezarchiwizowany.
_Avoid_: Duplicate, Clone
