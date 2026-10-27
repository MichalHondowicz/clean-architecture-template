# 05: Limity, wznawianie i pomiar

**What to build:** Pętla pilnuje limitu planu. Przed każdym ticketem sprawdza zużycie. Powyżej progu
okna 5-godzinnego czeka w tej samej sesji do resetu, a skrypt trzyma komputer w czuwaniu, więc praca
rusza dalej beze mnie. Powyżej progu tygodniowego kończy sesję w spójnym stanie i wysyła push, a po resecie
wznawiam `/ship` sam. Każdy ticket zostawia wiersz pomiaru, a ficzer sumę, więc po pilotach decyzja
o planie wynika z liczb.

**Blocked by:** 03

**Seams:** żywy mini-przebieg na ficzerze-zabawce z progiem okna 5-godzinnego w pliku DoD ustawionym tuż
nad bieżącym zużyciem. Obserwacja: pauza, czekanie w sesji, wznowienie po resecie i tabela pomiaru.
Skrypt czekania: testy procesu w `tests/HookTests`, jak `run-dod.ps1`.

**Status:** done

- [x] Przed każdym ticketem pętla odczytuje zużycie planu i nie zaczyna ticketu, gdy limit tygodniowy albo
      okno 5-godzinne jest na progu lub wyżej (historia 40).
- [x] Powyżej progu okna 5-godzinnego: stan zapisany w trackerze, sesja `/ship` uruchamia w tle skrypt,
      który trzyma komputer w czuwaniu i kończy się po godzinie resetu, a jego zakończenie wznawia tę samą
      sesję: ponowny odczyt zużycia i następny ticket. Powyżej progu tygodniowego: stan zapisany
      w trackerze, sesja kończy się czysto, a `/ship <feature>` po resecie wpisuję sam (historia 41,
      zmieniona na bramce przed implementacją, patrz `## Comments`).
- [x] Oczekiwanie na reset tygodniowy wysyła jeden push. Oczekiwanie na okno 5-godzinne nie wysyła żadnego
      (historie 43, 44).
- [x] Progi pochodzą z pliku DoD, a zmiana progu w pliku zmienia zachowanie pętli (historia 42).
- [x] Tabela pomiaru w katalogu ficzera ma wiersz na ticket: zużycie okna 5-godzinnego i tygodniowego na
      starcie i na końcu, liczba rund, model w każdej roli, czas, wynik. Na końcu ficzera jest suma
      (historia 47).
- [x] Żywy mini-przebieg: pauza i wznowienie odbywają się bez mojego udziału, a wynik jest zapisany
      w `## Comments` tego ticketu.
- [x] Wersja forka podbita.

## Comments

**2026-10-26, decyzje przed implementacją** (bramka z handoffu `docs/handoff-2026-10-26-agent-loop-05.md`,
wszystkie rekomendacje przyjęte, pytanie 1 rozstrzygnięte po sondzie):

1. **Wznawianie: czekanie w tej samej sesji, nie zadanie zaplanowane.** Kryterium 2 zmienione.
   - **Okno 5-godzinne:** stan w trackerze, potem `/ship` uruchamia w tle (`run_in_background`) skrypt
     czekania. Jego zakończenie budzi sesję, a ta odczytuje zużycie ponownie i bierze ticket. Bez pusha.
   - **Reset tygodniowy:** stan w trackerze, jeden push, czysty koniec sesji. Po resecie użytkownik wpisuje
     `/ship <feature>` sam.
   - **Dlaczego nie zadanie zaplanowane aplikacji** (sonda 21:12, zadanie jednorazowe z promptem
     `/ship-probe`, skill tylko dla człowieka):
     - prompt dociera jako zwykły tekst, więc komenda tylko dla człowieka się nie rozwija, a model nie może
       wywołać `ship` (`disable-model-invocation`);
     - uruchomienie dostaje model sesji, która utworzyła zadanie (tu Opus), i jej folder (główny checkout);
     - sesja nie zmieni własnego modelu, bo `set_session_model` odmawia dla siebie;
     - w uruchomieniu nie ma `request_keep_awake`;
     - uruchomienie zapisuje tylko to, co nazywa sam plik zadania;
     - tryb auto odrzucił `create_scheduled_task`, a dopisanie reguły do `.claude/settings.json` odrzucił
       jako samomodyfikację.
   - **Ryzyka wariantu:** restart aplikacji przerywa czekanie (wtedy `/ship` wznawia człowiek), kontekst
     sesji rośnie. Pilot (10) pokaże, czy to przeszkadza.
2. **Odczyt zużycia: `mcp__ccd_session_mgmt__get_usage`.**
   - Okno `5-hour limit` porównywane z `quota.fiveHourPercent`, a `Weekly · all models`
     z `quota.weeklyPercent`.
   - Działa w sesji tej bramki i w uruchomieniu z harmonogramu.
   - Status `unavailable`: jedna ponowna próba, potem pętla nie pauzuje, a tabela dostaje „brak danych”.
3. **Usypianie: skrypt czekania w `.claude/hooks/`.**
   - Trzyma komputer przez `SetThreadExecutionState` (bez zmiany ustawień systemu) i kończy się 2 minuty po
     `resetsAt`.
   - Fakty: desktop bez baterii, usypianie po 15 min na zasilaniu sieciowym, keep-awake aplikacji puszcza
     ok. 5 min po tym, jak sesja przestaje pracować.
   - Część deterministyczna (argumenty, koniec o czasie, kod wyjścia) testowana na seamie `tests/HookTests`.
   - Wpis komendy na allowliście: jeśli tryb auto znów zablokuje edycję `.claude/settings.json`, linię dodaje
     użytkownik.
4. **Effort:** implementer `medium`, weryfikator `low`, orkiestrator `medium` (pole `effort` we frontmatterze
   ról i skilla). Pilot (10) zmierzy, czy jakość nie spada.
5. **Model sesji `/ship`: zawsze Sonnet.** Czekanie w sesji zachowuje model, więc wznowienie po oknie
   5-godzinnym nie ma problemu z modelem. Przewodnik (09) opisze start na Sonnecie.
6. **Tabela pomiaru:** `.scratch/<feature>/metrics.md`.
   - Jeden wiersz na ticket: okno 5-godzinne i tygodniowe na starcie i na końcu, rundy, model w każdej roli,
     czas, wynik.
   - Po ostatnim tickecie wiersz sumy.
   - Commit razem z `ship: <NN> done`.
7. **Nieśledzony `.codex/`:** zostaje nieśledzony, bez commita i bez `.gitignore`. Jeśli Codex będzie
   wykonawcą, kopię wygeneruje skrypt ze ścieżkami względnymi.
8. **Push na telefon:** `PushNotification` dociera na telefon tylko przy włączonym Remote Control
   (mini-przebieg z 03 miał je wyłączone). W 05 `/ship` tylko wysyła push, a włączenie Remote Control dla
   sesji `/ship` przechodzi do ticketu 06.

**Stan limitu przy bramce:** okno 5-godzinne 23% (reset 27.10 o 01:40), tydzień 22% (reset 30.10 o 07:00).

**2026-10-27, implementacja i przebiegi na żywo**

Co powstało:

- `run-dod.ps1 -Check quota -FiveHourPercent <n> -WeeklyPercent <n>`: porównuje zużycie z progami z `dod.json`.
  Exit 1 wymienia przekroczone okna, tygodniowe pierwsze. Exit 2 oznacza procent spoza 0–100 albo brak progu.
- `wait-for-reset.ps1 -Until <resetsAt>` (nowy):
  - trzyma komputer przez `SetThreadExecutionState`;
  - kończy się 2 minuty po resecie;
  - exit 2 przy czasie bez daty albo offsetu i przy resecie dalej niż 6 h.
- `HookProcess` zabija drzewo procesu po timeoucie testu, bo osierocony skrypt trzymał komputer i potoki.
- Fork:
  - `/ship`: sekcje „Plan usage” i „Metrics”, sprawdzenie zużycia przed baseline'em, `effort: medium`;
  - implementer `medium`, verifier `low`;
  - `docs/engineering/ship.md`.

Dowody kryteriów:

- **AC1 (40):** `RunDod_Should_ReportTheFiveHourWindow_WhenItsUsageReachesTheThreshold`
  i `RunDod_Should_CompareTheUsageUnrounded_WhenItHasDecimals`. Na żywo:
  - A: tydzień 26% przy progu 20, żaden ticket nie ruszył;
  - B2: 01 ruszył przy 5h 12% (próg 14), a po nim 16% zatrzymało 02.
- **AC2 (41):** `WaitForResetTests`, 6 metod i 9 przypadków. Na żywo B2:
  - 09:38:23 commit pauzy `5630026` (tylko `metrics.md`);
  - skrypt w tle z `-Until 2026-10-27T10:29:59.633Z`;
  - wyjście 0 o 11:31:59;
  - ta sama sesja odczytała zużycie (5h 1%) i o 11:32:19 zrobiła `ship: start 02`;
  - 02 done o 11:37, po skrypcie nie został żaden proces.

  Tydzień, A: wiersz pauzy, commit tylko `metrics.md`, koniec bez buildu i bez subagentów. A2: ponowny start
  przed resetem skończył się bez wiersza i bez pusha. Trzymanie komputera dowodzi
  `WaitForReset_Should_HoldOffIdleSleep_WhenRunningOnWindows`. W B2 komputer nie zasnął przez 1 h 54 min, ale
  nie wiem, czy ktoś był wtedy przy nim, więc to słabszy dowód.
- **AC3 (43, 44):**
  - A wywołał push raz i nie doszedł, bo terminal był aktywny;
  - A2 drugiego nie wysłał;
  - B2 przy oknie 5-godzinnym pusha nie wysłał.

  Dostarczenie na telefon wymaga Remote Control (ticket 06).
- **AC4 (42):** `RunDod_Should_FollowTheThresholdsInTheDodFile_WhenTheFileChangesThem` (4 wiersze)
  i `RunDod_Should_ExitWithUsageError_WhenAThresholdIsNotAPercentage`. Na żywo progi sandboksa: tydzień 20 →
  stop, 5h 14 → pauza.
- **AC5 (47):** `metrics.md` z B2:
  - wiersze 01 i 02: 1 runda, modele czterech ról, po 4 min, 5h 12→16 i 1→6, tydzień 28→29 i 29→30;
  - wiersze obu pauz;
  - suma: `2 done, 0 needs-info`, 2 rundy, 8 min, `+9` i `+2`.
- **AC6:** B2 przeszedł od 09:33 do 11:37 bez mojego udziału.
- **AC7:** fork `1.3.1-dotnet.12`, po przebiegach `1.3.1-dotnet.13`.

Dowody czerwieni:

- **Czerwone w cyklu:**
  - testy quota i wait-for-reset z TDD;
  - po review: procent spoza 0–100 (zużycie i próg);
  - ułamki bez zaokrąglania;
  - czas bez offsetu (ten test raz czekał naprawdę ok. 3 h, stąd zabijanie procesu w `HookProcess`).
- **Red proof** (mutacja, test czerwony, plik przywrócony z tym samym hashem):
  - próg 5h na sztywno 90;
  - `-gt` zamiast `-ge`;
  - próg tygodniowy na sztywno;
  - reset w przeszłości;
  - test pliku projektu (usunięte `weeklyPercent`).

Review na osi Spec (Sonnet) miało dwie rundy:

- **Runda 1:** 9 uwag.
- **Runda 2:** 7 naprawionych, 2 częściowo, oraz nowe drobne uwagi:
  - runda 1 liczy tylko commity spoza `ship: `;
  - sprawdzenie zużycia dopiero, gdy jest ticket do wzięcia;
  - limit dwóch czekań liczony w przebiegu, nie z wierszy.

Wszystko poprawione w `94e93c5`, sprawdzone przebiegami, bez trzeciej rundy.

Co wyszło w przebiegach i co poprawiono:

- **A:** sesja w worktree odrzuca złożone polecenia z `git` albo `pwsh`. `/ship` dostał regułę prostych poleceń
  (fork `58d2d2f`).
- **A2:** `resetsAt` dryfuje o ułamek sekundy między odczytami, więc reset dopasowuje się z dokładnością do
  godziny (`dcd742e`).
- **B:** baseline był czerwony, bo test przypinał progi projektu 80/90. Teraz test sprawdza tylko, czy progi
  dają się użyć (szablon `96c0b70`).
- **B2** (`cadbf78`, wersja `.13`):
  - verifier i implementer składali polecenia z `cd`, `;` i `$(...)`, więc obie role dostały regułę prostych
    poleceń;
  - czas pauzy był wpisany z głowy (09:40 zamiast 09:38), więc czas i liczby w tabeli mają pochodzić z wyniku
    polecenia;
  - czekanie poszło równolegle z commitem pauzy, więc teraz rusza dopiero po commicie;
  - Bash dwa razy odrzucił `pwsh` w tle, a narzędzie PowerShell uruchomiło to samo polecenie, więc skrypt
    czekania idzie przez PowerShell.

Minuty w tabeli są obcinane: 4,9 min daje 4. Zostawiam, bo suma i tak pokazuje rząd wielkości.

Testy: `dotnet build` i `dotnet test CleanArchitecture.slnx --no-build`, 155 zielonych:

| Projekt | Testy |
|---|---|
| ArchitectureTests | 6 |
| Application.UnitTests | 54 |
| IntegrationTests | 14 |
| HookTests | 81 |

Commity:

- szablon: `974fb60`, `3cc9572`, `96c0b70` i commit zamykający;
- fork: `94e93c5`, `58d2d2f`, `dcd742e`, `cadbf78`;
- sandbox: `feat/toy-quota` od `afb53e7` do `f6910fe`.
