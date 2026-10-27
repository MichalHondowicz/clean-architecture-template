# 12: Fallback powiadomień: hook `Notification` i ntfy

**What to build:** Powiadomienia pętli docierają na telefon niezależnie od narzędzia `PushNotification`.
- Skrypt w szablonie wysyła jedno zdanie do tematu ntfy.
- `/ship` woła go przy każdym swoim pushu.
- Hook `Notification` woła go, gdy sesja w tle czeka na zgodę albo na odpowiedź.

**Blocked by:** None

**Seams:** skrypt `notify.ps1` testowany jako proces w `tests/HookTests`, z lokalnym serwerem HTTP zamiast ntfy.
Rejestrację hooka i wywołania w `/ship` sprawdza pilot (ticket 10).

**Status:** needs-triage

**Skąd:**
- Ticket 06: testy pusha po przebiegu C, 2026-10-27. Narzędzie cztery razy odpowiedziało „Mobile push requested”,
  a na telefon nie dotarł żaden push, także po wyłączeniu i włączeniu Remote Control. Szczegóły są w Comments
  ticketu 06.
- Ticket 10: jego kryterium przewiduje ten ticket, gdy push nie dociera.
- Spec: historie 43, 45 i 46 oraz uwaga przy decyzji o pushu przy pytaniu o zgodę („fallback to hook
  `Notification` i ntfy”).

- [ ] AC1: `notify.ps1 -Message <tekst>` wysyła treść POST-em na adres tematu z konfiguracji i kończy się
      kodem 0 (test: `NotifyTests.Notify_Should_PostTheMessageToTheTopic_WhenATopicIsConfigured`).
- [ ] AC2: Bez skonfigurowanego tematu skrypt nic nie wysyła i kończy się kodem 0. Brak fallbacku nie zatrzymuje
      pętli (test: `NotifyTests.Notify_Should_SendNothing_WhenNoTopicIsConfigured`).
- [ ] AC3: Odpowiedź spoza 2xx albo błąd sieci kończy się kodem 1 i opisem na stderr
      (test: `NotifyTests.Notify_Should_Fail_WhenTheServerRejectsTheMessage`). `/ship` traktuje to jak niewysłany
      push i podaje go w raporcie. Nie jest to awaria środowiska.
- [ ] AC4: `/ship` woła skrypt z tą samą treścią przy każdym ze swoich pushy: gotowe do bramki 2, `needs-info`,
      limit tygodniowy, awaria środowiska (historie 43, 45).
- [ ] AC5: Hook `Notification` woła skrypt, gdy sesja w tle czeka na zgodę albo na odpowiedź (historia 46).
      Sprawdza to pilot. `.claude/settings.json` dopisuje użytkownik, bo tryb auto blokuje jego edycję.
- [ ] AC6: Adres tematu i ewentualny token nie leżą w repozytorium. Przewodnik (ticket 09) opisuje, jak je ustawić.
- [ ] Wersja forka podbita.

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
