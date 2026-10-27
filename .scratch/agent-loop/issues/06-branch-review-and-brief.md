# 06: Review całej gałęzi i brief gotowości

**What to build:** Po ostatnim tickecie Opus raz przegląda całą gałąź (`ca-review` i review Spec całego
ficzera). Pętla poprawia uwagi w zakresie według tej samej reguły 3 rund. Na koniec dostaję push „gotowe”
i brief, z którego w minutę decyduję o scaleniu.

**Blocked by:** 04

**Seams:** pilot (ticket 10).

**Status:** done

- [x] Po ostatnim tickecie `ca-review` i review Spec całej gałęzi odbywają się raz, na Opusie (historia 33).
- [x] Uwagi w zakresie ficzera przechodzą regułę 3 rund. Nierozwiązane trafiają do briefu jako otwarte.
- [x] Brief w katalogu ficzera zawiera: co zrobione, liczby testów, uwagi z review, rzeczy niepewne,
      zmienione ekrany (gdy są) oraz propozycje konwencji, każdą z regułą, uzasadnieniem i ticketem,
      na którym wyszła (historia 34).
- [x] Push „gotowe do bramki 2” zawiera jedno zdanie i wskazanie briefu (historie 43, 45).
- [x] Wersja forka podbita.

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

**2026-10-27, implementacja (`/implement`):**

- **Szablon:**
  - `run-dod.ps1 -Check commands` wypisuje też `passed: [{name, output}]` z ostatnimi 20 liniami każdego
    udanego polecenia. Polecenia po pierwszym nieudanym się nie uruchamiają, więc nie ma ich ani w `passed`,
    ani w `failed`.
  - `dod.json` ma pole `standardsSkill: "ca-review"`. Progi limitu bez zmian (80/90).
  - `implementer-contract.md` opisuje rundę poprawek gałęzi: wejście to spec i
    `findings/branch-round-<r>.md`, wyjście to `## Resolution` na końcu tego pliku, a tickety zostają nietknięte.
  - `issue-tracker.md` opisuje wiersz `branch` w metrykach, wiersz sumy na końcu przebiegu, pliki
    `findings/branch-review.md` i `branch-round-<r>.md` oraz `brief.md`.
- **Fork, `/ship`:**
  - na starcie odczytuje `remoteControlState` (`get_session self`) i nigdy go nie przełącza;
  - gdy wszystkie tickety są `done` (`needs-triage` i `wontfix` się nie liczą), uruchamia sekcję
    `## Branch review`. Stan wznowienia wynika z plików uwag: `Started`, raporty, plik rundy, `## Resolution`,
    `## Result`;
  - zleca dwa review naraz na Opusie: nowy agent `standards-reviewer` (skill z `standardsSkill`, baza
    `git merge-base main HEAD`) oraz `spec-reviewer` z `model: opus` na całej gałęzi;
  - rundy poprawek działają według reguł ticketu: najwyżej 3 rundy, 3. runda na Opusie, a uwag verifiera nigdy
    się nie odpuszcza. Verifier sprawdza wszystko od bazy rundy 1. Ponowne review na Opusie obejmuje tylko
    zmianę z rundy i osie z pliku uwag;
  - przy otwartych uwagach i niezaliczonym ostatnim verifierze kod wraca jednym poleceniem
    `git restore --source=<baza> --staged --worktree -- . ":(exclude).scratch/<feature>"`;
  - brief ma sekcje Ready, Done, Tests, Review, Uncertain, Screens (tylko przy `screenPaths`), Convention
    proposals i Remote Control. Commit `ship: brief` zawiera też wiersz `branch` i wiersz sumy, a po nim idzie
    push.
- **Pozostałe agenty:** verifier, implementer i spec-reviewer dostały tryby dla gałęzi.

**Dowody (TDD na szwie `tests/HookTests`, proces `run-dod.ps1`):**

- **Testy z cyklu:** `RunDod_Should_ReportTheLastLinesOfEachPassingCommand_WhenCheckingCommands` i
  `RunDod_Should_ReportOnlyTheCommandsBeforeTheFailureAsPassed_WhenACommandFails`. Oba były czerwone
  (`KeyNotFoundException` na `passed`).
- **Dowody mutacyjne:** po każdej mutacji plik wrócił z tym samym hashem `3D663C97…606`.
  - Ogon 21 linii zamiast 20 → test 1 czerwony.
  - Nieudane polecenie dopisane do `passed` → test 2 czerwony („Sequence contains more than one element”).
- **20 linii wystarcza:** `dotnet test` wypisuje cztery podsumowania `Passed!` w ostatnich 7 liniach.
- **Pełny build i testy, łańcuchowo:** zielone, 6 + 54 + 14 + 83 = 157 testów.

**Review na osi Spec (Sonnet), dwie rundy:**

- **Runda 1, 11 uwag:**
  - brak stanu wznowienia; wznowienie pomijało verifiera;
  - kolizja `--grep` na `ship: branch review`;
  - rundy poprawek niezgodne z regułami ticketu;
  - licznik zużycia i sprawdzenia uruchamiane mimo gotowego briefu;
  - `ca-review` budował równolegle i dublował pokrycie AC;
  - ponowne review pomijało konwencje;
  - uwagi poza zakresem nie trafiały do ticketów;
  - niejasne, które linie to liczby testów;
  - niezamówiony flow `brief outdated`.

  Poprawki: lista wznowienia z `## Result`, `--diff-filter=A`, tryb ponownego review w `standards-reviewer`,
  `ca-review` bez budowania i bez specu; flow `brief outdated` usunięty.
- **Runda 2, 7 nowych uwag:**
  - konflikty revertów na plikach uwag → jeden `git restore` (sprawdzony w repozytorium testowym);
  - zakres verifiera od bazy rundy 1;
  - uwagi gubione po niezaliczonym verifierze → przenoszone dalej;
  - nieokreślone „żadna nie przeszła”;
  - usunięcie briefu przez użytkownika musi być commitem;
  - brakujące nazwy commitów;
  - baseline w środku rundy: zostawione jako znane ograniczenie (to samo dotyczy wznowionego ticketu
    `in-progress`, bo baseline idzie na jego bieżącym drzewie).
- **Trzecia runda:** nie odbyła się.

**Odstępstwa i uwagi:**

- Kontrakt implementera zmienił się tylko dla rundy gałęzi. Decyzja 6 („kontrakt bez zmian”) dotyczy propozycji.
- Obok `needs-triage` doszedł `wontfix`: porzucony ticket też nie blokuje review gałęzi.
- Flow `brief outdated` odpadł: do ponownego review użytkownik usuwa w commicie `brief.md` i
  `findings/branch-*.md`.
- FAQ o Remote Control zostało w przewodniku.

**Przebieg na żywo C w sandboksie** (`D:\Projects\ship-sandbox`):

- **Konfiguracja:**
  - gałąź `feat/toy-brief`, lokalny `main` na `53439c4`;
  - ticket 01 zrobiony ręcznie, z zasianym `DateTime.UtcNow` w `src/Application/Todos/TodoAging.cs`;
  - ticket 02 do zbudowania;
  - plugin `.14`, Sonnet z effort medium, tryb auto, ustawienie Remote Control dla nowych sesji włączone.
- **Przebieg, 15:14–15:26 (12 minut):**
  - `ship: start 02 (5h 52%, week 36%)`; ticket 02 `done` w 1 rundzie;
  - `ship: start branch review (5h 56%, week 36%)`;
  - `standards-reviewer` (Opus) wywołał `ca-review` przez Skill z bazą `53439c4` i trybem tylko raportu. Znalazł
    zasiane naruszenie jako uwagę w zakresie (reguła Time z `AGENTS.md`). Dał też 4 notatki i 2 propozycje
    konwencji: logikę spoza use case'ów na encji, z czasem jako parametrem, oraz miejsce i nazwy testów typów
    Domain;
  - `spec-reviewer` (Opus): pass;
  - runda 1: implementer zmienił sygnaturę na `IsStale(TodoItem, DateTime utcNow)` i dopisał `## Resolution`;
    verifier i ponowne review Standards na Opusie: pass;
  - commity `ship: branch round 1 resolved` i `ship: brief` (`d643a8c`, brief razem z wierszami `branch` i
    sumy).
- **Wyniki:**
  - Brief jest po polsku i ma wszystkie sekcje. Sekcji Screens nie ma, bo nie ma `screenPaths`.
  - Liczby testów pochodzą z `passed`: 6 + 59 + 14 + 83. Model skrócił linie o `Duration`, ale liczby się
    zgadzają.
  - Metryki są spójne:
    - wiersz `branch`: start 15:21, koniec 15:26, 5 min, 5h 56→62%, tydzień 36→37%;
    - suma: 2 rundy, 9 min, 5h +10, tydzień +1.
  - Remote Control: `on`. Brief i raport go podają.
- **Push:** treść zgodna z ustaleniem (`ship toy-brief: ready for gate 2, see .scratch/toy-brief/brief.md`),
  ale nie wyszedł. Narzędzie zwróciło „Push not sent — mobile push is disabled in /config”. To drugi warunek
  obok Remote Control i należy do użytkownika: włącza się go w `/config` interaktywnego `claude`.
- **Znaleziska → fork `52969e4`, plugin `1.3.1-dotnet.15`:**
  - **Pwsh w Bash verifiera.** Bash w sesji izolowanej w worktree odmawia każdego polecenia z `pwsh`, także
    prostego. Haiku-verifier nie mógł uruchomić `run-dod` ani `check-diff`, więc orkiestrator uruchomił je
    narzędziem PowerShell i przekazał wynik. Dotyczyło to też verifiera w B2 (6 odmów), czego poprawka po B2
    nie usunęła. Teraz verifier ma narzędzie PowerShell i uruchamia nim oba skrypty; implementer i `/ship`
    mówią to samo. Sprawdzone tylko tekstem, nie przebiegiem: potwierdzi to następny przebieg (pilot, 10).
  - **Niewysłany push.** Raport końcowy `/ship` podaje każdy niewysłany push z powodem. Przewodnik wymienia
    `/config` obok Remote Control.
- **Niesprawdzone na żywo:** ścieżki rund 2 i 3, uwagi otwarte w briefie, przywracanie kodu przez `git restore`
  (sprawdzone tylko w repozytorium testowym) oraz pauza limitu przed review gałęzi.
- **Drobne, bez poprawek:** orkiestrator kilka razy sklejał polecenia mimo reguły, a po odmowie je rozdzielał.
  Implementer ticketu 02 przywrócił po dowodzie mutacyjnym plik `sed`-em z innymi końcami linii, co odnotował w
  Comments.

**Commity:** szablon `6ea4afa` (bramka) i `990da05` (implementacja); fork `cadbf78` i `ff82e81` (`.14`) oraz
`52969e4` (`.15`).

**2026-10-27, dopisek: dostarczanie pusha.** Kryterium pusha jest odhaczone za treść i wysłanie. Dostarczenie na
telefon nie działa.

- **Ustawienia:** po przebiegu C użytkownik zalogował samodzielne CLI kontem claude.ai i włączył w `/config` oba
  przełączniki: **Push when Claude decides** i **Push when actions required** (`agentPushNotifEnabled` i
  `inputNeededNotifEnabled` w `~/.claude/settings.json`).
- **Testy z sesji testowej** (Haiku, Remote Control `on` z ustawienia aplikacji):
  - przy aktywnej aplikacji narzędzie pomija push („Not sent — this terminal is active”);
  - po 3 minutach czekania, z zablokowanym ekranem, narzędzie odpowiada „Mobile push requested”.
- **Wynik:** cztery takie pushe (2–5, w tym ostatni po wyłączeniu i włączeniu Remote Control) nie dotarły na
  telefon, choć telefon był zalogowany na to samo konto, miał zgodę na powiadomienia, a aplikację otwarto.
- **Zgłoszenia publiczne:** opisują ten sam objaw, np. anthropics/claude-code#60383 oraz zgłoszenia o pushach
  ginących po cichu.
- **Wniosek:** przy obecnej wersji push nie jest wiarygodnym sygnałem. Fallback przewiduje ticket 10 (hook
  `Notification` i ntfy).
