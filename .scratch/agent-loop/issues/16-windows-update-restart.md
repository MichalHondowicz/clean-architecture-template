# 16: Restart Windows Update przerywa pętlę bez nadzoru

**What to build:** Pętla bez nadzoru przeżywa noc bez restartu systemu, a gdy restart jednak nastąpi, użytkownik
wie o nim rano od razu i wie, co wpisać.

**Blocked by:** None

**Seams:** sprawdzenie oczekującego restartu jako proces w `tests/HookTests` (rejestr podstawiony, np. przez
parametr ścieżki klucza), wywołanie w `wait-for-reset.ps1` i na starcie `/ship` (fork: `ship/SKILL.md`);
przewodnik `docs/workflow.md`; obserwacja w kolejnym przebiegu nocnym.

**Status:** done

**Skąd:** pilot 10, przebieg 2 (`todo-comment-count`, 2026-10-29). Sesja `/ship` czekała na reset okna
5-godzinnego (23:50) w `wait-for-reset.ps1`, który trzyma komputer w czuwaniu. O 23:47:47
`TrustedInstaller.exe` zrestartował komputer („Operating System: Upgrade (Planned)”, zdarzenie 1074), a
o 23:48:27 `winlogon.exe` go wyłączył. Komputer wstał dopiero o 08:22. Sesja i skrypt czekania zginęły, push
nie poszedł, a stan w trackerze był spójny (wiersz pauzy, ticket 02 `ready-for-agent`). Keep-awake
nie chroni przed restartem aktualizacji.

- [x] Na starcie `/ship` i przed czekaniem w `wait-for-reset.ps1` sprawdzany jest oczekujący restart (klucze
      `HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\WindowsUpdate\Auto Update\RebootRequired` i
      `HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\Component Based Servicing\RebootPending`). Gdy restart
      czeka, idzie push z ostrzeżeniem, a przebieg trwa dalej. Godziny aktywne nie są ochroną (zob. Comments).
- [x] Przewodnik `docs/workflow.md` mówi, jak rozpoznać rano restart systemu i jak wznowić przebieg: wiadomość
      w starej sesji `/ship` albo `/ship <feature>` w nowej.
- [x] Wersja forka podbita.
- [x] Allowlista w `.claude/settings.json` ma wpisy dla `check-reboot.ps1` (dopisuje użytkownik, bo tryb auto
      blokuje edycję tego pliku), inaczej wywołanie pyta o zgodę w sesji bez nadzoru.

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

**2026-11-01, wynik:**

- **Template** (`aedd586`, `34362b1`): nowy `.claude/hooks/check-reboot.ps1` sprawdza klucze `RebootRequired`
  i `RebootPending` pod `-KeysRoot` (domyślnie rejestr). Exit 1: restart czeka, 0: nie czeka (także poza
  Windowsem), 2: sprawdzenie się nie udało, więc błąd nigdy nie wygląda jak oczekujący restart. Testy
  `CheckRebootTests` (3) podają katalog zamiast rejestru, bo CI działa na Ubuntu. Dowód red: 3 failed
  (brak skryptu), potem green. Na prawdziwym rejestrze: tymczasowy klucz w HKCU dał `pending: true` i exit 1,
  klucz usunięty. Na tym komputerze teraz `pending: false`. Build 0 ostrzeżeń, HookTests 88/88. Przewodnik:
  „Limity” odsyła do nowej części „Restart systemu w nocy” w „Wznawianie i stany pośrednie” (jak rozpoznać
  rano, obie drogi wznowienia), a tabela hooków wymienia nowy skrypt.
- **Fork** (`62f76ce`, `ce62779`; plugin `1.3.1-dotnet.22`, zainstalowany): skrypt jest opcjonalny.
  `/ship` uruchamia go w kroku 1 startu i w „Plan usage” krok 4, po commicie pauzy, a przed
  `wait-for-reset.ps1`. Exit 1 wysyła raz na przebieg push „Windows restart pending…”, a przebieg trwa dalej.
  Sprawdzenie stoi w `/ship`, a nie w `wait-for-reset.ps1`, bo skrypt nie może wysłać pusha. `wait-for-reset.ps1`
  się nie zmienił.
- **Review Spec** (Sonnet): `fail` z 2 findings. (1) brak wpisu w allowliście, więc wywołanie pytałoby
  o zgodę: zostaje dla użytkownika (ostatnie kryterium). (2) błąd skryptu dawał exit 1 jak oczekujący restart:
  poprawione w `34362b1` i `ce62779`.
- **Do dopisania przez użytkownika** w `.claude/settings.json`, w `permissions.allow`, obok wpisów
  `wait-for-reset.ps1` (wywołanie jest bez argumentów, więc wzorzec bez ` *`):
  `"Bash(pwsh -NoProfile -NonInteractive -File .claude/hooks/check-reboot.ps1)"` oraz
  `"PowerShell(pwsh -NoProfile -NonInteractive -File .claude/hooks/check-reboot.ps1)"`. Sandbox ma swoją kopię
  hooków i ustawień: trzeba tam przenieść oba skrypty (`run-dod.ps1` z ticketu 17 i `check-reboot.ps1`) i te
  wpisy.

**2026-11-11:** użytkownik dopisał wpisy `check-reboot.ps1` do allowlisty (sprawdzone na dysku, plik jest poprawnym JSON-em). Ticket `done`.
