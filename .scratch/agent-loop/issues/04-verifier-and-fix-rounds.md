# 04: Weryfikator i rundy poprawek

**What to build:** Po implementerze każdy ticket przechodzi osobnego weryfikatora i review Spec. Uwagi
w zakresie ticketu wracają do implementera, najwyżej w 3 rundach, z trzecią na Opusie. Ticket, który mimo to
nie zbiega, dostaje `needs-info` i brief, a ja dostaję push i rano wiem, co zdecydować. Niezależne tickety
idą dalej.

**Blocked by:** 02, 03

**Seams:** pilot (ticket 10): scenariusz (d) i każdy ticket pilota. Do czasu pilota tekst skilla sprawdza
review Spec.

**Status:** done

- [x] Weryfikator jest osobną rolą. Część mechaniczną wykonują skrypty z 01 i 02 (DoD, chronione ścieżki,
      wyciszenia, lista zmienionych testów). Część oceniającą wykonuje Haiku: każde `ACn` ma w `## Comments`
      istniejący, przechodzący test z odnotowanym dowodem czerwieni, a zmienione istniejące testy spoza
      ticketu są ocenione (historie 22, 23, 24).
- [x] Review Spec ticketu na Sonnecie odbywa się po zielonym weryfikatorze (historia 25).
- [x] Uwagi w zakresie ticketu trafiają do pliku uwag i wracają do implementera. Najwyżej 3 rundy, trzecia
      z implementerem na Opusie (historie 26, 27).
- [x] Po wyczerpaniu rund ticket dostaje `needs-info` i brief (co próbowano, gdzie stoi, czego potrzeba),
      a push zawiera jedno zdanie i wskazanie briefu (historie 28, 43, 45).
- [x] Tickety zależne od zablokowanego czekają, niezależne idą dalej (historia 29).
- [x] Uwagi spoza zakresu ticketu stają się nowymi ticketami `needs-triage` i niczego nie blokują
      (historia 30).
- [x] Naruszenie wykryte przez weryfikatora (osłabiony test, wyciszenie, chroniona ścieżka) liczy się jako
      uwaga w zakresie ticketu i nigdy nie zostaje zaakceptowane (historia 32).
- [x] Wersja forka podbita.

## Comments

**2026-10-26, `/implement`:**

- **Fork** (`e71cd59` po `7d5b7e1`; plugin `1.3.1-dotnet.9` zainstalowany):
  - `agents/verifier.md`: Haiku, tylko do odczytu (`Bash`, `Read`, `Grep`, `Glob`). Uruchamia
    `run-dod.ps1 -Check commands` i `check-diff.ps1 -Base`, ocenia `testsToReview` względem ticketu,
    sprawdza dla każdego `ACn` istnienie testu, brak `Skip` i dowód czerwieni. Odpowiada
    `VERDICT: pass` z `CONFIRMED: AC…`, `fail` albo `error`, gdy skrypt zwróci 2.
  - `agents/spec-reviewer.md`: Sonnet, tylko do odczytu, oś Spec. Oznacza uwagi `in-scope` i `out-of-scope`
    i dostaje wcześniejsze uwagi, żeby się nie powtarzać.
  - `skills/engineering/ship/SKILL.md`, rundy:
    - najwyżej 3, licznik to pliki `findings/<NN>-round-<r>.md`;
    - runda 3 na Opusie, a `STATUS: stuck` w rundzie 1 lub 2 eskaluje do niej, chyba że chodzi o decyzję
      albo ścieżkę chronioną;
    - przed weryfikacją stash resztek i sprawdzenie środowiska;
    - naruszenie weryfikatora nigdy nie jest odpuszczane, a ścieżkę chronioną przywraca z bazy orkiestrator;
    - `needs-info` z briefem Tried / Where it stands / Needs i pushem;
    - cofanie tylko commitów implementera (commity `ship:` zostają);
    - uwagi spoza zakresu jako tickety `needs-triage` bez duplikatów.
  - `agents/implementer.md`: rozwiązuje uwagi poprzedniej rundy. Strona `docs/engineering/ship.md`
    zsynchronizowana.
- **Template:**
  - `check-diff.ps1` zgłasza projekt usunięty z pliku solucji (`solution-project-removed`). Dodanie projektu
    jest dozwolone, bo etap 2 doda projekty MAUI. Nagłówek pliku w diffie (`--- a/…`) nie jest już mylony
    z usuniętą linią.
  - Na allowliście dokładna komenda `check-diff.ps1`.
  - `issue-tracker.md` opisuje pliki uwag, brief i tickety `needs-triage`.
- **Testy:** build 0/0; Architecture 6, Unit 54, Integration 14, Hook 59, wszystkie zielone. Test usuniętego
  projektu był czerwony przed zmianą. `check-diff -Base ae3deb4` na prawdziwym repo: czysto.
- **Seam ticketu to pilot (10).** Tekst skilla i ról przeszedł review Spec (Sonnet). Kryteria są odhaczone,
  bo opisane zachowanie istnieje, ale żadne nie było jeszcze sprawdzone na żywo: ścieżki fail, `stuck`,
  `needs-info` i push czekają na scenariusz (d) pilota.
- **Review Spec znalazł 2 luki z poprzednich ticketów i 9 błędów logiki rund.** Wszystkie poprawione:
  - projekty usunięte z solucji; test pomijany przez `Skip`; kod 2 skryptów;
  - resztki poza commitami widoczne dla DoD, a niewidoczne dla diffu;
  - `git revert` kasował commity z uwagami i ticketami spoza zakresu;
  - zmiana testu wymagana przez sam ticket była traktowana jako osłabienie;
  - naruszenie ścieżki chronionej było niemożliwe do naprawy przez implementera;
  - wyścig blokady DoD a weryfikator;
  - „brak commitów” sprawdzany w rundach 2 i 3;
  - `stuck` w rundzie 1 lub 2 omijał Opusa;
  - duplikaty ticketów spoza zakresu;
  - niejednoznaczne odhaczanie kryteriów.
- **Do pilota (10):** czy powiadomienie o końcu pracy sub-agenta przychodzi przed werdyktem blokady DoD.
  Skill każe czekać, aż implementer naprawdę skończy.
