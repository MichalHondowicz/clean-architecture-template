# Spec: pętla ficzera bez nadzoru i warstwa MAUI

**Status:** ready-for-agent

## Problem Statement

Template ma być silnikiem do pisania aplikacji C#/.NET MAUI z agentami, ale dziś każdy ficzer wymaga mojej
obecności przez cały czas. Przepływ z `docs/workflow.md` ma około ośmiu ręcznych kroków: osobne komendy
`/grill-with-docs`, `/to-spec`, `/to-tickets`, `/triage` i `/implement` dla każdego ticketu (wszystkie tylko
dla człowieka), zatwierdzanie seamów przed pierwszym testem, zgody w trakcie (np. nagłówek migracji),
zgodę na scalenie, ręczny push i ręczne uruchamianie Dockera. `implement` przechodzi raz: robi, recenzuje
i commituje, ale nie wraca do uwag z review. DoD to proza z checkboxami, więc nic nie sprawdza go
maszynowo, a agent może sam ogłosić sukces.

Wszystkie role działają na modelu sesji, a mam plan Pro: płacę limitem (okno 5-godzinne i tygodniowy), nie
złotówkami za token. Pętla bez nadzoru na Pro dojdzie do limitu i dziś po prostu stanie. Nie wiem też, ile
limitu kosztuje jeden ficzer, więc nie umiem ocenić, czy opłaca się Max 5x albo tańszy zewnętrzny model.

Template nie ma żadnej warstwy MAUI, choć moje aplikacje (Gym, Koszyk) to monorepo z backendem i klientem
mobilnym. To trzecie podejście: wcześniejszy auto-run w pluginie `dotnet-workflow` (własny runner ze stanem
w pliku) został wstrzymany, a większość jego historii to naprawy samego runnera.

## Solution

Ficzer ma dwie bramki i nic pomiędzy nimi. Na pierwszej zatwierdzam pakiet planu, który powstaje w jednej
sesji z `/plan-feature`: grill, spec i tickety z seamami, kryteriami z ID i DoD. Potem wpisuję
`/ship <feature>` i odchodzę. Pętla działa lokalnie, w sesji w tle aplikacji desktopowej, w osobnym
worktree. Robi tickety po kolei: implementer (Sonnet) pracuje TDD, osobny weryfikator (skrypt i Haiku)
sprawdza DoD, review Spec (Sonnet) porównuje z ticketem, a uwagi wracają do implementera najwyżej przez
3 rundy (trzecia na Opusie). Ticket, który nie zbiega, dostaje `needs-info` i brief, a niezależne tickety
idą dalej. Na końcu Opus raz robi `ca-review` i review Spec całej gałęzi, a ja dostaję push na telefon
i brief. Na drugiej bramce odpowiadam „OK” (także z telefonu), a agent scala do `main`, wypycha, sprząta
i sprawdza CI.

Pętla pilnuje limitu: nie zaczyna ticketu powyżej progów, kończy sesję w spójnym stanie i wznawia się
po resecie przez lokalne zadanie zaplanowane. Dla każdego ticketu zapisuje zużycie limitu, więc po dwóch
pilotach decyzja o Max 5x albo zewnętrznym implementerze wynika z liczb i reguły ustalonej z góry.
Implementer ma spisany kontrakt egzekwowany hookami, więc podmiana modelu to zmiana jednej definicji.

Template dostaje warstwę mobilną według wzorca Koszyka (`Mobile`, `Mobile.Core`, `Contracts`), skill
`add-screen` i DoD mobilny bez emulatora. Wdrożenie idzie etapami: najpierw pętla na samym backendzie
i pilot, potem MAUI i drugi pilot.

## User Stories

### Planowanie

1. Jako właściciel projektu chcę uruchomić planowanie ficzera jedną komendą `/plan-feature`, która w jednej sesji przeprowadzi grill, spisze spec i rozbije go na tickety, żeby nie wpisywać czterech komend po kolei.
2. Jako właściciel projektu chcę zatwierdzić cały pakiet planu (spec, tickety, seamy, kryteria, DoD) jedną decyzją, żeby od tej chwili agent o nic mnie nie pytał.
3. Jako właściciel projektu chcę, żeby każdy ticket miał sekcję `Seams:` wyprowadzoną z ADR 0004, żeby pętla nie zatrzymywała się na zatwierdzaniu seamów.
4. Jako właściciel projektu chcę, żeby każde kryterium akceptacji miało ID (`AC1`, `AC2`…) i planowaną nazwę testu, żeby dało się maszynowo sprawdzić, że kryterium jest udowodnione.
5. Jako właściciel projektu chcę, żeby każda decyzja, o którą agent dotąd pytał w trakcie i która się powtarza, była regułą w `AGENTS.md` albo w skillu, żeby pętla nie miała powodu pytać.
6. Jako właściciel projektu chcę, żeby planowanie szło na Opusie, żeby najlepsza ocena była tam, gdzie decyzje ważą najwięcej, a tokenów jest mało.
7. Jako właściciel projektu chcę, żeby po zatwierdzeniu planu agent zaproponował start pętli, żeby przejście do realizacji było jednym krokiem.

### Uruchomienie

8. Jako właściciel projektu chcę uruchomić realizację komendą `/ship <feature>` i odejść od komputera, żeby nie siedzieć przy ekranie.
9. Jako właściciel projektu chcę, żeby pętla działała lokalnie w sesji w tle aplikacji desktopowej, w osobnym worktree ficzera, żeby mój główny checkout był wolny do mojej pracy.
10. Jako właściciel projektu chcę, żeby komputer nie usypiał, dopóki pętla pracuje, żeby noc nie przepadła.
11. Jako właściciel projektu chcę, żeby pętla przed startem sprawdziła Dockera i uruchomiła Docker Desktop, gdy nie działa, żeby testy integracyjne nie zatrzymały pracy.
12. Jako właściciel projektu chcę, żeby pętla działała w trybie auto z rozszerzoną allowlistą i nigdy w `bypassPermissions`, żeby nie przerywały jej pytania o zgodę, a zabezpieczenia zostały.
13. Jako właściciel projektu chcę, żeby start i wznowienie pętli były tą samą operacją, która czyta stan z trackera, żeby przerwana sesja nie traciła postępu.

### Realizacja ticketu

14. Jako właściciel projektu chcę, żeby tickety szły po kolei, w kolejności zależności, na jednej gałęzi ficzera, żeby nie płacić limitem za równoległość i nie potrzebować agenta scalającego.
15. Jako właściciel projektu chcę, żeby implementer pracował TDD na seamach z ticketu i traktował je jako uzgodnione, żeby testy lądowały tam, gdzie zdecydowałem.
16. Jako właściciel projektu chcę, żeby implementer działał na Sonnecie, żeby większość tokenów szła na tańszy model.
17. Jako właściciel projektu chcę, żeby implementer miał spisany kontrakt (wejście, wyjście, zakazy), żeby dało się go później podmienić na inny model bez przeprojektowania pętli.
18. Jako właściciel projektu chcę, żeby implementer nie mógł zmieniać chronionych ścieżek (konfiguracji analizatorów, testów architektury, `AGENTS.md`, ADR-ów, konfiguracji agenta), żeby nie „zbiegał” przez osłabienie reguł.
19. Jako właściciel projektu chcę, żeby zmiana chronionej ścieżki dowolnym narzędziem, także przez Bash, była wykryta przy weryfikacji, żeby zakaz nie zależał od narzędzia, którego użył agent.
20. Jako właściciel projektu chcę, żeby implementer zapisał w tickecie dla każdego `ACn` nazwę testu i dowód, że test był czerwony (red w cyklu albo red proof), żeby weryfikator miał co sprawdzić.

### DoD i weryfikacja

21. Jako właściciel projektu chcę, żeby ticket nie mógł zostać ogłoszony gotowym, dopóki build ma błąd lub ostrzeżenie albo testy są czerwone, żeby agent nie ogłaszał sukcesu sam sobie.
22. Jako właściciel projektu chcę, żeby DoD sprawdzał osobny weryfikator, a nie implementer, żeby sprawdzenie było niezależne od tego, kto pisał kod.
23. Jako właściciel projektu chcę, żeby mechaniczną część DoD (build, testy) wykonywał skrypt, a przypisanie kryteriów do testów Haiku, żeby weryfikacja prawie nie zużywała limitu.
24. Jako właściciel projektu chcę, żeby weryfikator oznaczał każdą zmianę istniejącego testu, który nie należy do ticketu, żeby wychwycić osłabianie testów.
25. Jako właściciel projektu chcę, żeby każdy ticket przeszedł review na osi Spec (Sonnet), żeby wyłapać rozjazd z tym, o co prosiłem.
26. Jako właściciel projektu chcę, żeby pętla sama poprawiała uwagi z review w zakresie ticketu, najwyżej w 3 rundach, żeby nie palić limitu na beznadziejne próby.
27. Jako właściciel projektu chcę, żeby trzecia runda poprawek szła na Opusie, żeby trudny przypadek dostał mocniejszy model, zanim pętla się podda.
28. Jako właściciel projektu chcę, żeby po wyczerpaniu rund ticket dostał `needs-info` i brief (co próbowano, gdzie stoi, czego potrzeba), żeby rano od razu wiedzieć, co zdecydować.
29. Jako właściciel projektu chcę, żeby tickety zależne od zablokowanego czekały, a niezależne szły dalej, żeby jeden problem nie zatrzymał całej nocy.
30. Jako właściciel projektu chcę, żeby uwagi z review spoza zakresu ticketu trafiały do backlogu jako `needs-triage` i niczego nie blokowały, żeby pętla nie rozszerzała zakresu.
31. Jako właściciel projektu chcę, żeby awaria środowiska (Docker nie działa, build nie startuje) zatrzymała cały ficzer i mnie powiadomiła, żeby pętla nie kręciła się na zepsutym środowisku.
32. Jako właściciel projektu chcę, żeby pętla nigdy nie osłabiała testów ani nie wyciszała analizatorów, żeby zielony znaczył zielony.

### Koniec ficzera i bramka 2

33. Jako właściciel projektu chcę, żeby przed bramką 2 Opus raz zrobił `ca-review` i review Spec całej gałęzi, żeby ostatnia linia obrony była najmocniejsza.
34. Jako właściciel projektu chcę dostać brief gotowości (co zrobione, liczby testów, uwagi z review, rzeczy niepewne, zmienione ekrany, propozycje konwencji), żeby zdecydować o scaleniu w minutę.
35. Jako właściciel projektu chcę zatwierdzić scalenie jedną wiadomością, także z telefonu przez Remote Control, żeby nie musieć siadać do komputera.
36. Jako właściciel projektu chcę, żeby po moim „OK” agent zrobił fast-forward do `main`, wypchnął zmiany, usunął gałąź i worktree i sprawdził status CI, żeby nie robić tego ręcznie.
37. Jako właściciel projektu chcę, żeby allowlista dopuszczała dokładnie `git push origin main`, bez wzorca z gwiazdką, żeby agent nie mógł wypchnąć innej gałęzi ani użyć `--force`.
38. Jako właściciel projektu chcę decydować o każdej propozycji konwencji z briefu osobno, niezależnie od zgody na scalenie, żeby konwencje zmieniały się tylko moją decyzją.
39. Jako właściciel projektu chcę, żeby przyjęte propozycje konwencji agent wprowadził osobnym commitem, a odrzucone wypadły, żeby historia pokazywała, skąd wzięła się reguła.

### Limity

40. Jako właściciel projektu chcę, żeby pętla nie zaczynała nowego ticketu przy zużyciu limitu tygodniowego ≥ 80% albo okna 5-godzinnego ≥ 90%, żeby zostawić sobie limit na planowanie następnego ficzera.
41. Jako właściciel projektu chcę, żeby po dojściu do progu pętla zakończyła sesję w spójnym stanie i zaplanowała wznowienie na godzinę resetu, żeby praca ruszyła beze mnie.
42. Jako właściciel projektu chcę, żeby progi były w pliku DoD projektu, żeby zmienić je bez zmiany skilli i zasymulować limit w pilocie.

### Powiadomienia

43. Jako właściciel projektu chcę dostać push na telefon, gdy ficzer jest gotowy do bramki 2, ticket utknął, środowisko padło albo pętla czeka na reset tygodniowy, żeby wiedzieć o tym bez patrzenia w ekran.
44. Jako właściciel projektu chcę, żeby push o resecie tygodniowym przyszedł raz, a nie przy każdym oknie 5-godzinnym, żeby nie nauczyć się ignorować powiadomień.
45. Jako właściciel projektu chcę, żeby push zawierał jedno zdanie i wskazanie briefu, a pełny brief leżał w katalogu ficzera w trackerze, żeby treść była wersjonowana razem z ficzerem.
46. Jako właściciel projektu chcę dostać push, gdy sesja w tle czeka na zgodę, żeby jedno pytanie nie zabrało całej nocy.

### Pomiar

47. Jako właściciel projektu chcę, żeby pętla zapisywała dla każdego ticketu zużycie limitów na starcie i na końcu, liczbę rund review, model w każdej roli, czas i wynik, a na końcu ficzera sumę, żeby decyzja po pilotach opierała się na liczbach.
48. Jako właściciel projektu chcę reguły ustalonej z góry (mniej niż 2 ficzery po 3 tickety w tygodniowym limicie Pro oznacza Max 5x albo zewnętrzny implementer), żeby decyzja była mechaniczna.

### Warstwa MAUI

49. Jako właściciel projektu chcę, żeby template był monorepo z warstwą mobilną (`Mobile`, `Mobile.Core`, `Contracts`) według wzorca Koszyka, żeby nowa aplikacja startowała z backendem i klientem w jednej solucji.
50. Jako właściciel projektu chcę, żeby ViewModele, serwisy i klient API były w `Mobile.Core` bez zależności od MAUI, żeby dało się je testować bez urządzenia.
51. Jako właściciel projektu chcę, żeby DTO były współdzielone przez `Contracts` między Web.Api a klientem, żeby kontrakt API nie rozjeżdżał się po obu stronach.
52. Jako właściciel projektu chcę skilla `add-screen` (ViewModel, strona, typowany klient HTTP, rejestracja w DI, testy `Mobile.Core`), żeby agent dodawał ekrany według jednej konwencji.
53. Jako właściciel projektu chcę, żeby DoD ticketu mobilnego obejmował testy `Mobile.Core` i build głowy `net10.0-android` ze skompilowanymi bindingami, żeby błąd w XAML psuł build.
54. Jako właściciel projektu chcę, żeby brief wymieniał zmienione ekrany, żeby ocenić UI wzrokiem bez automatyzacji UI.
55. Jako właściciel projektu chcę, żeby konwencje mobilne były w `AGENTS.md` obok backendowych, żeby agent miał jedno źródło reguł.
56. Jako właściciel projektu chcę, żeby CI budował też głowę Androida, żeby plik, który nie trafił do commita, nie przeszedł niezauważony.

### Wdrożenie

57. Jako właściciel projektu chcę wdrożyć pętlę najpierw na samym backendzie i sprawdzić ją pilotem, żeby wiedzieć, że ewentualna porażka to pętla, a nie MAUI.
58. Jako właściciel projektu chcę, żeby drugi pilot był ficzerem full-stack z ekranem, żeby dał drugi pomiar i sprawdził warstwę mobilną.
59. Jako właściciel projektu chcę, żeby proces ogólny był w forku skilli, a konwencje i DoD w template'cie, żeby ten sam fork obsłużył inny projekt po dodaniu pliku DoD.
60. Jako właściciel projektu chcę usunąć starą kopię `.agents/skills/`, żeby żadne narzędzie nie czytało nieaktualnych reguł.
61. Jako właściciel projektu chcę, żeby `docs/workflow.md` opisywał nowy przepływ z dwiema bramkami, żeby mój przewodnik zgadzał się z rzeczywistością.
62. Jako właściciel projektu chcę, żeby `implement` i `implement-spec` zostały bez zmian do pracy przy komputerze, żeby tryb interaktywny dalej działał.

## Implementation Decisions

### Podział odpowiedzialności

- **Fork skilli** (proces, globalny): `/plan-feature`, `/ship`, definicje ról z polem `model:`, logika
  weryfikatora, zmiana w `tdd`. Oba nowe skille są tylko dla człowieka (`disable-model-invocation`), tak jak
  reszta łańcucha, żeby nic nie uruchomiło się samo.
- **Template** (per projekt): plik DoD projektu, skrypty hooków, dokument kontraktu implementera,
  rozszerzenie allowlisty, `add-screen`, sekcja mobilna w `AGENTS.md`, projekty MAUI, zmiany CI,
  aktualizacja `docs/workflow.md`.
- Pętla składa się z elementów Claude Code: skilla orkiestratora, sub-agentów z modelem per rola, hooków
  i plików trackera jako jedynego stanu. Nie powstaje własny runner ani osobny plik stanu.

### `/plan-feature`

- Łączy w jednej sesji grill (z zapisem do `GLOSSARY.md` i ADR-ów), spec i tickety. Działa na Opusie.
- Każdy ticket ma, oprócz pól z `to-tickets`: sekcję `Seams:` wyprowadzoną z ADR 0004 (najpierw HTTP,
  handler dla ścieżek porażki i czasu, walidator per reguła; dla ticketu mobilnego `Mobile.Core`),
  kryteria z ID `AC1…n` i planowaną nazwę testu przy każdym kryterium.
- Kończy się jedną decyzją o całym pakiecie. Po zatwierdzeniu tickety mają `ready-for-agent`, a agent
  proponuje uruchomienie `/ship`.

### `/ship <feature>`

- Zastępuje `implement-spec` w trybie bez nadzoru. `implement` i `implement-spec` zostają bez zmian.
- Działa w sesji w tle aplikacji desktopowej, w trybie auto, na Sonnecie, w jednym worktree na gałęzi
  `feat/<slug>`. Utrzymuje komputer w stanie czuwania.
- Start i wznowienie to ta sama operacja: stan to statusy ticketów i ich `## Comments`. Dochodzi status
  roboczy `in-progress`, poza rolami triage, tak jak `done`.
- Pre-flight: Docker działa albo zostaje uruchomiony, worktree jest czysty, limity są pod progami.
  Niepowodzenie Dockera lub buildu bazowego to awaria środowiska: stop ficzera i powiadomienie.
- Kolejność: frontier ticketów po kolei, bez równoległości i bez agenta scalającego.
- Cykl ticketu: sprawdzenie limitów → implementer → weryfikator DoD → review Spec ticketu → przy uwagach
  następna runda implementera z plikiem uwag. Najwyżej 3 rundy, trzecia z implementerem na Opusie.
  Po wyczerpaniu: `needs-info`, brief ticketu, tickety zależne czekają, niezależne idą dalej.
- Uwagi z review spoza zakresu ticketu: nowy ticket `needs-triage` w backlogu ficzera, bez blokowania.
- Po ostatnim tickecie: `ca-review` i review Spec całej gałęzi na Opusie. Uwagi w zakresie przechodzą tę
  samą regułę 3 rund. Nierozwiązane trafiają do briefu jako otwarte.
- Bramka 2: brief w katalogu ficzera w trackerze, push, czekanie na odpowiedź w tej samej sesji (także przez
  Remote Control). Po „OK”: fast-forward do `main`, `git push origin main`, usunięcie gałęzi i worktree,
  sprawdzenie statusu CI w przeglądarce, raport. Propozycje konwencji z briefu: decyzja per propozycja,
  przyjęte w osobnym commicie orkiestratora (strażnik działa tylko w sub-agencie implementera).

### Role i modele

| Rola | Model |
|---|---|
| Planowanie (`/plan-feature`) | Opus |
| Orkiestrator (`/ship`) | Sonnet |
| Implementer, rundy 1–2 | Sonnet |
| Implementer, runda 3 | Opus |
| Weryfikator DoD | skrypt + Haiku |
| Review Spec ticketu | Sonnet |
| `ca-review` i review Spec gałęzi | Opus |

### Kontrakt implementera

- Spisany w dokumentacji agentów template'u.
- **Wejście:** ścieżka ticketu (z `Seams:` i `AC1…n`), worktree, gałąź, numer rundy, plik uwag
  z poprzedniej rundy.
- **Wyjście:** commity na gałęzi; wpis w `## Comments` ticketu z nazwą testu i dowodem czerwieni dla
  każdego `ACn`; status `done` albo `stuck` z krótkim opisem.
- **Zakazy:** chronione ścieżki z pliku DoD; na start `.editorconfig`, `Directory.Build.props`, testy
  architektury, `AGENTS.md`, ADR-y, konfiguracja agenta. Zakaz osłabiania testów i wyciszania analizatorów
  obowiązuje bez zmian z `AGENTS.md`.
- Kontrakt jest niezależny od modelu: przyszły zewnętrzny implementer dostaje ten sam kontrakt i te same
  hooki.

### Zmiana w `tdd`

- Seamy zapisane w tickecie zatwierdzonym na bramce 1 są uzgodnione. W tym przypadku skill nie pyta
  o seamy przed pierwszym testem. Bez ticketu zachowanie zostaje jak dziś.

### Plik DoD projektu

- Jeden plik w dokumentacji agentów template'u, czytany przez hooki i weryfikatora: komenda buildu,
  komenda pełnych testów, komenda buildu głowy mobilnej (od etapu 2), wymaganie Dockera, lista chronionych
  ścieżek, progi limitu (tygodniowy 80%, okno 5-godzinne 90%).
- Fork nie zna komend projektu. Inny projekt dostaje pętlę po dodaniu tego pliku.

### Hooki

- Pisane w PowerShell 7 (`pwsh`), dostępnym lokalnie i na runnerach Ubuntu w CI.
- Rejestruje je template w ustawieniach projektu, nie fork: Claude Code ignoruje pole `hooks` we
  frontmatterze agentów z pluginu, a hooki samego pluginu działałyby w każdym projekcie.
- Działają tylko w sub-agencie implementera: zdarzenia sub-agenta niosą pole `agent_type`, a nazwa roli
  implementera z forka jest częścią kontraktu. Inne role (weryfikator, review) i sesja orkiestratora nie są
  blokowane, więc wprowadzenie przyjętych konwencji po bramce 2 jest możliwe.
- **`SubagentStop`:** uruchamia komendy DoD i blokuje zakończenie implementera przy błędzie lub
  ostrzeżeniu buildu albo czerwonych testach, z treścią porażki. Blokuje także przy powtórnej próbie
  (`stop_hook_active`). Pętlę bez wyjścia ogranicza limit tur implementera (`maxTurns`).
- **`PreToolUse` (Edit, Write):** odrzuca zapis do chronionej ścieżki.
- Rozstrzygające sprawdzenie robi weryfikator na diffie zakresu commitów ticketu: zmiany w chronionych
  ścieżkach (zapis przez Bash jest niewidoczny dla `PreToolUse`) i dodane wyciszenia analizatorów.

### Weryfikator DoD

- Skrypt: build 0 błędów i 0 ostrzeżeń, pełne testy zielone (z Dockerem), od etapu 2 build głowy
  `net10.0-android`, brak zmian w chronionych ścieżkach i brak dodanych wyciszeń w diffie ticketu.
- Haiku: każde `ACn` ma w `## Comments` test, który istnieje, przechodzi i ma odnotowany dowód czerwieni;
  zmiany w istniejących testach spoza ticketu są oznaczone.

### Limity i wznawianie

- Przed każdym ticketem odczyt zużycia planu. Powyżej progu: zapis stanu w tickecie, lokalne zadanie
  zaplanowane na godzinę resetu, które wznawia `/ship <feature>`, i czyste zakończenie sesji.
- Reset tygodniowy: jeden push. Okno 5-godzinne: bez pusha.

### Powiadomienia

- Push na telefon z aplikacji Claude przy czterech zdarzeniach: gotowe do bramki 2, ticket `needs-info`,
  awaria środowiska, oczekiwanie na reset tygodniowy. Treść: jedno zdanie i wskazanie briefu.
- Pytanie o zgodę w sesji w tle: powiadomienie aplikacji o sesji wymagającej uwagi.

### Uprawnienia

- Tryb auto dla sesji pętli, nigdy `bypassPermissions`.
- Allowlista rozszerzona o: `git worktree`, `git switch`, `git merge --ff-only`, dokładnie
  `git push origin main` (bez gwiazdki), uruchomienie Docker Desktop. Po pilocie uzupełniana o to,
  o co agent faktycznie pytał.

### Pomiar

- Tabela w katalogu ficzera w trackerze, wiersz per ticket: zużycie okna 5-godzinnego i tygodniowego na
  starcie i na końcu, liczba rund, model w każdej roli, czas, wynik. Na końcu ficzera suma.
- Reguła po dwóch pilotach: jeśli w tygodniowym limicie Pro mieszczą się mniej niż 2 ficzery po 3 tickety,
  to Max 5x albo zewnętrzny implementer. W przeciwnym razie zostaje Pro.

### Warstwa MAUI (etap 2)

- Monorepo według układu Koszyka: `Mobile` (MAUI, `net10.0-android`, CommunityToolkit.Maui, tylko XAML
  i kod platform), `Mobile.Core` (`net10.0`, CommunityToolkit.Mvvm, typowany klient HTTP z
  `Microsoft.Extensions.Http`, ViewModele i serwisy), `Contracts` (DTO współdzielone z Web.Api) i projekt
  testów `Mobile.Core`.
- Skompilowane bindingi (`x:DataType`) są obowiązkowe, więc błąd bindingu w XAML psuje build.
- Testy architektury dostają regułę: `Mobile.Core` nie zależy od MAUI.
- Skill projektu `add-screen`: ViewModel, strona, typowany klient HTTP, rejestracja w `MauiProgram`,
  testy `Mobile.Core`. Wiedza przeniesiona z `implementing-maui-mvvm` z pluginu `dotnet-workflow`, który
  zostaje wyłączony.
- Konwencje mobilne w `AGENTS.md`, obok backendowych.
- CI: instalacja workloadu `maui-android` i build głowy `net10.0-android` obok obecnego buildu i testów.

### Porządki

- Usunięcie nieśledzonego `.agents/skills/` przy wdrożeniu etapu 1.
- `docs/workflow.md` opisuje nowy przepływ: dwie bramki, `/plan-feature`, `/ship`, powiadomienia, limity.

## Testing Decisions

- Dobry test sprawdza zachowanie widoczne z zewnątrz, nie szczegóły implementacji. Seamy są dwa: pilot
  od początku do końca jako najwyższy i skrypty hooków jako proces.
- **Seam 1: pilot.** Prawdziwy ficzer przechodzi `/plan-feature`, zatwierdzenie, `/ship` i kończy się
  briefem na bramce 2. Obserwacja tylko przez artefakty: statusy ticketów i `## Comments`, tabelę pomiaru,
  brief, historię gałęzi, otrzymane powiadomienia. Dwa przebiegi: etap 1 (backend) i etap 2 (full-stack
  z ekranem). Scenariusze wywołane celowo:
  - (a) ścieżka bez problemów;
  - (b) Docker zatrzymany przed startem, pętla uruchamia go sama;
  - (c) pauza i wznowienie po limicie: próg w pliku DoD ustawiony tuż nad bieżącym zużyciem;
  - (d) ticket niemożliwy do zrobienia: `needs-info`, brief, powiadomienie, niezależne tickety idą dalej.
  Pilot jest zaliczony, gdy dochodzi do bramki 2 bez mojego udziału poza bramkami, DoD jest zielony, tabela
  pomiaru jest pełna, a brief kompletny. Każde pytanie o zgodę w trakcie to poprawka allowlisty.
- **Seam 2: skrypty hooków.** Wejście: JSON zdarzenia hooka na stdin i repozytorium przygotowane
  w katalogu tymczasowym. Wyjście: kod wyjścia i decyzja według protokołu hooków Claude Code. Testy xUnit
  w nowym projekcie testowym w solucji, uruchamiające prawdziwy skrypt jako proces, z Shouldly, nazwami
  `{Action}_Should_{Outcome}_When{Condition}` i znacznikami `// Arrange` `// Act` `// Assert`. Przypadki:
  - `Stop` blokuje przy błędzie buildu, przy ostrzeżeniu i przy czerwonym teście, a przepuszcza przy
    zielonym;
  - hooki przepuszczają wszystko, gdy `agent_type` nie jest rolą implementera;
  - `PreToolUse` odrzuca zapis do każdej chronionej ścieżki i przepuszcza zapis poza nimi;
  - progi limitu i lista chronionych ścieżek są czytane z pliku DoD.
- Prior art: testy integracyjne (prawdziwa granica procesu zamiast mockowania wnętrza), testy walidatorów
  (jeden test na regułę), dla warstwy mobilnej projekt testów mobilnych Koszyka (ViewModele z klientem API
  podstawionym przez NSubstitute).
- Testy hooków uruchamia pełne `dotnet test`, więc wchodzą do DoD i do CI (`pwsh` jest na runnerach Ubuntu).
- Teksty skilli w forku nie mają testów automatycznych. Sprawdza je seam 1.
- Warstwa MAUI: testy `Mobile.Core` według `add-screen` i reguła w testach architektury. Bez automatyzacji
  UI.

## Out of Scope

- Zewnętrzny implementer: dostawca, narzędzie, prywatność kodu. Wraca dopiero po dwóch pilotach, jeśli
  liczby przekroczą próg. Kontrakt implementera jest przygotowany pod podmianę.
- Zmiana planu (Max 5x) i włączenie dodatkowego płatnego użycia: decyzja po pilotach.
- Równoległe tickety i agent scalający.
- Sesje w chmurze.
- Targety iOS, MacCatalyst i Windows; automatyzacja UI (Appium); zrzuty ekranu w DoD.
- Wariant template'u tylko z MAUI (bez backendu, jak Radio).
- Automatyczne scalanie bez bramki 2.
- Ponowne włączenie pluginu `dotnet-workflow`.
- Przeniesienie istniejących aplikacji (Gym, Koszyk) na pętlę.
- Zmiany konwencji backendu poza tym, czego wymaga pętla.

## Further Notes

- Decyzje pochodzą z grillowania 2026-10-26 (Q1–Q26, wszystkie rekomendacje przyjęte). Kilka szczegółów
  dopisałem przy spisywaniu speca, bez osobnego pytania. Warto je potwierdzić przy `/to-tickets`:
  - uwagi z review całej gałęzi przechodzą tę samą regułę 3 rund;
  - hooki działają tylko w sub-agencie implementera, rozpoznanym po `agent_type`, i rejestruje je
    template (zmienione przy `/to-tickets`: wcześniej „tylko przy tickecie w toku”; dokumentacja Claude
    Code potwierdza, że hooki we frontmatterze agentów z pluginu są ignorowane);
  - reguła testów architektury „`Mobile.Core` nie zależy od MAUI”;
  - planowana nazwa testu przy każdym `ACn` w tickecie;
  - push przy pytaniu o zgodę opiera się na powiadomieniu aplikacji o sesji wymagającej uwagi.
    Do sprawdzenia w pilocie; jeśli push nie dociera na telefon, fallback to hook `Notification` i ntfy.
- Tickety obejmą dwa repozytoria: fork skilli (`D:\Projects\skills`, gałąź `dotnet`, podbicie wersji,
  aktualizacja pluginu i restart sesji według `docs/workflow.md`, sekcja „Utrzymanie”) oraz ten template.
- Etap 1 to tickety teraz. Etap 2 (MAUI, CI Androida) dostaje tickety po pilocie 1, bo pilot może
  zmienić kształt pętli.
- Stan limitu w chwili planowania: plan Pro, sesja planująca zużyła około 7% okna 5-godzinnego. Pomiar
  z pilotów zastąpi tę anegdotę liczbami.
- Lekcja z `dotnet-workflow`: im więcej własnego kodu sterującego, tym więcej kontraktów do utrzymania.
  Własnego kodu ma być tyle, ile mieszczą hooki i definicje ról.
