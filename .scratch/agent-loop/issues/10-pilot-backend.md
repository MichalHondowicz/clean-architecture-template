# 10: Pilot 1: ficzer backendowy przez całą pętlę

**What to build:** Prawdziwy mały ficzer backendowy przechodzi `/plan-feature`, bramkę 1, `/ship` i bramkę 2
bez mojego udziału poza bramkami. Pilot sprawdza też ścieżki awaryjne i daje pierwszy pomiar zużycia limitu.

**Blocked by:** 09

**Seams:** seam 1 ze speca. Obserwacja przez statusy ticketów, `## Comments`, tabelę pomiaru, brief, historię
gałęzi i otrzymane powiadomienia.

**Status:** ready-for-human

- [x] Wybrany ficzer backendowy (najlepiej 3 tickety) przechodzi od `/plan-feature` do scalenia w `main`
      (historie 1–8, 57).
- [x] Scenariusz (a): między bramkami nie pada żadne pytanie o zgodę. Każde pytanie, które się pojawiło,
      zostaje dopisane do allowlisty (historia 12).
- [ ] Scenariusz (b): Docker zatrzymany przed startem, pętla uruchamia go sama (historia 11).
- [ ] Scenariusz (c): próg w pliku DoD tuż nad bieżącym zużyciem daje pauzę i wznowienie bez mojego udziału
      (historie 40, 41).
- [ ] Scenariusz (d): ticket niemożliwy do zrobienia kończy się `needs-info`, briefem i pushem, a niezależne
      tickety idą dalej (historie 28, 29).
- [ ] Push przy pytaniu o zgodę w sesji w tle dociera na telefon. Jeśli nie dociera, powstaje ticket na
      fallback (hook `Notification` i ntfy) (historia 46).
- [x] Tabela pomiaru jest pełna, a zużycie ficzera zapisane jako pomiar nr 1 (historia 47).
- [x] Propozycje konwencji z briefu są rozstrzygnięte, a rozjazdy pętli zapisane jako tickety `needs-triage`.

## Comments

**2026-10-27, z ticketu 06:** pushe proaktywne z sesji z Remote Control `on` już nie docierają. Narzędzie
odpowiada „Mobile push requested”, ale na telefon nie przyszedł żaden z czterech testów, także po wyłączeniu i
włączeniu Remote Control (szczegóły w Comments ticketu 06). Ticket na fallback (hook `Notification` i ntfy)
założony przed pilotem jako 12 (`needs-triage`).

**2026-10-28, z ticketu 08: pierwszy pełny przebieg `/plan-feature` → `/ship` → bramka 2** (sandbox, ficzer
zabawkowy `todo-open-count`, jeden ticket). To nie jest pilot, tylko dowód, że łańcuch działa bez interwencji
poza bramkami:

- plan: grill na Opusie, jeden commit `plan: todo-open-count`, sesja `/ship` z karty na Sonnecie;
- `/ship`: ticket 01 w jednej rundzie, review gałęzi na Opusie bez uwag w zakresie, uwaga spoza zakresu jako
  ticket 02 (`needs-triage`), brief, odpowiedź na bramce 2, fast-forward i push do gołego origin, gałąź
  i worktree usunięte;
- pomiar: ticket 5 minut i +10% okna 5-godzinnego, review gałęzi 2 minuty i +3%, tydzień +2% łącznie;
- odpowiedź „OK, 1 tak, 2 tak” była pomyłką (zamierzone „2 nie”); konwencję 2 cofnięto ręcznie osobnym
  commitem `b493a0f`. Skill nie ma ścieżki korekty decyzji po scaleniu; do rozważenia w przewodniku (09).

Pilot nadal musi przejść scenariusze (b)–(d) ze spec i ficzer z kilkoma ticketami.

**2026-10-29, decyzje przed pilotem** (bramka w sesji po handoffie
`docs/handoff-2026-10-28-agent-loop-10.md`; wszystkie rekomendacje przyjęte):

**Fakty sprawdzone przed bramką:**

- **Limit na starcie sesji:** okno 5-godzinne 2% (reset 2026-10-29 23:49), tydzień 57% (reset 2026-10-30
  07:00). Przy progu 80% na pilota zostaje 23 punkty tygodnia; ficzer z 08 zużył +2%.
- **Tagi dublują istniejący kod.** `TodoItem` ma już `List<string> Labels`, a `toy-quota` dodał `HasLabel`.
- **`needs-info` zatrzymuje ficzer przed bramką 2.** `ship/SKILL.md`, koniec „Work the tickets”: review gałęzi
  rusza tylko wtedy, gdy każdy ticket jest `done` (`needs-triage` i `wontfix` się nie liczą). Ticket ze
  scenariusza (d) kończy więc przebieg bez briefu i scalenia.
- **`dod.json` czyta worktree.** `run-dod.ps1` szuka pliku od bieżącego katalogu (`Find-DodRoot`), a worktree
  powstaje z HEAD. Zmiana progu musi być zacommitowana na `main` przed `/ship`. Commit przywracający próg na
  `main` po utworzeniu gałęzi rozjechałby historię i fast-forward na bramce 2 by się nie udał.
- **Docker** był zatrzymany (`docker info` bez połączenia z silnikiem), a ścieżka startu z `environmentChecks`
  istnieje. `/plan-feature` Dockera nie potrzebuje.
- **`.agents/`** w template'cie pojawił się znowu 2026-10-29 o 20:46, choć usunięto go w 09. To kopia
  `.claude/skills` bez `README.md`. W sandboksie leży od 2026-10-26.
- **Sandbox:** zarejestrowane worktree `toy-overdue` i `toy-quota` były czyste, a ich gałęzie równe kopiom
  na gołym origin. Puste foldery `todo-open-count` i `toy-brief` zostały po przebiegach.

**Decyzje:**

1. **Miejsce: sandbox.** Krok CI po scaleniu zostaje niesprawdzony; sprawdzi go pierwszy ficzer w template'cie
   po scaleniu `chore/agent-loop`.
2. **Ficzer: komentarze do todo itemu.** Nowa encja `TodoComment` z migracją, trzy tickety: dodanie
   komentarza, lista komentarzy itemu, usunięcie komentarza. Własność przez item: cudzy item odpowiada 404.
3. **Scenariusze (b)–(d) w jednym przebiegu, wieczorem 2026-10-29:**
   - **(b):** Docker zostaje wyłączony do startu `/ship`.
   - **(c):** po bramce 1, tuż przed `/ship`, commit na `main` sandboksu z `fiveHourPercent` równym bieżącemu
     zużyciu plus 4. Ticket 01 rusza, pauza wypada przed następnym. Próg wraca do 90% commitem na gałęzi
     `feat/<feature>` w worktree w trakcie pierwszego czekania, przed następnym `ship: start`, żeby nie
     wszedł w diff żadnego ticketu.
   - **(d):** ticket niemożliwy pisany ręcznie po bramce 1, nie przez grill, żeby `/plan-feature` nie
     „naprawił” go zmianą `protectedPaths`. Wymaga nowej reguły w `tests/ArchitectureTests`, jest niezależny
     i wchodzi jako 02, a tickety planu 02–03 przechodzą na 03–04. Oczekiwane: `STATUS: stuck` w rundzie 1,
     `needs-info`, revert, push, tickety 03–04 idą dalej. Potem użytkownik oznacza go `wontfix` commitem na
     gałęzi i wpisuje `/ship` ponownie: review gałęzi, brief i bramka 2, przy okazji test wznawiania.
   - Pomiar liczy się mimo pauz.
4. **Ticket 13 po pilocie.** `wontfix` omija lukę, a ponowne `ready-for-agent` to droga ticketu 13.
5. **Sprzątanie sandboksu:** usunięte worktree `toy-overdue` i `toy-quota`, lokalne gałęzie `feat/toy-*`
   i puste foldery `todo-open-count` i `toy-brief`. Gałęzie na gołym origin zostają. Archiwizacja starych
   sesji przebiegów tylko na osobne słowo użytkownika.
6. **`.agents/` zostaje nieśledzony.** Nie wpływa na pilota: nowy worktree go nie ma, a bramka 2 używa
   `--untracked-files=no`.

**2026-10-29, przebieg 1 pilota: `todo-comments`** (sandbox, plugin `1.3.1-dotnet.19`). Sesję `/ship`
uruchomiono z karty `/plan-feature` od razu po bramce 1, więc ticketu do scenariusza (d) ani obniżonego progu
do scenariusza (c) w nim nie było. Dla scenariusza (a), pomiaru i bramki 2 przebieg jest pełny.

- **Plan:** sesja na Opusie, grill i pakiet, bramka 1 „OK”. O 21:28 commit `plan: todo-comments` (`840888d`)
  ze spec (19 historii), trzema ticketami i `GLOSSARY.md` (termin **Comment**, poprawiony **Copy**). ADR nie
  powstał. Zużycie planowania razem z tą sesją: okno 5-godzinne z 2% do 15%, tydzień z 57% do 59%.
- **`/ship`:** sesja z karty na Sonnecie, tryb auto, Remote Control `on`. Wszystkie 210 tur szło na
  `claude-sonnet-5-5`.
  - **Start:** keep-awake, worktree `.claude/worktrees/todo-comments`, limit, środowisko i baseline zielone.
  - **01:** 2 rundy. Runda 1 odpadła, bo zabrakło dowodu czerwieni dla AC2, AC3, AC5 i AC8. Runda 2 dopisała
    dowody bez zmian w kodzie. Stash `ship: 01 round 2 leftovers` zawiera tylko różnice końców linii.
  - **02 i 03:** po 1 rundzie.
  - **Review gałęzi (Opus):** 2 uwagi w zakresie, obie naprawione w rundzie 1. Endpointy przeniesione do
    `Endpoints/TodoComments/`, encja do `Domain/Todos`. Uwaga o domain eventach została otwarta, a rozstrzyga
    ją propozycja 1. Nowych ticketów `needs-triage` brak.
  - **Testy na gałęzi:** ArchitectureTests 6, Application.UnitTests 85, IntegrationTests 31, HookTests 83,
    wszystkie zielone. Build: 0 ostrzeżeń.
- **Bramka 2:** o 22:12 odpowiedź „OK, 1-4 tak” z komputera. Zakres „1-4” został odczytany poprawnie.
  - Commit `ship: gate 2, conventions 1, 2, 3, 4` (`b38e40e`): propozycja 1 w `ca-review`, propozycje 2–4
    w `AGENTS.md`.
  - **Pierwsza ścieżka `git merge --ff-only` na żywo,** bo checkout stał na `main`. Każde polecenie szło
    osobno. Przed pushem sesja porównała `main` z końcem gałęzi, a potem wysłała dokładnie
    `git push origin main` (`b493a0f..b38e40e`).
  - **Sprzątanie:** `git worktree remove` się udał (w 07 kończył się `Permission denied`), gałąź usunął
    `update-ref -d` z jej końcem.
  - **CI:** `no CI for this remote`, zgodnie z decyzją 1.
  - **Build po commicie konwencji** nie był powtórzony. Commit zmienił tylko pliki `.md`.
- **Scenariusz (a):** w transkrypcie brak pytania o zgodę między bramkami, więc allowlista nie potrzebuje
  dopisków. Jedyna odmowa to polecenie złożone w worktree („too complex to verify”). Sesja sama je rozdzieliła.
- **Scenariusz (b) niesprawdzony:** Docker Desktop wystartował o 21:32:18, 40 s przed utworzeniem sesji
  `/ship` (20:33:00 UTC), więc `-Check environment` zastał go działającego. Uruchomił go użytkownik po
  komunikacie `/plan-feature` „Testy integracyjne potrzebują działającego Dockera” (ticket 14).
- **Push o bramce 2 nie przyszedł jako powiadomienie.** Narzędzie odpowiedziało „Mobile push requested”.
  Na telefonie wiadomość była widoczna dopiero po wejściu do aplikacji Claude, bez powiadomienia
  systemowego (relacja użytkownika 2026-10-29). Problem z 06 trwa (ticket 12). Kryterium pusha przy pytaniu
  o zgodę zostaje otwarte, bo żadne pytanie o zgodę nie padło.
- **Konwencja 4 przyjęta na fałszywym fakcie.** Brief twierdził, że wszystkie dotychczasowe walidatory są
  `internal sealed`. `CreateTodoCommandValidator` i `CopyTodoCommandValidator` są `public class`, a nowe
  `AddTodoCommentCommandValidator` i `DeleteTodoCommentCommandValidator` weszły do `main` także jako
  `public class`. `main` sandboksu łamie więc regułę przyjętą na tej samej bramce (ticket 15).
- **Pomiar nr 1** (`.scratch/todo-comments/metrics.md` w sandboksie):

  | Etap | Rundy | Minuty | Okno 5h | Tydzień |
  |---|---|---|---|---|
  | 01 | 2 | 12 | 15% → 22% | 59% → 60% |
  | 02 | 1 | 8 | 22% → 26% | 60% → 60% |
  | 03 | 1 | 7 | 26% → 30% | 60% → 61% |
  | review gałęzi | 1 | 7 | 30% → 37% | 61% → 62% |
  | **suma** | 5 | 34 | +22 | +3 |

  Ficzer z 3 ticketami kosztuje ok. 3% tygodnia, razem z planowaniem ok. 5%. Okno 5-godzinne mieści ok. 4
  takie ficzery. Reguła decyzji wymaga też pilota MAUI.

**Do zrobienia w tym tickecie: przebieg 2 dla scenariuszy (b)–(d).** Mały ficzer w sandboksie, 2 tickety
z planu. Przygotowanie według decyzji 3, tym razem w tej kolejności:

1. Docker zatrzymany i nieuruchamiany ręcznie, niezależnie od komunikatów `/plan-feature`.
2. Po bramce 1 karty `/ship` nie klikać.
3. Ticket niemożliwy jako 02 i przesunięcie ticketu planu, a także próg `fiveHourPercent`: osobne commity na
   `main` sandboksu, przed `/ship`.
4. `/ship` ręcznie w nowej sesji. Próg 90% wraca commitem na gałęzi w trakcie pierwszego czekania.

**2026-10-29 22:42, przebieg 2 przygotowany** (decyzja użytkownika: tickety ręcznie, bez `/plan-feature`, bo
tę ścieżkę sprawdził przebieg 1):

- **Ficzer `todo-comment-count`**, commit `48bf7eb` na `main` sandboksu:
  - 01: `commentCount` w `GET todos/{id}` z unieważnieniem cache przy dodaniu i usunięciu commentu;
  - 02: test architektury w `tests/ArchitectureTests` (ścieżka chroniona), niezależny, scenariusz (d);
  - 03: `commentCount` w `GET todos`, zależny od 01.
- **Próg** `fiveHourPercent` 49 przy zużyciu 45% (okno resetuje się o 23:50), commit `8fbd78b`.
- **Docker** zatrzymany przez `docker desktop stop` (exit 0, brak procesów Docker Desktop).
- **Oczekiwany przebieg:**
  - Docker startuje w `-Check environment`;
  - 01 kończy się `done`, potem pauza 5-godzinna do ok. 23:52 i próg 90% przywrócony commitem na gałęzi;
  - 02 `needs-info` w rundzie 1, 03 `done`;
  - koniec bez briefu, potem `wontfix` dla 02 i drugi `/ship`.

**2026-10-29/30, przebieg 2, część 1** (sesja `/ship` na Sonnecie, uruchomiona przez użytkownika o 22:46):

- **Scenariusz (b) przeszedł.** `-Check environment` zaczął się o 22:46:05, Docker Desktop wystartował
  o 22:46:08, wynik `ok`. Dockera uruchomiła pętla.
- **01:** `done` w 2 rundach w 9 minut, okno 5-godzinne z 47% do 52%, tydzień z 63% do 64%. Runda 1 znowu
  odpadła na brakującym dowodzie czerwieni (AC3), tak jak 01 przebiegu 1. Wygląda to na wzorzec rundy 1.
- **Scenariusz (c), pierwsza połowa przeszła.** Sprawdzenie przed 02: `fiveHour` 52 ≥ 49, więc commit
  `ship: pause, 5-hour limit`, wiersz pauzy „5-hour until 2026-10-29 23:50” i od 22:56:57
  `wait-for-reset.ps1` w tle, bez pusha. Próg 90% przywrócony commitem `472370b` na gałęzi o 23:07.
- **Wznowienie się nie odbyło.** O 23:47:47 Windows Update zrestartował komputer („Operating System: Upgrade
  (Planned)”), a o 23:48:27 go wyłączył, 3 minuty przed końcem czekania. Komputer wstał o 08:22. Sesja
  i skrypt zginęły bez pusha. Tracker był spójny: 01 `done`, 02 i 03 `ready-for-agent`, wiersz pauzy
  (ticket 16). Wznowienie bez udziału użytkownika pozostaje niesprawdzone.
- **Część 2:** ponowny `/ship todo-comment-count` w checkoucie głównym o 2026-10-30 rano. Sprawdza
  wznowienie po przerwanym przebiegu i drugi raz scenariusz (b), bo Docker po restarcie nie działa.
  Limit na starcie: okno 5-godzinne 11%, tydzień 2% (reset tygodniowy 2026-10-30 07:00).
