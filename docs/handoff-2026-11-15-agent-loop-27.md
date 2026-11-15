# Handoff: obserwacje z przebiegu `mobile-smoke-hardening` (tickety 27, 19, 12)

Pilot 2 jest zamknięty (ticket 26 `done`). W template'cie zostały trzy otwarte tickety, wszystkie
`ready-for-human`: 12, 19 i 27. Każdy czeka na obserwacje z przebiegów `/ship` w Koszyku
(`D:\Projects\Koszyk`). Ta sesja nie była kompaktowana; wszystko poniżej jest z pierwszej ręki.

## 1. Stan wejściowy

- **Template** (`D:\Projects\clean-architecture_new`): gałąź `chore/agent-loop`; `main` i `chore/agent-loop`
  wskazują na ten sam commit lokalnie i na origin (sprawdź `git branch -vv`). Drzewo czyste.
- **Testy:** w tej sesji nie uruchamiałem builda ani testów; wszystkie zmiany z 2026-11-15 to dokumenty.
  Wynik CI na `main` po dzisiejszych pushach nie był sprawdzany.
- **Tickety pętli** (`.scratch/agent-loop/issues/`): `done` 01–11, 13–18, 20–26, 31–33; `wontfix` 28–30;
  `ready-for-human` 12, 19, 27.
- **Koszyk, stan z 2026-11-15 21:09** (odczytany z repozytorium, bez raportu sesji `/ship`):
  - `main` lokalnie `3f2f371` (`plan: mobile-smoke-hardening`), `origin/main` `ff5cdbf`: plan nie był wypchnięty.
  - **Przebieg `/ship mobile-smoke-hardening` trwał w innej sesji**: worktree
    `D:\Projects\Koszyk\.claude\worktrees\mobile-smoke-hardening`, gałąź `feat/mobile-smoke-hardening`
    (`e746a1d`). Ticket 01 `done` po 2 rundach (20:52–21:02, 5h 4% → 26%, tydzień 30% → 33%), ticket 02
    `in-progress`, 03–05 `ready-for-agent`, 06 `needs-triage` (nowy, spoza zakresu).
  - E2E Release po HTTPS zaliczone 2026-11-15; moduły 13 i 14 mają ✅ w roadmapie Koszyka.

## 2. Co robimy dalej

Nic nie budujemy. Po zakończeniu przebiegu `mobile-smoke-hardening` (bramka 2 i scalenie należą do użytkownika
i do sesji `/ship`) zapisz jego obserwacje w ticketach template'u:

1. **Ticket 27:** seam „CI po scaleniu” (to pierwszy przebieg `/ship` z CI Koszyka) i postęp AC2.
2. **Ticket 19:** czy padła pauza na limit i czy przebieg wznowił się sam; czy padło pytanie o zgodę, czy
   przyszło powiadomienie i czy pytanie trafiło do allowlisty Koszyka.
3. **Ticket 12, AC5:** czy hook `Notification` wysłał powiadomienie ntfy przy pytaniu o zgodę.

Pliku zadania nie ma i nie trzeba: to wpisy w ticketach.

## 3. Co przeczytać na start

- `.scratch/agent-loop/issues/27-koszyk-module-15.md`, kryteria AC1–AC3 i `## Comments` z 2026-11-15: co już
  jest zrobione z zakresu modułu 15 i co sprawdzić przy pierwszym przebiegu.
- `.scratch/agent-loop/issues/19-pilot-leftovers-resume-and-permission-push.md`, `## Comments`: dwie obserwacje,
  wpis o restarcie z 2026-11-11 i dopisek o allowliście z 2026-11-15.
- `.scratch/agent-loop/issues/12-push-fallback-ntfy.md`, AC5 i ostatni wpis: co jest ustawione, czego brakuje.
- `.scratch/agent-loop/issues/26-pilot-koszyk.md`, wpisy z 2026-11-15: wzór zapisu przebiegu z historii Koszyka
  (źródła, tabela, ocena kryteriów) i uzasadnienie zamknięcia.
- `D:\Projects\Koszyk\docs\tasks\handoff-15-smoke-hardening-and-next-feature.md`, sekcje 2 i 4: zakres ficzera
  i wybór następnego ficzera modułu 15.

## 4. Otwarte pytania na bramkę

1. **Czy wynik CI po scaleniu `mobile-smoke-hardening` jest zielony.** Agent nie ma dostępu do strony Actions;
   wynik podaje użytkownik albo raport końcowy `/ship`. Bez tego seam „CI po scaleniu” zostaje otwarty.
2. **Czy w przebiegu padło pytanie o zgodę i czy przyszło powiadomienie na telefon.** To wie tylko użytkownik
   (telefon) i log sesji `/ship`. Warianty zapisu: (a) użytkownik mówi, co widział; (b) agent szuka w logu sesji
   w `~/.claude/projects/` wywołań `PushNotification` i `notify.ps1`. Rekomendacja: (a), a (b) tylko gdy
   użytkownik nie pamięta.
3. **Następny ficzer modułu 15** (AC1 ticketu 27). Handoff Koszyka rekomenduje sync i auth (jedyna grupa
   z ryzykiem utraty danych). Decyzja zapada w sesji Koszyka; tutaj tylko ją odnotuj.
4. **Błąd AppHost znaleziony w E2E** (ficzer TLS przeszedł DoD, review i bramkę 2, a `api` nie startowało).
   Nie ma ticketu pętli, bo reguła zamrożenia wymaga nieudanego przebiegu `/ship`. Warianty: (a) zostawić;
   (b) ticket `needs-triage` o smoke'u backendu w DoD. Rekomendacja: (a), dopóki to się nie powtórzy.

## 5. Dług i ustalenia przeniesione

Nie spłacaj ich tutaj bez decyzji.

- **Ticket 12, AC5** i **ticket 19**: same obserwacje, bez pracy w kodzie, dopóki obserwacja nie pokaże błędu.
- **Poprawka forka `1.3.1-dotnet.35`** (komunikat o przerwanym czekaniu oznacza „wznów z trackera”) nie była
  sprawdzona na żywo. Należy do ticketu 19.
- **Koszt ficzera:** „ok. 5% tygodnia z planem” to dolna granica (ticket 11, korekta z 2026-11-15). Ticket 01
  bieżącego przebiegu kosztował 3 pp tygodnia i 22 pp okna 5h w 10 minut. Decyzja „zostaje Pro” stoi; wróć do
  niej tylko wtedy, gdy dwa ficzery przestaną się mieścić w tygodniu.
- **Reguła zamrożenia pętli** (2026-11-11): zmiany tylko po nieudanym realnym przebiegu. Warstwa MAUI
  template'u (28–30) zostaje zaparkowana.
- **Ticket 06 w `mobile-smoke-hardening`** (`needs-triage`, strażnik konwencji Skrypty) i reszta ticketów
  Koszyka należą do Koszyka.

## 6. Środowisko i pułapki, które kosztowały czas

- **Nie dotykaj Koszyka, gdy trwa `/ship`.** Commit na `main` Koszyka w trakcie przebiegu zatrzyma bramkę 2
  (przesunięty `main`). Czytaj wyłącznie: `git log`, `git worktree list`, pliki w worktree.
- **Stan przebiegu w toku jest w worktree, nie na `main`:**
  `D:\Projects\Koszyk\.claude\worktrees\<slug>\.scratch\<slug>\metrics.md` i `issues/*.md`. Po scaleniu te same
  pliki są w `.scratch/<slug>/` na `main`.
- **Wiersz `metrics.md` obejmujący pauzę nie mierzy kosztu** (ticket 26, wpis o przebiegach 2 i 3).
- **Reflog `origin/main`** (`git reflog show origin/main --date=iso`) pokazuje, kiedy `main` przesunął się
  pushem; ok. 20 s po commicie `ship: gate 2` oznacza, że scalił krok `/ship`, nie użytkownik.
- **Przesunięcie `main` template'u bez checkoutu:** `git merge-base --is-ancestor origin/main HEAD`, potem
  `git fetch . chore/agent-loop:main` i `git push origin main`. Push z sesji agenta działa po „tak” użytkownika.
- **`sed -i` i `cat >>` z Git Basha** zmieniają pliki poza narzędziem Edit; po nich Edit wymaga ponownego Read.
- **Logów CI nie da się odczytać z sesji** (prywatne repozytoria, bez `gh`, przeglądarka wylogowana).

## 7. Workflow

Bez zmian. Bramka 1 po `/plan-feature`, bramka 2 po briefie `/ship`; oba polecenia uruchamia użytkownik
w sesji Koszyka. W tej sesji template'u nie działali żadni agenci w tle. Commity i pushe w template'cie po
słowie użytkownika; w tej sesji zgadzał się każdorazowo.

## 8. Lekcje metodyczne do powtórzenia

1. **Zanim doradzisz krok w innym repozytorium, przeczytaj jego ostatni handoff i `git log`.** Dwa razy tego
   dnia stan Koszyka był dalej, niż mówiły tickety template'u (najpierw dwa przebiegi `/ship`, potem zrobione
   E2E i plan nowego ficzera). Ticket opisuje zamiar, repozytorium opisuje stan.
2. **Liczbę wpisaną do decyzji sprawdź na wszystkich pomiarach.** „Ok. 5%” wziąłem z dwóch pomiarów z ticketów,
   a nowsze przebiegi kosztowały więcej; korekta poszła po fakcie.
3. **Kryterium z dwiema częściami odhacza użytkownik, nie agent.** AC1 i AC2 ticketu 26 były spełnione
   częściowo albo inaczej, niż zapisano. Opisałem różnicę, zapytałem i dopiero po „tak” odhaczyłem z adnotacją.
4. **Luka znaleziona poza pętlą nie jest od razu ticketem pętli.** Błąd AppHost zapisałem jako wniosek
   w tickecie 26, bo reguła zamrożenia chroni przed przebudową po każdym pojedynczym sygnale.

## 9. Pierwszy krok i miejsce zatrzymania

Zacznij od `git -C D:\Projects\Koszyk worktree list` i `git -C D:\Projects\Koszyk log --format="%h %ad %s"
--date=short -15 main`.

- Jeśli worktree `mobile-smoke-hardening` nadal istnieje (przebieg trwa albo czeka na bramkę 2): powiedz to
  użytkownikowi, podaj stan z `metrics.md` w worktree i zatrzymaj się. Niczego nie zapisuj w Koszyku.
- Jeśli ficzer jest w `main` (commit `ship: gate 2`): dopisz przebieg do ticketów 27 i 19 według wzoru z ticketu
  26, zadaj pytania 1 i 2 z sekcji 4 i zatrzymaj się na odpowiedziach. Ticketów nie zamykaj bez decyzji
  użytkownika.
