# 16: Restart Windows Update przerywa pętlę bez nadzoru

**What to build:** Pętla bez nadzoru przeżywa noc bez restartu systemu, a gdy restart jednak nastąpi, użytkownik
wie o nim rano od razu i wie, co wpisać.

**Blocked by:** None

**Seams:** do ustalenia przy triage (możliwe: krok w `wait-for-reset.ps1` albo start `/ship`, który czyta
oczekujący restart Windows Update; ustawienie godzin aktywnych jako krok przewodnika; obserwacja w kolejnym
przebiegu nocnym).

**Status:** needs-triage

**Skąd:** pilot 10, przebieg 2 (`todo-comment-count`, 2026-10-29). Sesja `/ship` czekała na reset okna
5-godzinnego (23:50) w `wait-for-reset.ps1`, który trzyma komputer w czuwaniu. O 23:47:47
`TrustedInstaller.exe` zrestartował komputer („Operating System: Upgrade (Planned)”, zdarzenie 1074), a
o 23:48:27 `winlogon.exe` go wyłączył. Komputer wstał dopiero o 08:22. Sesja i skrypt czekania zginęły, push
nie poszedł, a stan w trackerze był spójny (wiersz pauzy, ticket 02 `ready-for-agent`). Keep-awake
nie chroni przed restartem aktualizacji.

- [ ] Ustalona ochrona przed restartem w trakcie przebiegu (np. godziny aktywne, sprawdzenie oczekującego
      restartu przed startem albo w czekaniu) albo świadoma decyzja, że jej nie ma.
- [ ] Przewodnik `docs/workflow.md` mówi, że restart systemu, tak jak restart aplikacji, wymaga ponownego
      `/ship <feature>`, i jak go rozpoznać rano.

## Comments

**2026-10-30, wznowienie po restarcie:** ponowny `/ship` nie był potrzebny. Po starcie aplikacji stara sesja
dostała o 08:41:53 powiadomienie „Background shell command didn't finish before the previous session ended”
o `wait-for-reset.ps1`, ale sama nic nie zrobiła. Ruszyła dopiero po wiadomości użytkownika „jak status?”
(08:42:06): sprawdziła limit i wróciła do kolejnego ticketu. Przy wznowieniu pominęła sprawdzenie
środowiska (ticket 17). Drugie kryterium powinno więc opisywać obie drogi: wiadomość w starej sesji albo
`/ship <feature>` w nowej.
