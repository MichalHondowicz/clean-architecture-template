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

**Status:** ready-for-agent

- [ ] Przed każdym ticketem pętla odczytuje zużycie planu i nie zaczyna ticketu, gdy limit tygodniowy albo
      okno 5-godzinne jest na progu lub wyżej (historia 40).
- [ ] Powyżej progu okna 5-godzinnego: stan zapisany w trackerze, sesja `/ship` uruchamia w tle skrypt,
      który trzyma komputer w czuwaniu i kończy się po godzinie resetu, a jego zakończenie wznawia tę samą
      sesję: ponowny odczyt zużycia i następny ticket. Powyżej progu tygodniowego: stan zapisany
      w trackerze, sesja kończy się czysto, a `/ship <feature>` po resecie wpisuję sam (historia 41,
      zmieniona na bramce przed implementacją, patrz `## Comments`).
- [ ] Oczekiwanie na reset tygodniowy wysyła jeden push. Oczekiwanie na okno 5-godzinne nie wysyła żadnego
      (historie 43, 44).
- [ ] Progi pochodzą z pliku DoD, a zmiana progu w pliku zmienia zachowanie pętli (historia 42).
- [ ] Tabela pomiaru w katalogu ficzera ma wiersz na ticket: zużycie okna 5-godzinnego i tygodniowego na
      starcie i na końcu, liczba rund, model w każdej roli, czas, wynik. Na końcu ficzera jest suma
      (historia 47).
- [ ] Żywy mini-przebieg: pauza i wznowienie odbywają się bez mojego udziału, a wynik jest zapisany
      w `## Comments` tego ticketu.
- [ ] Wersja forka podbita.

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
