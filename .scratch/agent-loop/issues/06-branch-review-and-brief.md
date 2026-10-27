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

**2026-10-27, decyzje przed implementacją** (bramka w sesji, która zamknęła ticket 05; wszystkie rekomendacje
przyjęte):

1. **Remote Control: ustawienie aplikacji „Connect new sessions to Remote Control” włączone.** Każda nowa
   sesja, także `/ship`, łączy się sama.
   - `/ship` nie może włączyć go sobie sam. Według opisu `set_remote_control` narzędzia wolno użyć tylko na
     prośbę użytkownika, aplikacja prosi o zgodę, a sesjom bez nadzoru odmawia.
   - Na starcie `/ship` odczytuje swój stan (`get_session self`, pole `remoteControlState`) i zapisuje go
     w raporcie i w briefie. Dzięki temu wiadomo, dlaczego push nie dotarł na telefon.
   - Ustawienie zmienione 2026-10-27 przez `set_setting`. Przewodnik (09) ma o nim wspomnieć.
2. **Review gałęzi tylko wtedy, gdy wszystkie tickety są `done`** i żaden nie da się już wziąć.
   - Nowe tickety `needs-triage` się nie liczą.
   - Ticket z `needs-info` kończy przebieg jak dotąd (push o tym tickecie już poszedł). Review robi następny
     `/ship` po odblokowaniu.
3. **Dwa subagenty na Opusie, równolegle.**
   - **Standards:** skill standardów projektu, wskazany w `dod.json` (nowe pole; tu `ca-review`), z bazą
     `git merge-base main HEAD`.
   - **Spec:** `spec-reviewer` z `model: opus` porównuje całą gałąź ze `spec.md` ficzera i jego ticketami.
   - Odrzucone: wywołanie skilla `code-review` w sesji `/ship`. Działałby na Sonnecie orkiestratora, a o bazę
     i spec pyta interaktywnie.
4. **Rundy poprawek gałęzi:**
   - plik `findings/branch-round-<r>.md` z uwagami w zakresie, a implementer dostaje ten plik i spec;
   - verifier sprawdza DoD i diff, a dowodu czerwieni wymaga tylko dla uwag, które zmieniają zachowanie;
   - ponowne review na Opusie obejmuje tylko zakres poprawki i uwagi z tej rundy, nie całą gałąź;
   - najwyżej 3 rundy, a nierozwiązane uwagi trafiają do briefu jako otwarte.
   - Odrzucone: generowany ticket poprawek w pełnym cyklu ticketu (cięższy, a dowód czerwieni dla uwag
     o nazewnictwie nie ma sensu).
5. **Liczby testów:** `run-dod.ps1 -Check commands` wypisuje też `passed: [{name, output}]` z ostatnimi
   20 liniami każdego udanego polecenia. `/ship` przepisuje do briefu linie podsumowania testów. Testy procesu
   są w `tests/HookTests`.
6. **Propozycje konwencji** pochodzą z dwóch źródeł:
   - reviewer Standards proponuje regułę dla wzorca, którego nie obejmują obecne konwencje;
   - ten sam rodzaj uwagi w plikach uwag co najmniej dwóch ticketów też staje się propozycją.

   Każda propozycja ma regułę, uzasadnienie i ticket, na którym wyszła. Kontrakt implementera bez zmian.
7. **Limit:** to samo sprawdzenie zużycia co przed ticketem (pauza przy oknie 5-godzinnym, koniec przy
   tygodniowym) przed review gałęzi i przed każdą rundą poprawek.
8. **Przebieg na żywo w sandboksie:**
   - ficzer-zabawka z jednym ticketem;
   - na gałęzi zasiane naruszenie konwencji (`DateTime.UtcNow` w Application), które review gałęzi musi
     znaleźć, żeby ruszyła runda poprawek;
   - obserwacja briefu i pusha przy włączonym Remote Control;
   - lokalny `main` sandboksu przesunięty do konfiguracji pętli, żeby `merge-base main` dawał bazę ficzera;
     nic nie jest wypychane.

**Ustalone bez pytania:**

- Brief leży w `.scratch/<feature>/brief.md` i ma commit `ship: brief`.
- Push brzmi `ship <feature>: ready for gate 2, see .scratch/<feature>/brief.md`.
- Ponowny `/ship` przy gotowym briefie i bez nowych commitów kończy się bez pusha.
- Sekcja „zmienione ekrany” pojawia się dopiero wtedy, gdy `dod.json` wskaże ścieżki ekranów (etap MAUI).

**Stan limitu przy bramce:** okno 5-godzinne 15% (reset 27.10 o 16:30), tydzień 31% (reset 30.10 o 07:00).
