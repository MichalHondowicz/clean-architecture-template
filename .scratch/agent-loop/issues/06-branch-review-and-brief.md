# 06: Review całej gałęzi i brief gotowości

**What to build:** Po ostatnim tickecie Opus raz przegląda całą gałąź (`ca-review` i review Spec całego
ficzera). Pętla poprawia uwagi w zakresie według tej samej reguły 3 rund. Na koniec dostaję push „gotowe”
i brief, z którego w minutę decyduję o scaleniu.

**Blocked by:** 04

**Seams:** pilot (ticket 10).

**Status:** ready-for-agent

- [ ] Po ostatnim tickecie `ca-review` i review Spec całej gałęzi odbywają się raz, na Opusie (historia 33).
- [ ] Uwagi w zakresie ficzera przechodzą regułę 3 rund. Nierozwiązane trafiają do briefu jako otwarte.
- [ ] Brief w katalogu ficzera zawiera: co zrobione, liczby testów, uwagi z review, rzeczy niepewne,
      zmienione ekrany (gdy są) oraz propozycje konwencji, każdą z regułą, uzasadnieniem i ticketem,
      na którym wyszła (historia 34).
- [ ] Push „gotowe do bramki 2” zawiera jedno zdanie i wskazanie briefu (historie 43, 45).
- [ ] Wersja forka podbita.

## Comments

**2026-10-26, przeniesione z bramki ticketu 05:** `PushNotification` dociera na telefon tylko przy włączonym
Remote Control, a sesja `/ship` z mini-przebiegu w 03 miała je wyłączone. Bramka 2 i tak wymaga Remote
Control (historia 35), więc tu zapada decyzja, czy `/ship` włącza je dla siebie
(`mcp__ccd_session_mgmt__set_session_remote_control`), czy robi to użytkownik przy starcie.
