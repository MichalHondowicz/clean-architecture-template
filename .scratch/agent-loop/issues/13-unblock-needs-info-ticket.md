# 13: Odblokowanie ticketu po `needs-info`

**What to build:** Ticket, który `/ship` odstawił na `needs-info`, da się po decyzji użytkownika puścić
ponownie, a `/ship` nie uzna go od razu znowu za `stuck`.

**Blocked by:** None

**Seams:** `ship/SKILL.md` w forku („Rounds”, wybór ticketu); przewodnik `docs/workflow.md`; obserwacja przy
następnym tickecie odblokowanym po `needs-info`.

**Status:** ready-for-agent

**Skąd:** review Spec ticketu 09, runda 2 (2026-10-28). `ship/SKILL.md`, „Rounds”: pliki
`.scratch/<feature>/findings/<NN>-round-<r>.md` liczą zużyte rundy, a ticket, który ma już trzy, jest `stuck`.
Ścieżka `stuck` tych plików nie usuwa. Ticket, który doszedł do `needs-info` po trzech rundach, ma trzy
takie pliki, więc po ręcznym przywróceniu `**Status:** ready-for-agent` kolejny `/ship <feature>` od razu uzna
go znowu za `stuck`. Ticket zatrzymany wcześniej (decyzja użytkownika, chroniona ścieżka, brak commitów
w rundzie 1) ruszyłby od następnej rundy. Przewodnik (`docs/workflow.md`, „Wznawianie i stany
pośrednie”) opisuje to jako lukę.

- [x] Droga od `needs-info` do ponownego `ready-for-agent`: użytkownik dopisuje w `## Comments` ticketu
      `**Decision:** …` i ustawia `**Status:** ready-for-agent` (commit na gałęzi w worktree). `/ship` przy
      wyborze takiego ticketu przenosi jego pliki `findings/<NN>-round-<r>.md` do `findings/<NN>-attempt-<k>/`
      i zaczyna od rundy 1, z pełnymi trzema rundami.
- [x] Odpowiedź na `needs-info` w żywej sesji `/ship` też rusza od rundy 1 (tak było w pilocie 10).
- [x] Przewodnik `docs/workflow.md` opisuje tę drogę zamiast luki.
- [x] Wersja forka podbita.

## Comments

**2026-10-30, z pilota 10 (przebieg 2):** pierwsza obserwacja na żywo. Ticket 02 stanął na `needs-info`
w rundzie 1 (chroniona ścieżka, bez plików rund). Użytkownik odpowiedział w tej samej sesji `/ship`
„Odblokuj ścieżkę”, bez zmiany statusu i bez nowego `/ship`. Pętla zdjęła ochronę commitem i wysłała
implementera „Implement ticket 02 retry” znowu jako rundę 1. Ticket wrócił do `needs-info` z innego powodu
(ticket 18). Działa więc droga „odpowiedź w sesji, która jeszcze żyje”. Droga przez zmianę statusu
i ponowny `/ship` po trzech rundach pozostaje niesprawdzona.

**2026-11-01, triage (decyzja użytkownika):** `ready-for-agent`, piąty w kolejce. Droga jak w pierwszym
kryterium: decyzja w `## Comments`, zmiana statusu, stare pliki rund do `findings/<NN>-attempt-<k>/`, start
od rundy 1.

**2026-11-01, wynik:** fork `486a300`, `c6ee3cf` (plugin `1.3.1-dotnet.25`, zainstalowany), template `0540d60`.

- **„Work the tickets”**, nowy akapit „A ticket back from `needs-info`”: ticket `ready-for-agent`, który ma pliki
  `findings/<NN>-round-<r>.md`, wraca z `needs-info`. To nowa próba z trzema rundami: `mkdir -p` folderu
  `findings/<NN>-attempt-<k>/`, `git mv` każdego pliku do niego, w tym samym commicie co start
  (`ship: start <NN> (…, attempt <k+1>)`). Implementer rundy 1 dostaje najnowszą `**Decision:**`. „Rounds” nie
  liczy plików z folderów prób, a recenzent Spec dostaje je jako wcześniejsze findings.
- **Akapit „A decision given in the session”:** odpowiedź użytkownika w sesji po końcu przebiegu `/ship`
  wpisuje dosłownie jako `**Decision:**`, ustawia `ready-for-agent`, commituje (`ship: <NN> decision`) i wraca
  do *Start or resume*. Obie drogi są więc tą samą ścieżką. Prośba o zdjęcie ochrony dostaje odpowiedź
  z „Stuck” (ticket 18).
- **„Stuck”, Needs:** mówi, jak dać decyzję, z wyjątkiem ticketu na chronionej ścieżce, który ma swoje dwie
  drogi.
- **Przewodnik:** „Po `needs-info`” opisuje obie drogi zamiast luki.
- **Review Spec** (Sonnet), runda 1: `fail` z 2 findings. (1) `git mv` do nieistniejącego folderu kończy się
  `fatal: destination directory does not exist`, recenzent to odtworzył. (2) Needs otwierał trzecią drogę dla
  ticketu na chronionej ścieżce. Oba poprawione w `c6ee3cf`, re-review `pass`.
- **Niesprawdzone na żywo:** droga przez zmianę statusu po trzech rundach. Do obserwacji przy pierwszym takim
  tickecie.
