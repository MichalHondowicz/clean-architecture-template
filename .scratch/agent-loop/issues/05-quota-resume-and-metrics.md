# 05: Limity, wznawianie i pomiar

**What to build:** Pętla pilnuje limitu planu. Przed każdym ticketem sprawdza zużycie, a powyżej progu
kończy sesję w spójnym stanie. Lokalne zadanie zaplanowane wznawia wtedy `/ship` po resecie, beze mnie.
Każdy ticket zostawia wiersz pomiaru, a ficzer sumę, więc po pilotach decyzja o planie wynika z liczb.

**Blocked by:** 03

**Seams:** żywy mini-przebieg na ficzerze-zabawce z progiem w pliku DoD ustawionym tuż nad bieżącym
zużyciem. Obserwacja: pauza, zadanie zaplanowane, wznowienie i tabela pomiaru.

**Status:** ready-for-agent

- [ ] Przed każdym ticketem pętla odczytuje zużycie planu i nie zaczyna ticketu, gdy limit tygodniowy albo
      okno 5-godzinne jest na progu lub wyżej (historia 40).
- [ ] Powyżej progu: stan zapisany w trackerze, lokalne zadanie zaplanowane na godzinę resetu uruchamia
      `/ship <feature>`, a sesja kończy się czysto (historia 41).
- [ ] Oczekiwanie na reset tygodniowy wysyła jeden push. Oczekiwanie na okno 5-godzinne nie wysyła żadnego
      (historie 43, 44).
- [ ] Progi pochodzą z pliku DoD, a zmiana progu w pliku zmienia zachowanie pętli (historia 42).
- [ ] Tabela pomiaru w katalogu ficzera ma wiersz na ticket: zużycie okna 5-godzinnego i tygodniowego na
      starcie i na końcu, liczba rund, model w każdej roli, czas, wynik. Na końcu ficzera jest suma
      (historia 47).
- [ ] Żywy mini-przebieg: pauza i wznowienie odbywają się bez mojego udziału, a wynik jest zapisany
      w `## Comments` tego ticketu.
- [ ] Wersja forka podbita.
