# 17: Sprawdzenie środowiska przed implementerem, także po wznowieniu

**What to build:** Implementer nigdy nie zaczyna pracy bez środowiska z `environmentChecks`, także po pauzie
i po wznowieniu przerwanego przebiegu. Problem środowiska nie zużywa rundy ani nie podnosi modelu do Opusa.

**Blocked by:** None

**Seams:** do ustalenia przy triage (możliwe: `ship/SKILL.md` w forku, krok „Check environment” przed każdym
wysłaniem implementera albo po każdym wznowieniu; reguła rund, która `stuck` z powodu środowiska liczy
inaczej niż `stuck` z powodu kodu; obserwacja w kolejnym przebiegu z zatrzymanym Dockerem).

**Status:** needs-triage

**Skąd:** pilot 10, przebieg 2, część 2 (`todo-comment-count`, 2026-10-30). `/ship` sprawdził środowisko
na starcie przebiegu (22:46, Docker uruchomiony przez pętlę) oraz po każdym raporcie implementera. Po
wznowieniu o 08:42 (restart komputera w trakcie pauzy, ticket 16) od razu sprawdził limit i wysłał
implementerów 02 i 03, bez `run-dod.ps1 -Check environment`. Implementer 03 pracował bez Dockera: 33 z 34
testów integracyjnych padło, więc zgłosił `STATUS: stuck`. Reguła „`stuck` w rundzie 1 lub 2 → od razu
runda 3 na Opusie” wysłała poprawkę do Opusa. Dopiero sprawdzenie po raporcie (08:46:05) uruchomiło
Dockera. Ticket skończył się `done`, ale problem środowiska kosztował rundę 3 na Opusie.

- [ ] Przed każdym wysłaniem implementera (albo przynajmniej po każdej pauzie i wznowieniu) `/ship` sprawdza
      środowisko i uruchamia to, czego brakuje.
- [ ] Implementer, który zgłasza `stuck` tylko z powodu środowiska, nie zużywa rundy i nie podnosi modelu.
