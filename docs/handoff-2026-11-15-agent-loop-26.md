# Handoff: domknięcie pilota 2 (ticket 26) po E2E Release w Koszyku

Pętla bez nadzoru jest zbudowana i zamrożona. W tym template'cie nie ma już pracy dla agenta: otwarte tickety
12, 19, 26 i 27 czekają na pracę w Koszyku (`D:\Projects\Koszyk`). Ta sesja nie była kompaktowana; wszystko
poniżej jest z pierwszej ręki.

## 1. Stan wejściowy

- **Template** (`D:\Projects\clean-architecture_new`): gałąź `chore/agent-loop`. `main` i `chore/agent-loop`
  wskazują na ten sam commit, lokalnie i na origin; sprawdź `git branch -vv`. Drzewo czyste.
- **Testy:** w tej sesji nie uruchamiałem ani builda, ani testów. Wszystkie zmiany z 2026-11-15 to dokumenty
  (`.scratch/agent-loop/issues/11-*.md`, `26-*.md`, `docs/workflow.md`, `docs/workflow-diagram.html`). CI na
  `main` po dzisiejszych pushach nie było sprawdzane.
- **Tickety pętli** (`.scratch/agent-loop/issues/`): `done` 01–10, 11 (zamknięty 2026-11-15), 13–18, 20–25,
  31–33; `wontfix` 28–30 (warstwa MAUI template'u, zaparkowana); `ready-for-human` 12, 19, 26, 27.
- **Koszyk:** `main` = `origin/main` = `b2d0ea8`, drzewo czyste (stan z 2026-11-15, odczytany z repozytorium).
  Scalone przez `/ship`: `ux-cosmetics-1`, `emulator-tls-release-e2e`, `mobile-smoke-emulator`. CI Koszyka
  istnieje od 2026-11-13 (ticket 24).

## 2. Co robimy dalej

Kolejność, każdy krok w innym repozytorium:

1. **W Koszyku:** E2E Release po HTTPS na emulatorach według
   `D:\Projects\Koszyk\docs\tasks\handoff-15-release-e2e-and-triage.md`. To osobna sesja z katalogiem roboczym
   w Koszyku. Ten przebieg obejmuje zaległe obejrzenia ekranów, czyli kryterium AC2 ticketu 26.
2. **Tutaj:** po E2E dopisz wynik do ticketu 26 i zapytaj użytkownika, czy go zamknąć (pytania w sekcji 4).
3. **W Koszyku:** następny ficzer modułu 15 przez `/plan-feature` i `/ship` (ticket 27). Przy tym przebiegu
   użytkownik obserwuje dwie rzeczy z ticketu 19: wznowienie po pauzie bez swojego udziału i powiadomienie ntfy
   przy pytaniu o zgodę (AC5 ticketu 12).

Pliku zadania dla kroku 2 nie ma i nie trzeba: to wpis w tickecie.

## 3. Co przeczytać na start

- `.scratch/agent-loop/issues/26-pilot-koszyk.md`, wpis „2026-11-15, przebiegi 2 i 3”: co każdy przebieg mówi
  o kryteriach AC1–AC5 i czego brakuje do zamknięcia.
- `.scratch/agent-loop/issues/27-koszyk-module-15.md`: kryteria AC1–AC3 (podział modułu 15 na ficzery, każdy
  w `main`, roadmapa Koszyka).
- `.scratch/agent-loop/issues/19-pilot-leftovers-resume-and-permission-push.md`, `## Comments`: dwie obserwacje
  i wpis z 2026-11-11 o restarcie w trakcie czekania.
- `D:\Projects\Koszyk\docs\tasks\handoff-15-release-e2e-and-triage.md`, sekcje 2 i 4: zakres E2E i pytania na
  bramkę. Jego sekcja 1 jest nieaktualna w jednym punkcie: `origin/main` Koszyka to już `b2d0ea8`, nie `626b1e5`.
- `docs/workflow.md`, „Reguła decyzji po pilotach”: decyzja o planie i korekta liczby kosztu.

## 4. Otwarte pytania na bramkę

1. **Czy AC1 ticketu 26 uznać za spełnione przebiegiem 2** (`emulator-tls-release-e2e`: do `main` na GitHubie
   bez udziału użytkownika poza bramkami)? Druga część kryterium (pytania o zgodę trafiają do allowlisty) nie
   ma żadnej obserwacji. Warianty: (a) uznać i odhaczyć, część o allowliście przenieść do ticketu 19;
   (b) czekać na przebieg z pytaniem o zgodę. Rekomendacja: (a), bo ticket 19 już zbiera tę obserwację.
2. **Seam „CI po scaleniu”** z ticketu 26 nie był sprawdzony w żadnym przebiegu (CI Koszyka powstało po
   ostatnim scaleniu). Warianty: (a) sprawdzić przy następnym `/ship` w ramach ticketu 27; (b) trzymać ticket 26
   otwarty do tego czasu. Rekomendacja: (a).
3. **Podział modułu 15** (AC1 ticketu 27) wymienia „wystawienie API po TLS dla emulatora z E2E Release”, a to
   jest już scalone jako `emulator-tls-release-e2e`. Przy pierwszym `/plan-feature` trzeba ustalić resztę
   kolejności: niezawodność sync i auth, motyw i branding, reszta kosmetyki UX, backend i schemat. Handoff
   Koszyka (sekcja 4, pytanie 3) proponuje jeszcze mały ficzer `mobile-smoke-hardening` z ticketów po triage.

## 5. Dług i ustalenia przeniesione

Nie spłacaj ich tutaj bez decyzji.

- **Ticket 12, AC5** (hook `Notification` przy pytaniu o zgodę): wpisy w `settings.json` i `CLAUDE_NTFY_URL` są
  ustawione od 2026-11-11, brakuje tylko obserwacji. Należy do ticketu 19.
- **Ticket 19:** wznowienia po pauzie 5-godzinnej bez restartu systemu nikt jeszcze nie widział. Poprawka forka
  `1.3.1-dotnet.35` (komunikat o przerwanym czekaniu oznacza „wznów z trackera”) nie była sprawdzona na żywo.
- **Sześć ticketów Koszyka po triage z 2026-11-13** (TLS 04, 05; smoke 04–07): należą do Koszyka, ich los
  rozstrzyga pytanie 3 w handoffie Koszyka.
- **Koszt ficzera:** „ok. 5% tygodnia z planem” to dolna granica. Przebiegi 2 i 3 w Koszyku kosztowały 4 pp
  i ponad 9 pp bez planu. Decyzja „zostaje Pro” (ticket 11) się nie zmienia; wróć do niej tylko wtedy, gdy
  dwa ficzery przestaną się mieścić w tygodniu.
- **Reguła zamrożenia pętli** (2026-11-11): zmiany w pętli tylko po nieudanym realnym przebiegu. Warstwa MAUI
  template'u (28–30) zostaje zaparkowana.

## 6. Środowisko i pułapki, które kosztowały czas

- **Wiersz `metrics.md` obejmujący pauzę nie mierzy kosztu.** Ticket 03 w `mobile-smoke-emulator` ma 1179 minut
  i tydzień 22% → 48%, ale to 19 godzin zegara z pracą innych sesji. Do porównań bierz tylko wiersze bez pauzy.
- **Ticket potrafi być o kilka przebiegów do tyłu.** Treść ticketu 26 kończyła się na 2026-11-05, a w Koszyku
  były już dwa kolejne przebiegi. Stan Koszyka czytaj z jego repozytorium: `git log`, reflog `origin/main`
  (pokazuje, kiedy i czym przesunął się `main`), `.scratch/<slug>/metrics.md` i `brief.md`.
- **Przesunięcie `main` bez checkoutu:** `git fetch . chore/agent-loop:main` (tylko fast-forward), potem
  `git push origin main`. Przed tym `git merge-base --is-ancestor origin/main HEAD`.
- **Push z sesji agenta** przeszedł 2026-11-15 po wyraźnym „tak” użytkownika w rozmowie. Wcześniejsza notatka,
  że tryb auto blokuje push, nie potwierdziła się w tej sesji.
- **Logów CI na GitHubie nie da się odczytać z sesji** (prywatne repozytorium, przeglądarka wylogowana, bez `gh`).
  Wynik CI podaje użytkownik.

## 7. Workflow

Bez zmian. Bramki: 1 po `/plan-feature` (akceptacja planu), 2 po briefie `/ship` (scalenie i propozycje
konwencji). Planowanie na Opusie, `/ship` na Sonnecie. W tej sesji nie działali żadni agenci w tle i nie ma
nic do wznowienia. Commity i pushe w template'cie: dopiero po słowie użytkownika.

## 8. Lekcje metodyczne do powtórzenia

1. **Zanim doradzisz następny krok w innym repozytorium, przeczytaj jego ostatni handoff.** W tej sesji
   najpierw poleciłem `/plan-feature` w Koszyku na podstawie ticketu 27, a handoff Koszyka wskazywał E2E Release
   jako krok wcześniejszy. Ticket w template'cie opisuje zamiar, handoff w Koszyku opisuje stan.
2. **Liczbę wpisaną do decyzji sprawdź na wszystkich pomiarach, nie tylko na tych z ticketu.** „Ok. 5%” wziąłem
   z pomiarów nr 1 i 2, a w Koszyku leżały dwa nowsze przebiegi z wyższym kosztem. Korekta poszła tego samego
   dnia, ale decyzja była już zapisana.
3. **Nie odhaczaj kryterium, którego druga połowa nie ma obserwacji.** AC1 ticketu 26 ma dwa zdania; przebieg 2
   spełnia pierwsze. Zostawiłem checkbox pusty i opisałem, czego brakuje, bo odhaczenie to decyzja użytkownika.
4. **Nieśledzony plik, który nie powstał w sesji, obejrzyj przed commitem** (rozmiar, początek, ciągi
   wyglądające na sekrety) i commituj osobno, dopiero po zgodzie.

## 9. Pierwszy krok i miejsce zatrzymania

Zacznij od `git -C D:\Projects\Koszyk log --format="%h %ad %s" --date=short -15` i sprawdź, czy po `b2d0ea8`
jest już przebieg E2E Release (commity z roadmapą 13 i 14 jako ✅ albo nowy handoff w `docs/tasks/`).

- Jeśli E2E jest zrobione: dopisz jego wynik do ticketu 26 (AC2) i zatrzymaj się na pytaniach 1 i 2 z sekcji 4.
- Jeśli nie: powiedz użytkownikowi, że następny krok to sesja w Koszyku z
  `Wykonaj docs/tasks/handoff-15-release-e2e-and-triage.md`, i zatrzymaj się. W template'cie nie ma wtedy nic
  do zrobienia.
