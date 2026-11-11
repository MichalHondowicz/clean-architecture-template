# 12: Fallback powiadomień: hook `Notification` i ntfy

**What to build:** Powiadomienia pętli docierają na telefon niezależnie od narzędzia `PushNotification`.
- Skrypt w szablonie wysyła jedno zdanie do tematu ntfy.
- `/ship` woła go przy każdym swoim pushu.
- Hook `Notification` woła go, gdy sesja w tle czeka na zgodę albo na odpowiedź.

**Blocked by:** None

**Seams:** skrypt `notify.ps1` testowany jako proces w `tests/HookTests`, z lokalnym serwerem HTTP zamiast ntfy.
Rejestrację hooka i wywołania w `/ship` sprawdza następny prawdziwy przebieg (ticket 19).

**Status:** ready-for-human

**Skąd:**
- Ticket 06: testy pusha po przebiegu C, 2026-10-27. Narzędzie cztery razy odpowiedziało „Mobile push requested”,
  a na telefon nie dotarł żaden push, także po wyłączeniu i włączeniu Remote Control. Szczegóły są w Comments
  ticketu 06.
- Ticket 10: jego kryterium przewiduje ten ticket, gdy push nie dociera.
- Spec: historie 43, 45 i 46 oraz uwaga przy decyzji o pushu przy pytaniu o zgodę („fallback to hook
  `Notification` i ntfy”).

- [x] AC1: `notify.ps1 -Message <tekst>` wysyła treść POST-em na adres tematu z konfiguracji i kończy się
      kodem 0 (test: `NotifyTests.Notify_Should_PostTheMessageToTheTopic_WhenATopicIsConfigured`).
- [x] AC2: Bez skonfigurowanego tematu skrypt nic nie wysyła i kończy się kodem 0. Brak fallbacku nie zatrzymuje
      pętli (test: `NotifyTests.Notify_Should_SendNothing_WhenNoTopicIsConfigured`).
- [x] AC3: Odpowiedź spoza 2xx albo błąd sieci kończy się kodem 1 i opisem na stderr
      (test: `NotifyTests.Notify_Should_Fail_WhenTheServerRejectsTheMessage`). `/ship` traktuje to jak niewysłany
      push i podaje go w raporcie. Nie jest to awaria środowiska.
- [x] AC4: `/ship` woła skrypt z tą samą treścią przy każdym ze swoich pushy: gotowe do bramki 2, `needs-info`,
      limit tygodniowy, awaria środowiska (historie 43, 45).
- [ ] AC5: Hook `Notification` woła skrypt, gdy sesja w tle czeka na zgodę albo na odpowiedź (historia 46).
      Sprawdza to pilot. `.claude/settings.json` dopisuje użytkownik, bo tryb auto blokuje jego edycję.
- [x] AC6: Adres tematu i ewentualny token nie leżą w repozytorium. Przewodnik (ticket 09) opisuje, jak je ustawić.
- [x] Wersja forka podbita.

## Do rozstrzygnięcia przy triage

1. **Serwer ntfy:**
   - publiczny ntfy.sh: temat jest sekretem tylko przez swoją nazwę, a treść widzi każdy, kto ją zna;
   - własny serwer na Dokploy z tokenem dostępu.
2. **Miejsce adresu i tokenu:** zmienna środowiskowa użytkownika albo plik w `~/.claude`.
3. **Los `PushNotification`:** zostaje obok ntfy (dwa powiadomienia, kiedy oba działają) albo ntfy go zastępuje.
4. **Zakres hooka `Notification`:** jak go ograniczyć do sesji bez nadzoru, żeby nie powiadamiał przy pracy przy
   ekranie. Trzeba też sprawdzić, czy ten hook w ogóle uruchamia się w sesjach aplikacji desktopowej.
5. **Treść:** tylko jedno zdanie i ścieżka briefu, bez kodu i danych z repozytorium, bo treść trafia do usługi
   zewnętrznej.

## Comments

**2026-10-29, z pilota 10 (przebieg 1, `todo-comments`):** push `PushNotification` o bramce 2 z sesji `/ship`
z Remote Control `on` znowu nie przyszedł jako powiadomienie systemowe. Narzędzie odpowiedziało „Mobile push
requested”, a wiadomość była widoczna dopiero po otwarciu aplikacji Claude na telefonie. Piąty push z rzędu bez
powiadomienia, więc fallback jest nadal potrzebny.

**2026-11-01, triage (decyzja użytkownika):** `ready-for-agent`, ostatni w kolejce (największy). Odpowiedzi na
pytania z „Do rozstrzygnięcia przy triage”:

1. **Serwer:** publiczny ntfy.sh z losowym tematem o długości co najmniej 24 znaków. Treść to jedno zdanie bez
   kodu, więc własny serwer na Dokploy nie jest potrzebny.
2. **Adres:** pełny adres tematu w zmiennej środowiskowej użytkownika `CLAUDE_NTFY_URL`, poza repozytorium.
   Bez zmiennej skrypt nic nie wysyła (AC2).
3. **`PushNotification`:** zostaje obok ntfy. Dwa powiadomienia, gdy oba działają, są akceptowalne.
4. **Hook `Notification`:** tylko matcher `permission_prompt`. Bramkę 2, `needs-info`, limit tygodniowy
   i awarię środowiska pokrywa `/ship`, który sam woła `notify.ps1` (AC4). Pierwszy krok ticketu: sprawdzić,
   czy hook `Notification` w ogóle uruchamia się w sesjach aplikacji desktopowej. Jeśli nie, AC5 przechodzi
   do ticketu 19 jako obserwacja, a reszta ticketu idzie bez niego.
5. **Treść:** jedno zdanie i ścieżka briefu, bez kodu i danych z repozytorium.

**2026-11-01, wynik:**

- **Template** (`7629132`): `.claude/hooks/notify.ps1`. `-Message <tekst>` wysyła POST na adres
  z `CLAUDE_NTFY_URL`. Bez `-Message` czyta JSON hooka ze stdin i wysyła jego `message`. Kody wyjścia: 0
  wysłane albo brak zmiennej (`{"sent":false,…}`), 1 odrzucone lub brak połączenia (stderr), 2 brak treści.
  Testy `NotifyTests`: trzy z AC1–AC3 i czwarty dla trybu hooka. Lokalny `TopicServer` zastępuje ntfy,
  a `HookProcess.RunWithEnvironmentAsync` ustawia albo usuwa `CLAUDE_NTFY_URL` w każdym teście, więc test
  nigdy nie trafi do prawdziwego tematu. Dowód red: 4 failed (brak skryptu), potem green. Build 0 ostrzeżeń,
  HookTests 92/92. Przewodnik: „Powiadomienia” opisuje ustawienie w pięciu krokach.
- **Fork** (`7eed158`, plugin `1.3.1-dotnet.27`, zainstalowany): `/ship` po każdym pushu wysyła tę samą
  linię przez `notify.ps1`, jeśli projekt go ma. Exit 1 to niewysłany push w raporcie i nie zatrzymuje
  przebiegu. Pokrywa sześć pushy: bramka 2, `needs-info`, limit tygodniowy, awaria środowiska, oczekujący
  restart, czerwone CI.
- **Hook `Notification`** (dokumentacja https://code.claude.com/docs/en/hooks.md): stdin ma `message`,
  opcjonalne `title` i `notification_type`. `permission_prompt` odpala około 6 s po pytaniu o zgodę,
  w aplikacji desktopowej od v2.1.233 (zainstalowana 2.1.311). Czy pytanie o zgodę pada w trybie auto,
  dokumentacja nie mówi. Sprawdzi to ticket 19.
- **Review Spec** (Sonnet): `pass`. Jedyny warunek: wpisy do `settings.json` muszą stać w tym tickecie,
  bo odsyła tu przewodnik.

**Do zrobienia przez użytkownika** (AC5; tryb auto blokuje agentowi edycję `.claude/settings.json`):

1. Zmienna użytkownika `CLAUDE_NTFY_URL` z losowym tematem ntfy.sh, restart aplikacji, subskrypcja
   w aplikacji ntfy, test `notify.ps1 -Message "test"` (przewodnik, „Powiadomienia”).
2. W `.claude/settings.json`, w `hooks`, obok `PreToolUse` i `SubagentStop`:

   ```json
   "Notification": [
     {
       "matcher": "permission_prompt",
       "hooks": [
         {
           "type": "command",
           "command": "pwsh -NoProfile -NonInteractive -File \"${CLAUDE_PROJECT_DIR}/.claude/hooks/notify.ps1\""
         }
       ]
     }
   ]
   ```
3. W `permissions.allow`, obok wpisów `wait-for-reset.ps1`:
   `"Bash(pwsh -NoProfile -NonInteractive -File .claude/hooks/notify.ps1 *)"` i
   `"PowerShell(pwsh -NoProfile -NonInteractive -File .claude/hooks/notify.ps1 *)"`.
4. Sandbox ma swoją kopię hooków i ustawień: przenieś tam `notify.ps1` i te wpisy przed następnym przebiegiem.

**2026-11-11:** użytkownik dopisał hook `Notification` i wpisy `notify.ps1` do `.claude/settings.json` (sprawdzone na dysku: hook leży w `hooks`, plik jest poprawnym JSON-em), `CLAUDE_NTFY_URL` jest ustawiony. AC5 zostaje otwarte do pierwszego pytania o zgodę w przebiegu `/ship` (ticket 19).
