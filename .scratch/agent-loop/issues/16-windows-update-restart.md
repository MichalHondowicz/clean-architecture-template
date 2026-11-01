# 16: Restart Windows Update przerywa pętlę bez nadzoru

**What to build:** Pętla bez nadzoru przeżywa noc bez restartu systemu, a gdy restart jednak nastąpi, użytkownik
wie o nim rano od razu i wie, co wpisać.

**Blocked by:** None

**Seams:** sprawdzenie oczekującego restartu jako proces w `tests/HookTests` (rejestr podstawiony, np. przez
parametr ścieżki klucza), wywołanie w `wait-for-reset.ps1` i na starcie `/ship` (fork: `ship/SKILL.md`);
przewodnik `docs/workflow.md`; obserwacja w kolejnym przebiegu nocnym.

**Status:** ready-for-agent

**Skąd:** pilot 10, przebieg 2 (`todo-comment-count`, 2026-10-29). Sesja `/ship` czekała na reset okna
5-godzinnego (23:50) w `wait-for-reset.ps1`, który trzyma komputer w czuwaniu. O 23:47:47
`TrustedInstaller.exe` zrestartował komputer („Operating System: Upgrade (Planned)”, zdarzenie 1074), a
o 23:48:27 `winlogon.exe` go wyłączył. Komputer wstał dopiero o 08:22. Sesja i skrypt czekania zginęły, push
nie poszedł, a stan w trackerze był spójny (wiersz pauzy, ticket 02 `ready-for-agent`). Keep-awake
nie chroni przed restartem aktualizacji.

- [ ] Na starcie `/ship` i przed czekaniem w `wait-for-reset.ps1` sprawdzany jest oczekujący restart (klucze
      `HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\WindowsUpdate\Auto Update\RebootRequired` i
      `HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\Component Based Servicing\RebootPending`). Gdy restart
      czeka, idzie push z ostrzeżeniem, a przebieg trwa dalej. Godziny aktywne nie są ochroną (zob. Comments).
- [ ] Przewodnik `docs/workflow.md` mówi, jak rozpoznać rano restart systemu i jak wznowić przebieg: wiadomość
      w starej sesji `/ship` albo `/ship <feature>` w nowej.
- [ ] Wersja forka podbita.

## Comments

**2026-10-30, wznowienie po restarcie:** ponowny `/ship` nie był potrzebny. Po starcie aplikacji stara sesja
dostała o 08:41:53 powiadomienie „Background shell command didn't finish before the previous session ended”
o `wait-for-reset.ps1`, ale sama nic nie zrobiła. Ruszyła dopiero po wiadomości użytkownika „jak status?”
(08:42:06): sprawdziła limit i wróciła do kolejnego ticketu. Przy wznowieniu pominęła sprawdzenie
środowiska (ticket 17). Drugie kryterium powinno więc opisywać obie drogi: wiadomość w starej sesji albo
`/ship <feature>` w nowej.

**2026-11-01, triage (decyzja użytkownika):** `ready-for-agent`, drugi w kolejce. Godziny aktywne Windows Update
są na tym komputerze ustawione na 18:00–0:00 (`ActiveHoursStart` 18, `ActiveHoursEnd` 0), a restart z 23:47
wypadł w ich trakcie (TrustedInstaller, „Upgrade (Planned)”). Wcześniejszy taki restart: 2026-10-14 10:17.
Dlatego kryterium 1 to sprawdzenie oczekującego restartu z ostrzeżeniem, a nie godziny aktywne. Wstrzymanie
aktualizacji to ustawienie systemu użytkownika, nie krok pętli.
