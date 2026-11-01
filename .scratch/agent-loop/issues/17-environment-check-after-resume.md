# 17: Sprawdzenie środowiska przed implementerem, także po wznowieniu

**What to build:** Implementer nigdy nie zaczyna pracy bez środowiska z `environmentChecks`, także po pauzie
i po wznowieniu przerwanego przebiegu. Problem środowiska nie zużywa rundy ani nie podnosi modelu do Opusa.

**Blocked by:** None

**Seams:** `ship/SKILL.md` w forku (krok „Check environment” przed każdym wysłaniem implementera; reguła rund
dla `stuck` z powodu środowiska); obserwacja w kolejnym przebiegu z zatrzymanym Dockerem (ticket 19).

**Status:** done

**Skąd:** pilot 10, przebieg 2, część 2 (`todo-comment-count`, 2026-10-30). `/ship` sprawdził środowisko
na starcie przebiegu (22:46, Docker uruchomiony przez pętlę) oraz po każdym raporcie implementera. Po
wznowieniu o 08:42 (restart komputera w trakcie pauzy, ticket 16) od razu sprawdził limit i wysłał
implementerów 02 i 03, bez `run-dod.ps1 -Check environment`. Implementer 03 pracował bez Dockera: 33 z 34
testów integracyjnych padło, więc zgłosił `STATUS: stuck`. Reguła „`stuck` w rundzie 1 lub 2 → od razu
runda 3 na Opusie” wysłała poprawkę do Opusa. Dopiero sprawdzenie po raporcie (08:46:05) uruchomiło
Dockera. Ticket skończył się `done`, ale problem środowiska kosztował rundę 3 na Opusie.

- [x] Przed każdym wysłaniem implementera `/ship` sprawdza środowisko i uruchamia to, czego brakuje.
- [x] Gdy sprawdzenie po raporcie implementera musiało coś uruchomić, `/ship` powtarza tę samą rundę na tym
      samym modelu. Ta powtórka nie liczy się do trzech rund i nie podnosi modelu do Opusa.
- [x] Wersja forka podbita.

## Comments

**2026-11-01, triage (decyzja użytkownika):** `ready-for-agent`, pierwszy w kolejce 17 → 16 → 14 → 18 → 13 → 15
→ 12. Sprawdzenie przed każdym implementerem zamiast „tylko po wznowieniu”, bo jest prostsze i pokrywa oba
przypadki. Tickety 17, 14, 18 i 13 zmieniają ten sam `ship/SKILL.md` (14: `plan-feature/SKILL.md`), więc idą
po kolei.

**2026-11-01, wynik:**

- **Template** (`10bd525`): `run-dod.ps1 -Check environment` drukuje `"started":[name]` ze sprawdzeniami,
  które musiał uruchomić. Bez tego `/ship` nie wiedziałby, że środowisko padło w trakcie rundy. Nowe testy
  `RunDodTests.RunDod_Should_ReportTheStartedCheck_WhenItHadToBeStarted` i
  `RunDod_Should_ReportNothingStarted_WhenEveryCheckAlreadyPasses`. Dowód red: oba padały na braku pola
  `started` (2 failed, 25 passed), potem build 0 ostrzeżeń, HookTests 85/85.
- **Fork** (`cc4a003`, `6baddc2`, `918e69c`; plugin `1.3.1-dotnet.21`, zainstalowany): w „Rounds” krok 1 (Build)
  sprawdza środowisko przed implementerem. Pokrywa to start, wznowienie ticketu `in-progress`, powrót po
  pauzie 5-godzinnej i rundy brancha. Krok 3 sprawdza środowisko po raporcie, przed reakcją na status.
  Gdy `started` coś wymienia, ta sama runda idzie jeszcze raz z tymi samymi wejściami, na tym samym modelu,
  bez pliku findings. Druga powtórka tej samej rundy to awaria środowiska. „Fix rounds” odwołuje się do
  kroków 1–3, a Verify znowu sprawdza środowisko, bo wznowienie na Verify nie wysyła implementera.
- **Review Spec** (Sonnet): runda 1 `fail` z 3 findings (brak sprawdzenia przy wznowieniu na Verify,
  niejasne „no findings file”, odwołanie do kroku 4 z regułami tylko dla ticketów). Wszystkie poprawione
  w `918e69c`, re-review `pass`.
- **Do pamiętania:** projekt ze starszym `run-dod.ps1` (bez `started`) nigdy nie powtórzy rundy, ale
  sprawdzenie przed implementerem działa. Sandbox `D:\Projects\ship-sandbox` ma swoją kopię hooków: przed
  następnym przebiegiem tam trzeba przenieść nowy `run-dod.ps1`. Obserwacja na żywo: ticket 19.
