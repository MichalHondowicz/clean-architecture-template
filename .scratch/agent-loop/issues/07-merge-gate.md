# 07: Bramka 2: scalenie po „OK”

**What to build:** Na brief odpowiadam jedną wiadomością w tej samej sesji, także z telefonu. Po „OK” agent
scala gałąź do `main`, wypycha zmiany, sprząta i sprawdza CI. O każdej propozycji konwencji decyduję osobno,
a przyjęte trafiają do repozytorium osobnym commitem.

**Blocked by:** 06

**Seams:** pilot (ticket 10).

**Status:** ready-for-agent

- [ ] Po briefie sesja czeka na odpowiedź. Odpowiedź przez Remote Control działa tak samo jak z komputera
      (historia 35).
- [ ] Po „OK”: fast-forward do `main`, `git push origin main`, usunięcie gałęzi i worktree, status CI
      sprawdzony w przeglądarce i raport z wynikiem (historia 36).
- [ ] Allowlista zawiera dokładnie `git push origin main`, bez gwiazdki. Push innej gałęzi albo z `--force`
      nadal wymaga zgody (historia 37).
- [ ] Każda propozycja konwencji ma osobną decyzję, niezależną od zgody na scalenie (historia 38).
- [ ] Przyjęte propozycje trafiają osobnym commitem orkiestratora na gałąź ficzera przed scaleniem.
      Odrzucone wypadają (historia 39).
- [ ] Bez „OK” nic nie trafia do `main`.
- [ ] Wersja forka podbita.
