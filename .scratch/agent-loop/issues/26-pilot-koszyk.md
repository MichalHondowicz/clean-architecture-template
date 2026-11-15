# 26: Pilot 2: mały ficzer Koszyka przez pętlę

**What to build:** Mały ficzer full-stack z ekranem przechodzi w Koszyku `/plan-feature`, bramkę 1, `/ship`
i bramkę 2 aż do `main` na GitHubie. To pilot 2 z pomiarem nr 2 i pierwsze sprawdzenie pętli na MAUI.

**Blocked by:** 20, 23, 24, 25

**Seams:** seam 1 ze speca: statusy ticketów, `## Comments`, `metrics.md`, brief, historia gałęzi,
powiadomienia, CI po scaleniu.

**Status:** ready-for-human

**Skąd:** ticket 11, decyzje 1 i 8 z bramki 2026-11-04 (historie 54, 58, 63). Zakres pilota: dwie lub trzy
pozycje „kosmetyki UX” z modułu 15 (`D:\Projects\Koszyk\docs\tasks\state-15-mobile-polish-hardening.md`),
np. przecinek w polu `Numeric` i podwójny alert „Brak dostępu”.

- [ ] AC1: Ficzer dochodzi do `main` bez udziału użytkownika poza bramkami. Każde pytanie o zgodę trafia do
      allowlisty Koszyka.
- [ ] AC2: Brief wymienia zmienione ekrany i pliki do sprawdzenia na emulatorze (`screenPaths`), a użytkownik
      sprawdza je na emulatorze przed `OK`.
- [x] AC3: Tabela pomiaru jest pełna i zapisana jako pomiar nr 2 (historia 47), obok pomiaru nr 1 z ticketu 10.
- [ ] AC4: Obserwacje z ticketu 19 (wznowienie bez użytkownika, push przy pytaniu o zgodę) są zapisane, jeśli
      przebieg na nie trafi.
- [ ] AC5: Rozjazdy pętli i dokumentów Koszyka są zapisane jako tickety `needs-triage`.

**2026-11-05, przebieg 1 (`ux-cosmetics-1`), zapis agenta z repozytorium Koszyka, bez raportu sesji `/ship`:**

- Plan: commit `e78d1f6` w Koszyku. `/ship` 2026-11-04 22:02–22:33: tickety 01–03 `done` (rundy 1, 2, 1), runda
  gałęzi 1 (nazwy testów ticketu 03), brief, decyzja „OK, 1 tak” (`b4b7b04`; konwencja `System.TimeProvider`
  w `AGENTS.md`). Testy 99/5/130/611, 0 ostrzeżeń. Nowe `needs-triage`: 04 (e-mail „Ty” po synchronizacji, DTO
  i API), 05 (nazwy testów `SuggestionsViewModelTests`).
- `metrics.md` w Koszyku (`.scratch/ux-cosmetics-1/metrics.md`): 26 minut, 5h +19 pp (44% → 63%), tydzień +2 pp
  (58% → 60%).
- Po „OK” `main` nie został przesunięty: Koszyk nie ma remote (ticket 20), więc krok scalenia z `git fetch origin`
  i `git push origin main` nie mógł przejść. 2026-11-05 na słowo użytkownika w tej sesji: `main` przesunięty
  fast-forwardem na `b4b7b04`, worktree wyrejestrowany i gałąź `feat/ux-cosmetics-1` usunięta. Pusty folder
  `.claude/worktrees/ux-cosmetics-1` zostaje do zamknięcia sesji `/ship` (blokada katalogu). Bez pushu.
- Nie sprawdzone: ekrany `ListsPage` i `MembersPage` na emulatorze (AC2), pytania o zgodę i wznowienie (AC4).
  Ticket zostaje `ready-for-human`: AC1 wymaga przebiegu z remote, AC3 wpisu pomiaru nr 2.

**2026-11-05, pomiar nr 2** (`D:\Projects\Koszyk\.scratch\ux-cosmetics-1\metrics.md`, na `main` Koszyka `b4b7b04`):

| Etap | Rundy | Minuty | Okno 5h | Tydzień |
|---|---|---|---|---|
| 01 | 1 | 5 | 44% → 48% | 58% → 59% |
| 02 | 2 | 9 | 48% → 53% | 59% → 59% |
| 03 | 1 | 6 | 53% → 57% | 59% → 60% |
| review gałęzi | 1 | 6 | 57% → 63% | 60% → 60% |
| **suma** | 5 | 26 | +19 | +2 |

Modele: `/ship` Sonnet, implementer Sonnet, weryfikator Haiku, recenzent ticketu Sonnet, review gałęzi Opus
(Standards, Spec i re-review). Wynik: 3 `done`, 0 `needs-info`.

Obok pomiaru nr 1 (ticket 10, `todo-comments`, backend w sandboksie: 5 rund, 34 minuty, 5h +22, tydzień +3):
ficzer MAUI z 3 ticketami w Koszyku kosztował mniej, choć jego DoD buduje dwie solucje (czas DoD zmierzony
przed pilotem w tickecie 23: 49,5 s przy 587 testach mobilnych; po pilocie testów mobilnych jest 611, czasu
w pilocie nikt nie mierzył). Różnica mieści się w zaokrągleniu procentów tygodnia do pełnych punktów, więc
oba pomiary mówią to samo: ok. 2–3% tygodnia na ficzer z 3 ticketami. Czego tabela nie obejmuje: planowania
(`/plan-feature` na Opusie; w pomiarze nr 1 zmierzone na ok. 2% tygodnia, tydzień z 57% do 59%; planowania
ficzera MAUI nikt nie zmierzył) i ewentualnej równoległej pracy w innych sesjach w tym samym oknie. Wniosek
do reguły z historii 48 (`docs/workflow.md`, „Reguła decyzji po pilotach”) należy do decyzji z ticketu 11:
przy ok. 5% tygodnia na ficzer razem z planem (plan z pomiaru nr 1) próg 2 ficzerów tygodniowo jest daleko.

**2026-11-05, rozjazdy pętli z przeglądu stanu (AC5):** trzy luki `/ship` (CI bez workflow, otwarte tickety
scalonego ficzera, niewidoczny brak tematu ntfy) i błąd Git Bash przy wznowieniu po scaleniu poprawione od
razu w tickecie 31 (plugin `1.3.1-dotnet.30`). Rozjazdy Koszyka z przebiegu 1 to jego tickety 04 i 05,
po triage'u 04 `ready-for-agent`, 05 `wontfix` (Koszyk `fbc74b3`). AC5 zostaje otwarte do końca pilota.

**2026-11-15, przebiegi 2 i 3 z historii Koszyka** (zapis agenta z repozytorium Koszyka, `main` `b2d0ea8`, bez
raportów sesji `/ship`; źródła: `git log`, reflog `origin/main`, `.scratch/<slug>/metrics.md` i `brief.md`,
`docs/tasks/handoff-15-release-e2e-and-triage.md`):

- **Przebieg 2, `emulator-tls-release-e2e`** (pierwszy z remote). Plan `0b180ef` 2026-11-10 21:36, `/ship`
  21:39–22:32: tickety 01–03 `done` po 1 rundzie, review gałęzi 2 rundy, brief. Bramka 2 „OK, 1 tak” 2026-11-11
  15:07 (`c69ede1`), `origin/main` przesunięty pushem o 15:08:20. Testy 99/5/136/14/622, 0 ostrzeżeń. Nowe
  tickety Koszyka: 04, 05.
- **Przebieg 3, `mobile-smoke-emulator`.** Plan `b0fe56f` 2026-11-11 16:00, `/ship` od 16:36: 01 i 02 `done` po
  1 rundzie, pauza na limit 5-godzinny 16:51 (do 20:40), 03 `done` po 2 rundach (start 21:13, koniec 2026-11-12
  16:52, „wznowiony po pauzie 5h i po błędzie limitu implementera”), review gałęzi 1 runda, brief 17:06. Bramka 2
  „OK, 1 tak” 2026-11-12 20:44 (`626b1e5`), `origin/main` przesunięty pushem o 20:45:25. Testy 99/5/136/14/647,
  smoke `mobile-login` zielony. Nowe tickety Koszyka: 04–07.

| Przebieg | Rundy | Minuty | Okno 5h | Tydzień |
|---|---|---|---|---|
| 2 `emulator-tls-release-e2e` | 5 | 51 | 24% → 56% (+32) | 21% → 25% (+4) |
| 3 `mobile-smoke-emulator`, 01 i 02 | 2 | 14 | 34% → 90% (+56) | 5% → 14% (+9) |
| 3, ticket 03 (z pauzą i czekaniem) | 2 | 1179 | 47% → 17% | 22% → 48% |
| 3, review gałęzi | 1 | 12 | 17% → 27% (+10) | 48% → 49% (+1) |

Modele jak w pomiarze nr 2. Wiersz ticketu 03 obejmuje 19 godzin zegara i dwa okna 5-godzinne, a tydzień między
pauzą a startem 03 urósł z 14% do 22% bez pracy `/ship`, więc była w nim praca innych sesji; ten wiersz nie mierzy
kosztu ticketu. Suma w `metrics.md` przebiegu 3 („+66%*”, „+36%”) ma gwiazdkę bez objaśnienia w pliku. To, co da
się porównać: przebieg 2 kosztował 4 pp tygodnia, a same tickety 01 i 02 przebiegu 3 kosztowały 9 pp w 14 minut.
Oba wyniki są wyższe niż 2–3 pp z pomiarów nr 1 i 2. Próg z historii 48 (mniej niż 2 ficzery tygodniowo) dalej
jest daleko, więc decyzja z ticketu 11 (zostaje Pro) się nie zmienia, ale liczba „ok. 5% na ficzer z planem”
jest dolną granicą, nie średnią.

Co te przebiegi mówią o kryteriach:

- **AC1.** W obu przebiegach `origin/main` przesunął się pushem ok. 20 s po commicie bramki 2, czyli scalenie
  i push zrobił krok `/ship`, nie użytkownik. Przebieg 2 doszedł do `main` bez udziału użytkownika poza bramkami.
  Przebieg 3 nie: po restarcie systemu w trakcie czekania sesja nie ruszyła sama (ticket 19, wpis z 2026-11-11).
  Druga część kryterium (pytania o zgodę trafiają do allowlisty) nie ma zapisu: z repozytorium nie widać, czy
  w tych przebiegach padło jakiekolwiek pytanie o zgodę.
- **AC2.** Oba briefy mają sekcję „Screens” ze zmienionymi plikami (`screenPaths`): 4 pliki głowy MAUI
  w przebiegu 2, `MauiProgram.cs` i 8 plików XAML w przebiegu 3; brief przebiegu 3 ma też „Check before OK”.
  Sprawdzenia na emulatorze przed `OK` nie było: brief przebiegu 2 mówi „E2E na emulatorach jeszcze nie robiony”,
  a handoff Koszyka z 2026-11-13 wymienia jako zaległe obejrzenia `ux-cosmetics-1` (`ListsPage` jako Widz,
  `MembersPage`, wiersz „Ty”) i wszystkie ekrany po przebudowie `MauiProgram`. Zostają do przebiegu E2E Release
  z tego handoffu.
- **AC4.** Wznowienie: zapisane w tickecie 19 (pauza zadziałała, restart o 20:25 przerwał czekanie, 0 tur).
  Push przy pytaniu o zgodę: brak obserwacji, jak w AC1.
- **AC5.** Rozjazdy pętli z tych przebiegów są zapisane i poprawione: ticket 32 (krok CI przy przeglądarce
  wylogowanej z GitHuba), ticket 33 (z briefu przebiegu 2: nic nie sprawdzone na emulatorze, wyciszenie
  analizatora obejściem przez refleksję; stąd smoke w DoD i `allowedSuppressions`), poprawka forka `.35`
  (komunikat o przerwanym czekaniu). Rozjazdy Koszyka: 6 ticketów po triage z 2026-11-13 (Koszyk `166458a`):
  TLS 04 `ready-for-agent`, TLS 05 `ready-for-human`, smoke 04–07 `ready-for-agent`.
- **Seam „CI po scaleniu”.** Żaden z trzech przebiegów go nie sprawdził: workflow CI Koszyka powstał 2026-11-13
  (`b2d0ea8`, ticket 24), po ostatnim scaleniu przez `/ship`.

Ticket zostaje `ready-for-human`. Do zamknięcia brakuje: obejrzenia ekranów na emulatorze (AC2), obserwacji
pytania o zgodę (AC1, AC4) i decyzji użytkownika, czy AC1 uznać za spełnione przebiegiem 2.
