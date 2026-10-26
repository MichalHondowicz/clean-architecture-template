# 04: Weryfikator i rundy poprawek

**What to build:** Po implementerze każdy ticket przechodzi osobnego weryfikatora i review Spec. Uwagi
w zakresie ticketu wracają do implementera, najwyżej w 3 rundach, z trzecią na Opusie. Ticket, który mimo to
nie zbiega, dostaje `needs-info` i brief, a ja dostaję push i rano wiem, co zdecydować. Niezależne tickety
idą dalej.

**Blocked by:** 02, 03

**Seams:** pilot (ticket 10): scenariusz (d) i każdy ticket pilota. Do czasu pilota tekst skilla sprawdza
review Spec.

**Status:** ready-for-agent

- [ ] Weryfikator jest osobną rolą. Część mechaniczną wykonują skrypty z 01 i 02 (DoD, chronione ścieżki,
      wyciszenia, lista zmienionych testów). Część oceniającą wykonuje Haiku: każde `ACn` ma w `## Comments`
      istniejący, przechodzący test z odnotowanym dowodem czerwieni, a zmienione istniejące testy spoza
      ticketu są ocenione (historie 22, 23, 24).
- [ ] Review Spec ticketu na Sonnecie odbywa się po zielonym weryfikatorze (historia 25).
- [ ] Uwagi w zakresie ticketu trafiają do pliku uwag i wracają do implementera. Najwyżej 3 rundy, trzecia
      z implementerem na Opusie (historie 26, 27).
- [ ] Po wyczerpaniu rund ticket dostaje `needs-info` i brief (co próbowano, gdzie stoi, czego potrzeba),
      a push zawiera jedno zdanie i wskazanie briefu (historie 28, 43, 45).
- [ ] Tickety zależne od zablokowanego czekają, niezależne idą dalej (historia 29).
- [ ] Uwagi spoza zakresu ticketu stają się nowymi ticketami `needs-triage` i niczego nie blokują
      (historia 30).
- [ ] Naruszenie wykryte przez weryfikatora (osłabiony test, wyciszenie, chroniona ścieżka) liczy się jako
      uwaga w zakresie ticketu i nigdy nie zostaje zaakceptowane (historia 32).
- [ ] Wersja forka podbita.
