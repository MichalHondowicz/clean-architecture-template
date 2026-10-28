# 13: Odblokowanie ticketu po `needs-info`

**What to build:** Ticket, który `/ship` odstawił na `needs-info`, da się po decyzji użytkownika puścić
ponownie, a `/ship` nie uzna go od razu znowu za `stuck`.

**Blocked by:** None

**Seams:** do ustalenia przy triage (zmiana w forku: `ship/SKILL.md`; obserwacja w pilocie, ticket 10,
scenariusz d).

**Status:** needs-triage

**Skąd:** review Spec ticketu 09, runda 2 (2026-10-28). `ship/SKILL.md`, „Rounds”: pliki
`.scratch/<feature>/findings/<NN>-round-<r>.md` liczą zużyte rundy, a ticket, który ma już trzy, jest `stuck`.
Ścieżka `stuck` tych plików nie usuwa. Ticket, który doszedł do `needs-info` po trzech rundach, ma trzy
takie pliki, więc po ręcznym przywróceniu `**Status:** ready-for-agent` kolejny `/ship <feature>` od razu uzna
go znowu za `stuck`. Ticket zatrzymany wcześniej (decyzja użytkownika, chroniona ścieżka, brak commitów
w rundzie 1) ruszyłby od następnej rundy. Przewodnik (`docs/workflow.md`, „Wznawianie i stany
pośrednie”) opisuje to jako lukę.

- [ ] Opisana i działająca droga od `needs-info` do ponownego `ready-for-agent`: co robi użytkownik (decyzja,
      zmiana statusu, commit na gałęzi w worktree) i od której rundy rusza ticket.
- [ ] Przewodnik `docs/workflow.md` opisuje tę drogę zamiast luki.
