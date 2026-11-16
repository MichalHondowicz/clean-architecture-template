# 19: Resztki pilota 10: wznowienie bez użytkownika i push przy pytaniu o zgodę

**What to build:** Dwa kryteria pilota 10, których pilot nie mógł sprawdzić, zostają sprawdzone przy okazji
następnego prawdziwego ficzera puszczonego przez `/ship`, bez osobnego przebiegu ustawionego tylko pod nie.

**Blocked by:** None

**Seams:** obserwacja w kolejnym przebiegu `/ship` (statusy ticketów, `metrics.md`, log sesji w
`~/.claude/projects/<projekt>/*.jsonl`, telefon). Bez zmian w kodzie, dopóki obserwacja nie pokaże błędu.

**Status:** ready-for-human

**Skąd:** pilot 10 (2026-10-29/30), przeniesione decyzją użytkownika z 2026-11-01 przy zamknięciu ticketu 10.

- Scenariusz (c): w przebiegu 2 pauza na progu 49% zadziałała (`ship: pause, 5-hour limit`, wiersz pauzy,
  `wait-for-reset.ps1` w tle). Wznowienia nie było: o 23:47 Windows Update zrestartował komputer (ticket 16).
  Rano przebieg ruszył dopiero po wiadomości użytkownika w starej sesji. W przebiegu 1 limit nie padł.
- Push przy pytaniu o zgodę: w trzech przebiegach pilota (todo-comments, todo-comment-count część 1–3)
  nie padło ani jedno pytanie o zgodę, więc nie było czego wysłać.

- [ ] Pauza na limit 5-godzinny kończy się wznowieniem bez udziału użytkownika: `wait-for-reset.ps1` wraca,
      a pętla sama sprawdza limit i środowisko (ticket 17) i rusza z kolejnym ticketem (historie 40, 41).
- [ ] Pytanie o zgodę w sesji `/ship` w tle daje push na telefon. Jeśli push nie dociera jako powiadomienie,
      wynik trafia do ticketu 12 (fallback: hook `Notification` i ntfy) (historia 46).

## Comments

**2026-11-01, triage (decyzja użytkownika):** `ready-for-human`. To obserwacja, którą robi użytkownik przy
następnym prawdziwym ficzerze, nie praca dla agenta. Najlepiej po ticketach 17 i 16, żeby wznowienie
sprawdzić już ze sprawdzeniem środowiska i ostrzeżeniem o restarcie.

**2026-11-11, przebieg `mobile-smoke-emulator` w Koszyku:** pauza o 16:51 do 20:40, restart systemu o 20:25, sesja dostała
komunikat o przerwanym `wait-for-reset.ps1` i nic nie zrobiła (0 tur), jak w pilocie 10. Wznowienia bez restartu nadal
nikt nie widział. Poprawka w forku (`1.3.1-dotnet.35`): taki komunikat oznacza „wznów z trackera”. Nie wiadomo, czy
aplikacja daje sesji turę na ten komunikat; pokaże to następny restart w trakcie czekania.

**2026-11-15, z zamknięcia ticketu 26:** do drugiej obserwacji dochodzi część AC1 tamtego ticketu: gdy pytanie
o zgodę padnie w przebiegu `/ship` w Koszyku, zapisz też, czy trafiło potem do allowlisty Koszyka. W trzech
przebiegach pilota 2 nie ma zapisu żadnego pytania o zgodę.

**2026-11-15 (noc), przebieg `mobile-smoke-hardening` w Koszyku** (zapis agenta z repozytorium Koszyka, `main`
`9db2b11`; szczegóły przebiegu w tickecie 27):

- **Wznowienie po pauzie:** brak obserwacji. Przebieg trwał 72 minuty w jednym oknie 5h (4% → 66%), historia nie
  ma commita `ship: pause`, więc `wait-for-reset.ps1` nie ruszył. Poprawka forka `.35` dalej nie jest sprawdzona
  na żywo.
- **Pytanie o zgodę:** z repozytorium nie widać, czy padło. `.claude/` Koszyka nie zmienił się między planem
  (`3f2f371`) a bramką 2 (`9db2b11`), więc do allowlisty w repozytorium nic nie doszło. Czy pytanie padło i czy
  przyszło powiadomienie, wie użytkownik; do uzupełnienia po jego odpowiedzi (także AC5 ticketu 12).

**2026-11-15 (noc), odpowiedź użytkownika:** w przebiegu `mobile-smoke-hardening` nie padło żadne pytanie o zgodę.
Nie było więc czego wysłać ani dopisać do allowlisty. Oba kryteria zostają otwarte do następnego przebiegu `/ship`.
